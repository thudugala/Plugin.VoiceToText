﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.VoiceToText 
{
    public interface IVoiceToText
    {
        void StartVoiceToText();
        void StopVoiceToText();
    }
}