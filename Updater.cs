using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Int64;

namespace GenericUpdater.SDK
{
    public sealed class Updater
    {
        private readonly WebClient _webClient;
        private readonly string _configUrl;
        private readonly string _updatesPath = Path.Combine(Directory.GetCurrentDirectory(), "Software Updates");
        public string UpdateLocalFileName { get; private set; }
        public ConfigFile ConfigFile { get; set; }

        public event DownloadProgressChangedEventHandler DownloadProgressChanged;
        public event AsyncCompletedEventHandler DownloadCompleted;
        /// <summary>
        ///  Initialize Updater
        /// </summary>
        /// <param name="configFileUrl">Config file Url
        /// Make sure this is the .json file download link
        /// </param>
        public Updater(string configFileUrl)
        {
            _configUrl = configFileUrl;
            _webClient = new WebClient();
        }

        public async Task<bool> CheckUpdate(string actualVersion)
        {
            try
            {
                if (!Utils.IsConnectedToInternet())
                    throw new Exception("Error connecting to server, please check your internet connection");
                // get info from Url
               
                var jsonText = await GetJsonConfigFromUrl();
                ConfigFile = GenerateConfigFromJsonFile(jsonText);

                UpdateLocalFileName = Path.Combine(_updatesPath + "//" + ConfigFile?.FileName);
                // comparing versions
                var actVersion = new Version(actualVersion);
                var serverVersion = new Version(ConfigFile.Version);
                return serverVersion > actVersion;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool UpdateJustDownloaded()
        {
            try
            {
                Console.WriteLine(UpdateLocalFileName);
                if (!File.Exists(UpdateLocalFileName)) return false;
                var info = new FileInfo(UpdateLocalFileName);
                var vInfo = FileVersionInfo.GetVersionInfo(UpdateLocalFileName);
                var vLocal = new Version(vInfo.ProductVersion);
                var v1 = new Version(ConfigFile.Version);
                Console.WriteLine(info.Length);
                Console.WriteLine(info.Length);
                Console.WriteLine(ConfigFile.FileSize);
                Console.WriteLine(v1 == vLocal);
                return v1 == vLocal && Math.Abs(info.Length - ConfigFile.FileSize) < 1;
            }
            catch (Exception e)
            { 
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// get JSON Text From  Cloud File
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<string> GetJsonConfigFromUrl()
        {
            try
            {
                var request = WebRequest.Create(_configUrl);
                var reply = await request.GetResponseAsync();
                var returning = new StreamReader(reply.GetResponseStream() ?? new MemoryStream());
                return await returning.ReadToEndAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Serialize a Config File to Json
        /// </summary>
        /// <param name="configFile"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GenerateJsonFromConfig(ConfigFile configFile)
        {
            try
            {
                return JsonConvert.SerializeObject(configFile);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///  Deserialize a Config File From a Json Text
        /// </summary>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ConfigFile GenerateConfigFromJsonFile(string jsonText)
        {
            try
            {
                return JsonConvert.DeserializeObject<ConfigFile>(jsonText);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        ///  Cancel async download operation
        /// </summary>
        public void CancelDownloadOperation()
        {
            _webClient?.CancelAsync();
        }

        /// <summary>
        ///  install the app when download is finished
        /// </summary>
        /// <param name="current">Application process to be killed</param>
        public void InstallUpdate(Process current = null)
        {
            try
            {
                if (!File.Exists(UpdateLocalFileName)) return;
                Process.Start(UpdateLocalFileName);
                if (current == null) return;

                // kill all application process instances
                Process.GetProcessesByName(current.ProcessName)
                    .Where(t => t.Id != current.Id)
                    .ToList()
                    .ForEach(t => t.Kill());

                current.Kill();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Download de updater File
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void DownloadFile()
        {
            try
            {
                // handling download progress event
                _webClient.DownloadProgressChanged += (sender, args) => OnDownloadProgressChanged(args);
                _webClient.DownloadFileCompleted += (sender, args) => OnDownloadCompleted(args);
                // download file
                if (!Directory.Exists(_updatesPath))
                    Directory.CreateDirectory(_updatesPath);
              
                _webClient.DownloadFileAsync(new Uri(ConfigFile.FileUrl),
                    UpdateLocalFileName);
            }
            catch (UriFormatException)
            {
                throw new Exception("The file download address is not in the correct format");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }

        private void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(this, e);
        }

        private void OnDownloadCompleted(AsyncCompletedEventArgs e)
        {
            DownloadCompleted?.Invoke(this, e);
        }
    }
}