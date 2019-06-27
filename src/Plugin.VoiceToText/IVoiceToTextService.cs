namespace Plugin.VoiceToText
{
    /// <summary>
    /// Used, to convert Voice to Text
    /// </summary>
    public interface IVoiceToTextService
    {
        /// <summary>
        /// fires when Stopped listening to voice.
        /// </summary>
        event StoppedListeningEventHandler StoppedListening;

        /// <summary>
        /// fires when Text is received.
        /// </summary>
        event TextReceivedEventHandler TextReceived;

        /// <summary>
        /// Internal use Only
        /// </summary>
        void OnStoppedListening();

        /// <summary>
        /// Internal use Only
        /// </summary>
        /// <param name="e"></param>
        void OnTextReceived(TextReceivedEventArg e);

        /// <summary>
        /// Start listening to voice
        /// </summary>
        void StartListening();

        /// <summary>
        /// Stop listening to voice
        /// </summary>
        void StopListening();
    }
}