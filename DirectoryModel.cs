using System.Collections.Generic;

namespace Nebula.CI.Plugins.Common.Result.Controllers
{
    public class DirectoryModel
    {
        public string Name { get; set; }
        public bool IsFile { get; set; }
        public string Path { get; set; }
        public string DownloadPath { get; set; }
        public List<DirectoryModel> Children { get; set; }
    }
}