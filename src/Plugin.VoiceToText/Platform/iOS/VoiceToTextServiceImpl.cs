using AVFoundation;
using Foundation;
using Speech;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.VoiceToText.Platform.iOS
{
    /// <inheritdoc />
    [Foundation.Preserve]
    public class VoiceToTextServiceImpl : IVoiceToTextService
    {
        private readonly AVAudioEngine _audioEngine;
        private readonly SFSpeechRecognizer _speechRecognizer;
        private SFSpeechAudioBufferRecognitionRequest _recognitionRequest;
        private SFSpeechRecognitionTask _recognitionTask;
        private bool _isAuthorized;
        private NSTimer _timer;

        /// <inheritdoc />
        public event TextReceivedEventHandler TextReceived;

        /// <inheritdoc />
        public event StoppedListeningEventHandler StoppedListening;

        /// <inheritdoc />
        public void OnTextReceived(TextReceivedEventArg e)
        {
            TextReceived?.Invoke(e);
        }

        /// <summary>
        /// Internal use Only
        /// </summary>
        public void OnStoppedListening()
        {
            StoppedListening?.Invoke();
        }

        /// <inheritdoc />
        public void StartListening()
        {
            try
            {
                if (_isAuthorized)
                {
                    return;
                }

                if (_audioEngine.Running)
                {
                    StopRecordingAndRecognition();
                }

                StartRecordingAndRecognizing();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void StopListening()
        {
            try
            {
                if (_isAuthorized)
                {
                    return;
                }

                StopRecordingAndRecognition();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public VoiceToTextServiceImpl()
        {
            try
            {
                AskForSpeechPermission();

                _audioEngine = new AVAudioEngine();
                _speechRecognizer = new SFSpeechRecognizer();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
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
            OnStoppedListening();
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
                        var eventArg = new TextReceivedEventArg
                        {
                            Text = result.BestTranscription.FormattedString
                        };

                        OnTextReceived(eventArg);

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

                    OnStoppedListening();
                    StopRecordingAndRecognition();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void StopRecordingAndRecognition()
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