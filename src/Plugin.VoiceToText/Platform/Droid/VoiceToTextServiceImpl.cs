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
                var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);

                voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak now");

                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
                voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);

                VoiceToTextCenter.MyActivity.StartActivityForResult(voiceIntent, VoiceToTextCenter.RequestCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <inheritdoc />
        public void StopListening()
        {
            // Ignored
        }
    }
}