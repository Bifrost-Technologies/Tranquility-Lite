// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using LinkStream.Server;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tranquility.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Tranquility
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            mWindow = new MainWindow();
            Frame rootFrame = mWindow.Content as Frame;
            
            if (rootFrame == null)
            {
                rootFrame = new Frame();

                mWindow.Content = rootFrame;
            }
            Storage.SolanaVault _solanaVault = new Storage.SolanaVault();
            Core.Runtime.LoadRPCProvider();
            if (Core.Runtime.WalletRPCprovider == null | Core.Runtime.WalletRPCprovider == String.Empty)
            {
                Core.Runtime.WalletRPCprovider = "https://autumn-tame-field.solana-mainnet.quiknode.pro/af19ce3e33a293839ae3faf65e4989d05d95ebba";

            }
            Core.Runtime.SolanaVault = _solanaVault;
       
            await Task.Delay(50);
            Core.Runtime.TokenMintDatabase = await Solnet.Extensions.TokenMintResolver.LoadAsync();


            if (Core.Runtime.SolanaVault.AccountStorage == null)
            {
                Core.Runtime.SolanaVault.AccountStorage = new List<Windows.Storage.Streams.IBuffer>();
            }
            if (Core.Runtime.SolanaVault.WalletIndexChart == null)
            {
                Core.Runtime.SolanaVault.WalletIndexChart = new List<int>();
                Core.Runtime.SolanaVault.WalletIndexChart.Add(0);
            }

            Core.Runtime.LinkNetwork = new LinkNetwork(50505, "127.0.0.1", "Tranquility");
            Core.Runtime.LinkNetwork.SignatureRequestEvent += HandleRequestEvent;
            Core.Runtime.LinkNetwork.IsOnline = true;
           

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(Views.MainPage), args.Arguments);
            }
            mWindow.Activate();

            await Core.Runtime.linkNetworkAI();
        }
        public Window mWindow;
        void HandleRequestEvent(object sender, SignRequestEventArgs e)
        {
            Debug.WriteLine("Reached Successfully!");
            Core.Runtime.ActiveTransactionMessage = e.Message;
            Frame rootFrame = mWindow.Content as Frame;
            rootFrame.Navigate(typeof(Approve));

        }
    }
}
