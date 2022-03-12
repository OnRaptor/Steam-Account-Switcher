using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm;
using SteamSwitcher.Sevices;
using SteamSwitcher.Model;
using System.Windows;

namespace SteamSwitcher.ViewModel
{
    class ListViewModel : ViewModelBase
    {
        public List<Account> Accounts {get; set;}
        public int? SelectedIndex { get; set; }

        public ICommand AddAccount
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Navigation.ChangePage(StaticData.LoginP);
                });
            }
        }
        public ICommand DeleteAccount
        {
            get
            {
                return new DelegateCommand<string>((id) =>
                {
                    Accounts = AccountDataStorage.DeleteAccount(int.Parse(id));                           
                });
            }
        }

        public ICommand Import
        {
            get
            {
                return new DelegateCommand<string>(async (id) =>
                {
                    var acc = AccountDataStorage.GetAccountByIndex(int.Parse(id));
                    if (string.IsNullOrEmpty(acc.SteamID64))
                    {
                        MessageBox.Show("Must have Steam Url");
                        return;
                    }
                    var accs = AccountDataStorage.ReadAccounts();
                    accs.RemoveAll(i => i.ID == int.Parse(id));
                    Navigation.ChangePage(StaticData.ImportP);
                    StaticData.ImportP.DataContext = new ImportViewModel(await SteamProvider.GetGamesInFolder(acc.SteamID64), accs, acc.SteamID64);  
                });
            }
        }
        public ICommand Edit
        {
            get
            {
                return new DelegateCommand<string>((id) =>
                {
                    var acc = AccountDataStorage.GetAccountByIndex(int.Parse(id));
                    LoginPageViewModel vm = new LoginPageViewModel();
                    vm.Importdata(acc, int.Parse(id));
                    StaticData.LoginP.DataContext = vm;
                    Navigation.ChangePage(StaticData.LoginP);          
                });
            }
        }

        public AsyncCommand Login
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                if (SelectedIndex != null)
                       await Task.Run(() => SteamProvider.Login(Accounts[SelectedIndex.Value].login, Accounts[SelectedIndex.Value].password));
                });
            }
        }
        public ListViewModel()
        {
            Accounts = AccountDataStorage.ReadAccounts();
        }
    }
}
