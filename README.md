<img src="Screenshots/icon.png" width="64px" >

# Plugin.VoiceToText
Convert voice input to text

# Building Status

<img src="https://ci.appveyor.com/api/projects/status/github/masonyc/Plugin.VoiceToText?svg=true" width="100">

# Setup

- Need Xamarin.Forms version 3 or above
- `Plugin.VoiceToText` Available on NuGet: https://www.nuget.org/packages/Plugin.VoiceToText
- Install into your platform-specific projects (iOS/Android), and any .NET Standard 2.0 projects required for your app.
  
## Platform Support

|Platform|Supported|Version|Notes|
| ------------------- | :-----------: | :------------------: | :------------------: |
|Xamarin.iOS|Yes|iOS 11+| |
|Xamarin.Android|Yes|API 16+|Project should [target Android framework 9.0+](https://docs.microsoft.com/en-us/xamarin/android/app-fundamentals/android-api-levels?tabs=vswin#framework)|    

## For Android
Add following into MainActivity class.
```csharp
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity,IVoiceMessageSender
    {
        private readonly int VOICE = 10; // This must be 10 because this plugin registered as 10 
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            VoiceToText.Init(this);  // Need this to init
            LoadApplication(new App());
        }
        
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {

            if (requestCode == VOICE)
            {
                if (resultCode == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        var textInput = matches[0];
                        MessagingCenter.Send<IVoiceMessageSender, string>(this, "STT", textInput);
                    }
                    else
                    {
                        MessagingCenter.Send<IVoiceMessageSender, string>(this, "STT", "No input");
                    }

                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }
    }
```

## For iOS
Add following into info.plist
```csharp
	<key>NSSpeechRecognitionUsageDescription</key>
	<string>Allows you create tasks</string>
	<key>NSMicrophoneUsageDescription</key>
	<string>Allows you to recognise your voice to create tasks</string>
```

## Usage

Check MainPage.xaml.cs in Sample application to see how to setup and use.

# Limitations

Only support Android and iOS for the moment. 

# Contributing

Contributions are welcome.  Feel free to file issues and pull requests on the repo and they'll be reviewed as time permits.

# License
Under MIT, see LICENSE file.
