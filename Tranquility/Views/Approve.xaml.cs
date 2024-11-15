using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using LinkStream.Server;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Models;
using Solnet.Rpc;
using System.Diagnostics;

namespace Tranquility.Views
{
    public sealed partial class Approve : Page
    {
        
        public Approve()
        {
            this.InitializeComponent();
          

            ReceiveNavButton.BorderBrush = pageborder.BorderBrush;
            ReceiveNavButton.BorderThickness = new Thickness(1,1,1,1);
            this.Loaded += Receive_Loaded;
        }

        private void Receive_Loaded(object sender, RoutedEventArgs e)
        {
            string _decodedInstructions = PacketProcessor.DecodeTransactionMessage(Convert.FromBase64String(Core.Runtime.ActiveTransactionMessage));
            InstructionDisplayBlock.Text = _decodedInstructions;
            Console.WriteLine(_decodedInstructions);
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_SelectionChanged_1(object sender, RoutedEventArgs e)
        {

        }

        private async void Approvebutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Approvebutton.Visibility = Visibility.Collapsed;

                //Debug Signing Messages
                string _decodedInstructions = PacketProcessor.DecodeTransactionMessage(Convert.FromBase64String(Core.Runtime.ActiveTransactionMessage));
                Debug.WriteLine(_decodedInstructions);

                IRpcClient rpcClient = ClientFactory.GetClient(Core.Runtime.WalletRPCprovider);

                await Wallets.SolanaWallet.Init();

                byte[] signedMessage = Wallets.SolanaWallet.SignActiveRequest();
                string message = Convert.ToBase64String(signedMessage);
                Core.Runtime.LinkNetwork.SetOutboundMessage(message);
                Debug.WriteLine(message);

                MessageBlock.Text = message;
                MessageBlock.Visibility = Visibility.Visible;
                Wallets.SolanaWallet.DeallocateMemory();
                await Task.Delay(5000);
                Frame.Navigate(typeof(Overview));
            }
            catch (Exception error)
            {
                Debug.WriteLine(error);    
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
