using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using _9Contatos.Store;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace _9Contatos.Interface
{
    public sealed partial class OptionBox_Donate : ContentDialog
    {
        public OptionBox_Donate()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void bt_donate01(object sender, TappedRoutedEventArgs e)
        {
            Store.Comprar.ComprarDoacao("DONATE01");
        }

        private void bt_donate02(object sender, TappedRoutedEventArgs e)
        {
            Store.Comprar.ComprarDoacao("DONATE02");

        }

        private void bt_donate03(object sender, TappedRoutedEventArgs e)
        {
            Store.Comprar.ComprarDoacao("DONATE03");
        }

        private void bt_donate04(object sender, TappedRoutedEventArgs e)
        {
            Store.Comprar.ComprarDoacao("DONATE04");
        }

        private void bt_donate05(object sender, TappedRoutedEventArgs e)
        {
            Store.Comprar.ComprarDoacao("DONATE05");
        }
    }
}
