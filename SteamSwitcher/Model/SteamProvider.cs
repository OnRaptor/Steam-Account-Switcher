using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
using System.Net;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.IO;

namespace SteamSwitcher.Model
{
    public struct Game
    {
        public string id { get; set; }
        public string link { get; set; }
    }
    class SteamProvider
    {
        private static readonly string SteamPath_x64 = @"SOFTWARE\Wow6432Node\Valve\Steam";
        private static readonly string SteamPath_x32 = @"Software\Valve\Steam";
        public static string location = GetLocationSteam();
        

        public static string SteamStatus()
        {
            try
            {
                WebRequest.Create("https://steamcommunity.com/").GetResponse();
                return "OK";
            }
            catch (Exception ex)
            {
                return "NoConnection: " + ex.Message;
            }
        }

        public static bool Login(string login, string pass)
        {
            if (SteamRunning())
                LogOut();

            System.Threading.Thread.Sleep(3000);
            Process steam_process = new Process() { 
            StartInfo = new ProcessStartInfo()
            {
                FileName = location + "\\steam.exe",
                Arguments = $"-login {login} {pass}"
            }
            };
            return steam_process.Start();
        }
        public static bool SteamRunning()
        {
            //true - running, false not
            return Process.GetProcessesByName("steam").Any();
        }

        public static string GetLocationSteam(string Inst = "InstallPath", string Source = "SourceModInstallPath")
        {
            using (var BaseSteam = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32)))
            {
                using (RegistryKey Key = BaseSteam.OpenSubKey(SteamPath_x64, Environment.Is64BitOperatingSystem))
                {
                    using (RegistryKey Key2 = BaseSteam.OpenSubKey(SteamPath_x32, Environment.Is64BitOperatingSystem))
                    {
                        return Key?.GetValue(Inst)?.ToString() ?? Key2?.GetValue(Source)?.ToString();
                    }
                }
            }
        }
        static void LogOut()
        {
            Process.Start(new ProcessStartInfo() { CreateNoWindow=true,
                FileName = "cmd.exe",
                Arguments = "/c taskkill /im steam.exe /f", WindowStyle = ProcessWindowStyle.Hidden
            });
        }
        private static string ConvertSteamId(string steamID64)
        {
            long communityId = long.Parse(steamID64);
            if (communityId < 76561197960265729L || !Regex.IsMatch(communityId.ToString((IFormatProvider)System.Globalization.CultureInfo.InvariantCulture), "^7656119([0-9]{10})$"))
            {
                return string.Empty;
            }
            return string.Format("{0}", communityId - 76561197960265728L);
        }

        public async static  Task<List<Game>> GetGamesInFolder(string SteamID64)
        {
            try
            {
                List<Game> result = new List<Game>();
                await Task.Run(() =>
                {
                    var steamId32 = ConvertSteamId(SteamID64);
                    var ids = Directory.GetDirectories($"{location}\\userdata\\{steamId32}").Select(s => s.Split('\\').Last()).ToArray();
                    
                    foreach (var item in ids)
                        result.Add(new Game { id = item, link = "https://store.steampowered.com/app/" + item});
                    return result;
                });
                return result;
             }
            catch { return null; }
        }
        static void CopyDirectory(string begin_dir, string end_dir)
        {
            //Берём нашу исходную папку
            DirectoryInfo dir_inf = new DirectoryInfo(begin_dir);
            //Перебираем все внутренние папки
            foreach (DirectoryInfo dir in dir_inf.GetDirectories())
            {
                //Проверяем - если директории не существует, то создаём;
                if (Directory.Exists(end_dir + "\\" + dir.Name) != true)
                {
                    Directory.CreateDirectory(end_dir + "\\" + dir.Name);
                }

                //Рекурсия (перебираем вложенные папки и делаем для них то-же самое).
                CopyDirectory(dir.FullName, end_dir + "\\" + dir.Name);
            }

            //Перебираем файлики в папке источнике.
            foreach (string file in Directory.GetFiles(begin_dir))
            {
                //Определяем (отделяем) имя файла с расширением - без пути (но с слешем "\").
                string filik = file.Substring(file.LastIndexOf('\\'), file.Length - file.LastIndexOf('\\'));
                //Копируем файлик с перезаписью из источника в приёмник.
                File.Copy(file, end_dir + "\\" + filik, true);
            }
        }

        public static void ExportSetting(string origin, string dest, List<Game> games)
        {
            origin = ConvertSteamId(origin.ToString());
            dest = ConvertSteamId(dest.ToString());

            string path = $"{location}\\userdata\\{dest}\\";
            string path_origin = $"{location}\\userdata\\{origin}\\";
            //Delete folders in dest and copy folders from origin acc to dest
            foreach (var game in games)
            {
                if (Directory.Exists(path + game.id))
                {
                    Directory.Delete(path + game.id, true);
                    Directory.CreateDirectory(path + game.id);
                }
                    CopyDirectory(path_origin + game.id, path + game.id);
                
            }
        }
    }
}
