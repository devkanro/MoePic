﻿#pragma checksum "C:\Users\HIGAN\documents\visual studio 2013\Projects\MoePic\MoePic\ImageViewPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1943A9757EA49731DB790A8768999093"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.34014
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Shell;
using MoePic.Models;
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


namespace MoePic {
    
    
    public partial class ImageViewPage : MoePic.Models.MoePicPage {
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton addFav;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.ProgressBar downProgress2;
        
        internal System.Windows.Controls.ProgressBar downProgress;
        
        internal System.Windows.Controls.TextBlock failText;
        
        internal System.Windows.Controls.Primitives.ViewportControl viewport;
        
        internal System.Windows.Controls.Canvas canvas;
        
        internal System.Windows.Controls.Image TestImage;
        
        internal System.Windows.Media.ScaleTransform xform;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/MoePic;component/ImageViewPage.xaml", System.UriKind.Relative));
            this.addFav = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("addFav")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.downProgress2 = ((System.Windows.Controls.ProgressBar)(this.FindName("downProgress2")));
            this.downProgress = ((System.Windows.Controls.ProgressBar)(this.FindName("downProgress")));
            this.failText = ((System.Windows.Controls.TextBlock)(this.FindName("failText")));
            this.viewport = ((System.Windows.Controls.Primitives.ViewportControl)(this.FindName("viewport")));
            this.canvas = ((System.Windows.Controls.Canvas)(this.FindName("canvas")));
            this.TestImage = ((System.Windows.Controls.Image)(this.FindName("TestImage")));
            this.xform = ((System.Windows.Media.ScaleTransform)(this.FindName("xform")));
        }
    }
}
