using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamSwitcher.Model;
using System.Text.Json;
using System.IO;

namespace SteamSwitcher.Sevices
{
    class AccountDataStorage
    {
        static string pathToSave = AssemblyDirectory + "\\accounts.json";

        public static void Init()
        {
            if (!File.Exists(pathToSave))
            {
                File.WriteAllBytes(pathToSave, Encoding.UTF8.GetBytes("[]"));
            }
        }
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        public static List<Account> ReadAccounts()
        {
            try
            {
                return JsonSerializer.Deserialize<List<Account>>(File.ReadAllBytes(pathToSave));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error reading account data\n" + ex.Message);
                return null;
            }
        }
        public static void AddAccount(Account acc)
        {

            var readed = ReadAccounts();
            acc.ID = readed.Count != 0 ? readed.Last().ID + 1 : 0;
            readed.Add(acc);
            File.WriteAllText(pathToSave, JsonSerializer.Serialize(readed));

        }
        public static void SaveAccounts(List<Account> accs)
        {
            File.WriteAllBytes(pathToSave, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(accs)));
        }
        public static Account GetAccountByIndex(int index)
        {
            return ReadAccounts().Find(i => i.ID == index);
        }
        public static void ReplaceAtIndex(int index, Account acc)
        {
            var readed = ReadAccounts();
            readed[index] = acc;
            SaveAccounts(readed);
        }

        public static List<Account> DeleteAccount(int index)
        {
            var readed = ReadAccounts();
            readed.RemoveAll(i => i.ID == index);
            SaveAccounts(readed);
            return ReadAccounts();
        }

    }
}
