using System;

namespace Plugin.VoiceToText
{
    /// <summary>
    /// Cross platform IVoiceToTextService Resolver.
    /// </summary>
    public static partial class VoiceToTextCenter
    {
        private static IVoiceToTextService _current;

        /// <summary>
        /// Platform specific INotificationService.
        /// </summary>
        public static IVoiceToTextService Current
        {
            get =>
                _current ?? throw new ArgumentException(
                    "[Plugin.VoiceToText] No platform plugin found.  Did you install the nuget package in your app project as well?");
            set => _current = value;
        }
    }
}