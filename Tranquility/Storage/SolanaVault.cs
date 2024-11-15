﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Tranquility.Storage
{
    public class SolanaVault
    {  
        public IBuffer Wallet { get; set; }

        public IBuffer ProtectedSessionKey { get; set; }
        public IBuffer HashedProtectedKey { get; set; }

        public List<IBuffer> AccountStorage = new List<IBuffer>();
        public List<Wallets.ActiveAccount> ActiveAccounts = new List<Wallets.ActiveAccount>();

        public List<int> WalletIndexChart = new List<int>();
        public SolanaVault()
        {
            LoadVaultKey();
            LoadWalletStorage();
            LoadAccountStorage();
            LoadWalletIndexChart();
        }
        public async void LoadVaultKey()
        {
            var folder = ApplicationData.Current.LocalFolder;
            try
            {
                var file = await folder.GetFileAsync("vault.key");
                if (file != null)
                {
                    HashedProtectedKey = Convert.FromBase64String(await FileIO.ReadTextAsync(file)).AsBuffer();
                    Core.Runtime.SuccessfullyLoaded = true;
                }
                   
            }
            catch
            {
                Core.Runtime.SuccessfullyLoaded = false;
            }
            await Task.Delay(50);
        }

        public async void LoadWalletStorage()
        {
            var folder = ApplicationData.Current.LocalFolder;
            try
            {
                var file = await folder.GetFileAsync("solana.vault");
                if (file != null)
                {
                    Wallet = Convert.FromBase64String(await FileIO.ReadTextAsync(file)).AsBuffer();
                    Core.Runtime.SuccessfullyLoaded = true;
                }
                   
            }catch
            {
                Core.Runtime.SuccessfullyLoaded = false;
            }
            await Task.Delay(50);
        }
        public async void LoadAccountStorage()
        {
            var folder = ApplicationData.Current.LocalFolder;
            try
            {
                var file = await folder.GetFileAsync("solano.vault");
                if (file != null)
                {
                    AccountStorage = new List<IBuffer>();
                    var rawVault = await FileIO.ReadLinesAsync(file);
                    foreach (var line in rawVault)
                    {
                        AccountStorage.Add(Convert.FromBase64String(line).AsBuffer());
                    }
                    Core.Runtime.SuccessfullyLoadedAV = true;
                }

            }
            catch
            {
                Core.Runtime.SuccessfullyLoaded = false;
            }
            await Task.Delay(50);
        }
        public async void LoadWalletIndexChart()
        {
            var folder = ApplicationData.Current.LocalFolder;
            try
            {
                var file = await folder.GetFileAsync("wallet.index");
                if (file != null)
                {
                    WalletIndexChart = new List<int>();
                    var rawVault = await FileIO.ReadLinesAsync(file);
                    foreach(var line in rawVault)
                    {
                      WalletIndexChart.Add(Convert.ToInt32(line));
                    }
                    Core.Runtime.SuccessfullyLoadedWI = true;
                }

            }
            catch
            {
                Core.Runtime.SuccessfullyLoadedWI = false;
            }
            await Task.Delay(50);
        }
        public async void SaveVaultKey()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                StorageFile file = null;
                file = await folder.CreateFileAsync("vault.key", CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteTextAsync(file, Convert.ToBase64String(HashedProtectedKey.ToArray()));
            }
            catch
            {

            }
        }
        public async void SaveWalletStorage()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                StorageFile file = null;
                file = await folder.CreateFileAsync("solana.vault", CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteTextAsync(file, Convert.ToBase64String(Wallet.ToArray()));
            }
            catch
            {

            }
        }
        public async void SaveAccountStorage()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                StorageFile file = null;

                var _AV = new List<string>();
                foreach (var acc in AccountStorage)
                {
                    _AV.Add(Convert.ToBase64String(acc.ToArray()));
                }
                file = await folder.CreateFileAsync("solano.vault", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteLinesAsync(file, _AV);
            }
            catch
            {

            }
        }
        public async void SaveWalletIndex()
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                StorageFile file = null;

                var _WI = new List<string>();
                foreach (var index in WalletIndexChart)
                {
                    _WI.Add(index.ToString());
                }
                file = await folder.CreateFileAsync("wallet.index", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteLinesAsync(file, _WI);
            }
            catch
            {

            }
        }

    }
}
