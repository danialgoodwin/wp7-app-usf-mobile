using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace MySchoolApp
{
    public class Settings
    {
        public string Name { get; set; }

        public string NewsUrl { get; set; }

        public string BingMapsKey { get; set; }

        public SolidColorBrush ThemeColor1 { get; set; }

        public SolidColorBrush ThemeColor2 { get; set; }

        public string NameToUpper
        {
            get { return Name.ToUpper(); }
        }
    }
}