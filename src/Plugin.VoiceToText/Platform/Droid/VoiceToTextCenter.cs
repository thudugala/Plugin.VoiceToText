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
        /// This plugin registered as 1995
        /// </summary>
        internal const int RequestCode = 1995;

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
                if (requestCode != RequestCode || resultCode != Result.Ok)
                {
                    return;
                }

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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}