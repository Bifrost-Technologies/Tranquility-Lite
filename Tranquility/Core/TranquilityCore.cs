using Solnet.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using LinkStream.Server;
using System.Diagnostics;
using Solnet.Metaplex.NFT.Library;

namespace Tranquility.Core
{
    public static class Runtime
    {
        public static Storage.SolanaVault SolanaVault { get; set; }
        public static int SelectedNavItem { get; set; }
        public static bool OverviewInitialized { get; set; }

        public static string WalletRPCprovider = "https://autumn-tame-field.solana-mainnet.quiknode.pro/af19ce3e33a293839ae3faf65e4989d05d95ebba";

        public static List<MetadataAccount> Inventory = new List<MetadataAccount>();
        public static Dictionary<string, string> Tokens = new Dictionary<string, string>();
        public static TokenWallet TokenWallet { get; set; }
        public static bool SuccessfullyLoaded { get; set; }
        public static bool SuccessfullyLoadedAV { get; set; }
        public static bool SuccessfullyLoadedWI { get; set; }
        public static TokenMintResolver TokenMintDatabase { get; set; }
        public static int SelectedAccount { get; set; }
        public static string ActiveTransactionMessage { get; set; }
        public static LinkNetwork LinkNetwork { get; set; }
        public static bool IsLinkEnabled { get; set; }
        public static bool IsMonitoring { get; set; }

        public static async Task linkNetworkAI()
        {
            IsMonitoring= true;
            IsLinkEnabled = false;
            while (IsMonitoring)
            {
                Debug.WriteLine("Monitoring...");
                if (IsLinkEnabled)
                {
                    if(LinkNetwork != null)
                    {
                        Debug.WriteLine("Currently Listening for Dapp TXs...");
                        LinkNetwork.EncryptionEnabled = false;
                        await LinkNetwork.LinkStream();
                        Debug.WriteLine("Stopped Listening!");
                    }
                }
                await Task.Delay(6000);
            }
        }
        public static void CutLink()
        {
            Runtime.IsLinkEnabled = false;
            Runtime.LinkNetwork.IsOnline = false;
        }
        public static void SaveRPCProvider(string value)
        {
            try
            {
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values["RPC_provider"] = value;
            }
            catch
            {

            }
        }
        public static void LoadRPCProvider()
        {
            try
            {
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                String rpc = localSettings.Values["RPC_provider"] as string;
                if (rpc != null && rpc != string.Empty)
                {
                    WalletRPCprovider = rpc;
                }
                else
                {
                    WalletRPCprovider = "https://autumn-tame-field.solana-mainnet.quiknode.pro/af19ce3e33a293839ae3faf65e4989d05d95ebba";
                }
            }
            catch
            {

            }
        }
    }
 
}
