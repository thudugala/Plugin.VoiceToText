using System;

namespace Plugin.VoiceToText
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="e"></param>
    public delegate void TextReceivedEventHandler(TextReceivedEventArg e);

    /// <summary>
    /// Returning event after converting voice.
    /// </summary>
    public class TextReceivedEventArg : EventArgs
    {
        /// <summary>
        /// Returning text after converting voice.
        /// </summary>
        public string Text { get; internal set; }
    }
}