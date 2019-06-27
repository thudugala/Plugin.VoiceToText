using Android.Content;
using Android.Speech;
using System;

namespace Plugin.VoiceToText.Platform.Droid
{
    /// <inheritdoc />
    [Android.Runtime.Preserve]
    public class VoiceToTextServiceImpl : IVoiceToTextService
    {
        /// <inheritdoc />
        public event TextReceivedEventHandler TextReceived;

        /// <inheritdoc />
        public event StoppedListeningEventHandler StoppedListening;

        /// <inheritdoc />
        public void OnTextReceived(TextReceivedEventArg e)
        {
            TextReceived?.Invoke(e);
            StoppedListening?.Invoke();
        }

        /// <inheritdoc />
        public void StartListening()
        {
            if (VoiceToTextCenter.MyActivity.PackageManager.HasSystemFeature(Android.Content.PM.PackageManager
                    .FeatureMicrophone) == false)
            {
                throw new AccessViolationException("You don't seem to have a microphone to record with");
            }

            var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);

            voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, VoiceToTextCenter.Prompt);

            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            voiceIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, VoiceToTextCenter.MyActivity.PackageName);

            VoiceToTextCenter.MyActivity.StartActivityForResult(voiceIntent, VoiceToTextCenter.RequestCode);
        }

        /// <inheritdoc />
        public void StopListening()
        {
            StoppedListening?.Invoke();
        }
    }
}