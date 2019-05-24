using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.VoiceToText;

namespace Sample
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        private IVoiceToText _speechRecongnitionInstance;
        public MainPage()
        {
            InitializeComponent();
            _speechRecongnitionInstance = DependencyService.Get<IVoiceToText>();
            MessagingCenter.Subscribe<IVoiceToText, string>(this, "STT", (sender, args) =>
            {
                SpeechToTextFinalResultReceived(args);
            });

            MessagingCenter.Subscribe<IVoiceToText>(this, "Final", (sender) =>
            {
                Micro.IsEnabled = true;
            });

            MessagingCenter.Subscribe<IVoiceMessageSender, string>(this, "STT", (sender, args) =>
            {
                SpeechToTextFinalResultReceived(args);
            });
        }
        private void SpeechToTextFinalResultReceived(string args)
        {
            RecordLabel.Text = args;
        }

        private void Micro_OnClicked(object sender, EventArgs e)
        {
            try
            {
                _speechRecongnitionInstance.StartVoiceToText();
                if (Device.RuntimePlatform == Device.iOS)
                {
                    Micro.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
