using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Reflection;
using GenericUpdater.SDK;

namespace GenericUpdaterDemo
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
          
            var updater = new Updater("https://www.dropbox.com/s/k304ioy6st2kq6j/SapeUpdater.json?dl=1");
            var hasUpdate = updater.CheckUpdate("2022.0.12.0").Result;

            if (!hasUpdate) return;
            Process.Start("UpdaterSample.exe", "Julio jose de andrade reis");
            updater.DownloadProgressChanged += (sender, eventArgs) =>
            {
                Console.WriteLine(
                    $@"{Utils.ToKnownTransferUnit(eventArgs.BytesReceived)}/{Utils.ToKnownTransferUnit(updater.ConfigFile.FileSize)}");
            };
            Console.WriteLine(updater.ConfigFile.ToString());
            updater.DownloadFile();

            Console.ReadLine();
        }
    }
}