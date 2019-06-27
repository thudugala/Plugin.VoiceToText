using System;

namespace Plugin.VoiceToText
{
    public static partial class VoiceToTextCenter
    {
        static VoiceToTextCenter()
        {
            try
            {
                Current = new Platform.iOS.VoiceToTextServiceImpl();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}