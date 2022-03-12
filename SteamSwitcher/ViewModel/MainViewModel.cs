using System;
using System.Collections.Generic;
using System.Linq;
using SteamSwitcher.Sevices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm;
using SteamSwitcher.View;
using System.IO;
using SteamSwitcher.Model;

namespace SteamSwitcher.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        public Page CurrentPage { get; set; }
        public string SteamInfo { get; set; }

        public MainViewModel()
        {

            Navigation.ChangePage = (p) => CurrentPage = p;
            AccountDataStorage.Init();
            SteamInfo =  "Steam Status: " + SteamProvider.SteamStatus() + "     Steam Location: " + SteamProvider.location + "      Steam " + (SteamProvider.SteamRunning() ? "" : "not ") + "Running";

            if (AccountDataStorage.ReadAccounts().Count == 0)
                Navigation.ChangePage(StaticData.LoginP);
            else
                Navigation.ChangePage(StaticData.ListP);
        }
    }
}
