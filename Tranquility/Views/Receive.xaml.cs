using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System;

namespace Tranquility.Views
{
    public sealed partial class Receive : Page
    {
        public Receive()
        {
            this.InitializeComponent();
            try
            {
                this.WalletAddressDisplayBlock.Text = Core.Runtime.SolanaVault.ActiveAccounts[Core.Runtime.SelectedAccount].Address;
            }
            catch
            {

            }
            ReceiveNavButton.BorderBrush = pageborder.BorderBrush;
            ReceiveNavButton.BorderThickness = new Thickness(1,1,1,1);
            this.Loaded += Receive_Loaded;
        }

        private void Receive_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_SelectionChanged_1(object sender, RoutedEventArgs e)
        {

        }

        private async void Copybutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataPackage dataPackage = new DataPackage();
                dataPackage.RequestedOperation = DataPackageOperation.Copy;
                dataPackage.SetText(Core.Runtime.SolanaVault.ActiveAccounts[Core.Runtime.SelectedAccount].Address);
                Clipboard.SetContent(dataPackage);
                MessageBlock.Text = "Wallet Address Copied Successfully!";
                MessageBlock.Visibility = Visibility.Visible;
                await Task.Delay(5000);
                MessageBlock.Visibility = Visibility.Collapsed;
            }
            catch (Exception error)
            {
                Debug.WriteLine(error);
                MessageBlock.Text = "Error occured retrieving your address";
                MessageBlock.Visibility = Visibility.Visible;
                await Task.Delay(10000);
                MessageBlock.Visibility = Visibility.Collapsed;
            }

        }

  
        private void ReceiveNavButton_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 2;
            Frame.Navigate(typeof(Receive));
        }

        private void OverviewNavButton_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 1;
            Frame.Navigate(typeof(Overview));
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Core.Runtime.SelectedNavItem = 5;
            Frame.Navigate(typeof(SettingsPage));
        }
    }
}
