using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.VoiceToText;

namespace Sample
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            VoiceToTextCenter.Current.TextReceived += Current_TextReceived;
            VoiceToTextCenter.Current.StoppedListening += Current_StoppedListening;
        }

        protected override void OnDisappearing()
        {
            VoiceToTextCenter.Current.TextReceived -= Current_TextReceived;
            VoiceToTextCenter.Current.StoppedListening -= Current_StoppedListening;

            base.OnDisappearing();
        }

        private void Current_StoppedListening()
        {
            Micro.IsEnabled = true;
        }

        private void Current_TextReceived(TextReceivedEventArg e)
        {
            RecordLabel.Text = e.Text;
        }

        private void Micro_OnClicked(object sender, EventArgs e)
        {
            try
            {
                VoiceToTextCenter.Current.StartListening();
                Micro.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}