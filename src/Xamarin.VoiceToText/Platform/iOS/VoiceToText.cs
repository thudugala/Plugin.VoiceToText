using AVFoundation;
using Foundation;
using Speech;
using System;
using Xamarin.Forms;
using Xamarin.VoiceToText.Platform.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(VoiceToText))]

namespace Xamarin.VoiceToText.Platform.iOS 
{
    public class VoiceToText : IVoiceToText
    {
        private readonly AVAudioEngine _audioEngine;
        private readonly SFSpeechRecognizer _speechRecognizer;
        private SFSpeechAudioBufferRecognitionRequest _recognitionRequest;
        private SFSpeechRecognitionTask _recognitionTask;
        private string _recognizedString;
        private bool _isAuthorized;
        private NSTimer _timer;

        public VoiceToText()
        {
            AskForSpeechPermission();

            _audioEngine = new AVAudioEngine();
            _speechRecognizer = new SFSpeechRecognizer();
        }

        public void StartVoiceToText()
        {
            if (_audioEngine.Running)
            {
                StopRecordingAndRecognition();
            }
            StartRecordingAndRecognizing();
        }

        public void StopVoiceToText()
        {
            StopRecordingAndRecognition();
        }

        private void AskForSpeechPermission()
        {
            SFSpeechRecognizer.RequestAuthorization(status =>
            {
                switch (status)
                {
                    case SFSpeechRecognizerAuthorizationStatus.Authorized:
                        _isAuthorized = true;
                        break;

                    case SFSpeechRecognizerAuthorizationStatus.Denied:
                        break;

                    case SFSpeechRecognizerAuthorizationStatus.NotDetermined:
                        break;

                    case SFSpeechRecognizerAuthorizationStatus.Restricted:
                        break;
                }
            });
        }

        private void DidFinishTalk()
        {
            MessagingCenter.Send<IVoiceToText>(this, "Final");
            if (_timer != null)
            {
                _timer.Invalidate();
                _timer = null;
            }

            if (_audioEngine.Running)
            {
                StopRecordingAndRecognition();
            }
        }

        private void StartRecordingAndRecognizing()
        {
            try
            {
                _timer = NSTimer.CreateRepeatingScheduledTimer(5, delegate
                {
                    DidFinishTalk();
                });

                _recognitionTask?.Cancel();
                _recognitionTask = null;

                var audioSession = AVAudioSession.SharedInstance();
                var nsError = audioSession.SetCategory(AVAudioSessionCategory.PlayAndRecord);
                audioSession.SetMode(AVAudioSession.ModeDefault, out nsError);
                nsError = audioSession.SetActive(true, AVAudioSessionSetActiveOptions.NotifyOthersOnDeactivation);
                audioSession.OverrideOutputAudioPort(AVAudioSessionPortOverride.Speaker, out nsError);
                _recognitionRequest = new SFSpeechAudioBufferRecognitionRequest();

                var inputNode = _audioEngine.InputNode;
                if (inputNode == null)
                {
                    throw new Exception();
                }

                var recordingFormat = inputNode.GetBusOutputFormat(0);
                inputNode.InstallTapOnBus(0, 1024, recordingFormat, (buffer, when) =>
                {
                    _recognitionRequest?.Append(buffer);
                });

                _audioEngine.Prepare();
                _audioEngine.StartAndReturnError(out nsError);

                _recognitionTask = _speechRecognizer.GetRecognitionTask(_recognitionRequest, (result, error) =>
                {
                    if (result != null)
                    {
                        _recognizedString = result.BestTranscription.FormattedString;
                        MessagingCenter.Send<IVoiceToText, string>(this, "STT", _recognizedString);
                        _timer.Invalidate();
                        _timer = null;
                        _timer = NSTimer.CreateRepeatingScheduledTimer(2, delegate
                        {
                            DidFinishTalk();
                        });
                    }

                    if (error == null)
                    {
                        return;
                    }

                    MessagingCenter.Send<IVoiceToText>(this, "Final");
                    StopRecordingAndRecognition(audioSession);
                });
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message); 
            }
        }

        private void StopRecordingAndRecognition(AVAudioSession aVAudioSession = null)
        {
            try
            {
                if (!_audioEngine.Running)
                {
                    return;
                }

                _audioEngine.Stop();
                _audioEngine.InputNode.RemoveTapOnBus(0);
                _recognitionTask?.Cancel();
                _recognitionRequest.EndAudio();
                _recognitionRequest = null;
                _recognitionTask = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}