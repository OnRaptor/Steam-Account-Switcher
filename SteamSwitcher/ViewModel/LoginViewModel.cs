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
using SteamSwitcher.Sevices;
using SteamSwitcher.Model;

namespace SteamSwitcher.ViewModel
{
    class LoginPageViewModel : ViewModelBase
    {
        public string LoginText { get; set; }
        public string PassText { get; set; }
        public string UrlText { get; set; }
        public string TagText { get; set; }
        int? index = null;

        public AsyncCommand Login
        {
            get
            {
                return new AsyncCommand(async () =>
                {
                    Model.Account acc = new Account(LoginText, PassText);
                    Navigation.ChangePage(StaticData.ListP);
                    //async parsing avatar and screenName from steam url
                    await Task.Run(() =>
                    {
                        if (!string.IsNullOrEmpty(UrlText))
                        {
                            acc.SteamUrl = UrlText;
                            try
                            {
                                XmlDocument doc = new XmlDocument();
                                if (UrlText.Split('\\').Last().Contains("xml"))
                                    doc.Load(UrlText);
                                else
                                    doc.Load(UrlText + "?xml=1");

                                XmlElement xRoot = doc.DocumentElement;
                                if (xRoot != null)
                                {
                                    // обход всех узлов в корневом элементе
                                    foreach (XmlElement xnode in xRoot)
                                    {
                                        if (xnode.Name == "steamID")
                                            acc.ViewName = xnode.InnerText;
                                        if (xnode.Name == "avatarFull")
                                            acc.ImageUrl = xnode.InnerText;
                                        if (xnode.Name == "steamID64")
                                            acc.SteamID64 = xnode.InnerText;
                                    }
                                }
                            }
                            catch (Exception ex) { MessageBox.Show("Error in request to Steam\n" + ex.Message + "\n" + ex.StackTrace); }
                        }
                        if (!string.IsNullOrEmpty(TagText))
                            acc.tag = TagText;

                        if (index != null)
                        {
                            AccountDataStorage.ReplaceAtIndex(index.Value, acc);
                            index = null;
                        }
                        else
                            AccountDataStorage.AddAccount(acc);

                        
                    });
                    StaticData.ListP.DataContext = new ListViewModel();

                });
            }
        }
        public LoginPageViewModel()
        {

        }

        public void Importdata(Model.Account acc, int Viewindex)
        {
            PassText = acc.password;
            LoginText = acc.login;
            UrlText = acc.SteamUrl;
            TagText = acc.tag;
            this.index = Viewindex;
        }
    }
}
