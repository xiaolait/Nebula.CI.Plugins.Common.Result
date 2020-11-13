using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Nebula.CI.Plugins.Common.Result.Controllers
{
    [ApiController]
    [Route("api/ci/plugins/common/result/catalog")]
    public class FileController : ControllerBase
    {
        [HttpGet("{pipelineHistoryId}")]
        public ActionResult<List<DirectoryModel>> Get(string pipelineHistoryId)
        {
            var directory = GetDirectory(pipelineHistoryId);
            if (directory == null) return NotFound();
            var basePath = $"/api/ci/plugins/common/result/filebrowser/{directory.Name}";
            var baseDownloadPath = $"/api/ci/plugins/common/result/filedownload/{directory.Name}";
            var catalog = GetCatalog(directory, basePath, baseDownloadPath);
            return catalog;
        }

        private DirectoryInfo GetDirectory(string id )
        {
            var nfsdir = new DirectoryInfo("/nfs");
            var dirList = nfsdir.GetDirectories();
            foreach (var dir in dirList)
            {
                Console.WriteLine(dir.Name);
                if(dir.Name.StartsWith($"ci-nebula-{id}-pvc"))
                {
                    return dir;
                }
            }
            return null;
        }

        private List<DirectoryModel> GetCatalog(DirectoryInfo directory, string basePath, string baseDownloadPath)
        {
            var catalog = new List<DirectoryModel>();
            var directories = directory.GetDirectories();
            foreach(var subDirectory in directories)
            {
                var directoryModel = new DirectoryModel()
                {
                    Name = subDirectory.Name,
                    IsFile = false,
                };
                directoryModel.Path = $"{basePath}/{directoryModel.Name}";
                directoryModel.DownloadPath = $"{baseDownloadPath}/{directoryModel.Name}";
                directoryModel.Children = GetCatalog(subDirectory, directoryModel.Path, directoryModel.DownloadPath);
                catalog.Add(directoryModel);
            }
            var files = directory.GetFiles();
            foreach(var file in files)
            {
                var directoryModel = new DirectoryModel()
                {
                    Name = file.Name,
                    IsFile = true,
                    Path = $"{basePath}/{file.Name}",
                    DownloadPath = $"{baseDownloadPath}/{file.Name}"
                };        
                catalog.Add(directoryModel);       
            }
            return catalog;
        }
    }
}
