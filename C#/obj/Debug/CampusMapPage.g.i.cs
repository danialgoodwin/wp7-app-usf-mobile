﻿#pragma checksum "C:\Users\Dan\Documents\Programming\Windows\WindowsPhone\Apps\USF News\C#\CampusMapPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DF0DA56D3747BA3F43ABFC11B84590D5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace MySchoolApp {
    
    
    public partial class CampusMapPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Controls.PhoneApplicationPage pageImage;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Image MyImage;
        
        internal System.Windows.Media.CompositeTransform transform;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/MySchoolApp;component/CampusMapPage.xaml", System.UriKind.Relative));
            this.pageImage = ((Microsoft.Phone.Controls.PhoneApplicationPage)(this.FindName("pageImage")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.MyImage = ((System.Windows.Controls.Image)(this.FindName("MyImage")));
            this.transform = ((System.Windows.Media.CompositeTransform)(this.FindName("transform")));
        }
    }
}
