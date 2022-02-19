using Newtonsoft.Json;

namespace GenericUpdater.SDK
{
    public  class ConfigFile
    {
        public string Version { get; set; }
        public string FileUrl { get; set; }
        public double FileSize { get; set; }

        public bool Critical { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}