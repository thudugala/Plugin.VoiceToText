using System;
using Android.App;
using Android.Content;
using Android.Speech;
using Plugin.VoiceToText.Platform.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(VoiceToText))]

namespace Plugin.VoiceToText.Platform.Droid
{
    public class VoiceToText : IVoiceToText 
    {
        private static Activity _activity;
        private const int Voice = 10;

        public static void Init(Activity activity)
        {
            _activity = activity;
        }

        public void StartVoiceToText()
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
                _activity.StartActivityForResult(voiceIntent, Voice);
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.Message); 
            }
        }

        public void StopVoiceToText()
        {
        }
    }
}
