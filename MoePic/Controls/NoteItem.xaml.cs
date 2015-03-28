using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MoePic.Models;

namespace MoePic.Controls
{
    public partial class NoteItem : UserControl
    {
        public NoteItem()
        {
            InitializeComponent();
        }

        public MoeNote Note { get; set; }

        private void LayoutRoot_Click(object sender, RoutedEventArgs e)
        {
            if(Click != null)
            {
                Click(this, e);
            }
        }

        public event RoutedEventHandler Click;

    }

   


}
