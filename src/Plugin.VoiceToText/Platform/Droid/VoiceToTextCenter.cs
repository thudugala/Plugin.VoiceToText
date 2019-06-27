using Android.App;
using Android.Content;
using Android.Speech;
using System;
using System.Linq;

namespace Plugin.VoiceToText
{
    public static partial class VoiceToTextCenter
    {
        internal static Activity MyActivity { get; set; }

        /// <summary>
        /// Prompt Text
        /// </summary>
        public static string Prompt { get; set; } = "Speak now";

        /// <summary>
        /// Voice Language, Java.Util.Locale.Default
        /// </summary>
        public static Java.Util.Locale Language { get; set; } = Java.Util.Locale.Default;

        /// <summary>
        /// This plugin registered as 19951984
        /// </summary>
        internal const int RequestCode = 10;

        /// <summary>
        /// Init Pluggin.
        /// </summary>
        /// <param name="activity"></param>
        public static void Init(Activity activity)
        {
            MyActivity = activity;
        }

        static VoiceToTextCenter()
        {
            try
            {
                Current = new Platform.Droid.VoiceToTextServiceImpl();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Notify Local Notification Tapped.
        /// </summary>
        public static void OnTextReceived(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                if (requestCode != RequestCode)
                {
                    return;
                }

                if (resultCode == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Any())
                    {
                        var eventArg = new TextReceivedEventArg
                        {
                            Text = matches[0]
                        };

                        Current.OnTextReceived(eventArg);
                    }
                }
                Current.OnStoppedListening();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}