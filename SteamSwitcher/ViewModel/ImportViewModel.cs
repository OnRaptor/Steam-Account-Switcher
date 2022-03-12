using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using DevExpress.Mvvm;
using SteamSwitcher.Model;
using SteamSwitcher.Sevices;

namespace SteamSwitcher.ViewModel
{
    class ImportViewModel : ViewModelBase
    {
        public List<Account> Accounts { get; set; }
        public List<Game> Games { get; set; }
        public Account SelectedAccount { get; set; }
        public string OriginID { get; set; }

        public ICommand Import
        {
            get
            {
                return new DelegateCommand<object>((games) =>
                {
                    if (games == null && SelectedAccount == null)
                        return;

                    System.Collections.IList items = (System.Collections.IList)games;
                    var collec = items.Cast<Game>();
                    string dest = AccountDataStorage.GetAccountByIndex(SelectedAccount.ID).SteamID64;
                    SteamProvider.ExportSetting(OriginID, dest, collec.ToList());
                    Navigation.ChangePage(StaticData.ListP);
                });
            }
        }
        public ICommand OpenUrl
        {
            get
            {
                return new DelegateCommand<string>((uri) =>
                {
                    Process.Start(uri);
                });
            }
        }

        public ICommand Back
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Navigation.ChangePage(StaticData.ListP);
                });
            }
        }

        public ImportViewModel(List<Game> Games, List<Account> Accounts, string orginID)
        {
            this.Games = Games;
            this.Accounts = Accounts;
            this.OriginID = orginID;
        }
        public ImportViewModel()
        {
        }
    }
}
