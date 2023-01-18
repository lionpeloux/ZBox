using Geolocation;
using Grasshopper.Kernel;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using ZymeToolbox.Core.API.DOE;

namespace ZymeToolbox.Grasshopper.Components.IO
{
    public class GHComp_DownloadFile : GH_Component
    {

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{E20704A3-E038-41F5-8DF3-34C0AA4BED4F}");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public GHComp_DownloadFile()
          : base("Download File", "Download", "",
            "ZBox", "1 | I/O")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Url To File", "url", "A valid url pointing to a file hosted on a web server.", GH_ParamAccess.item);
            pManager.AddTextParameter("Directory Path", "dirPath", "A path to a directory where the file will be downloaded", GH_ParamAccess.item);
            pManager.AddTextParameter("New File/Directroy Name", "rename", "Optional file name to rename the downloaded file. For .zip file will rename the extract folder.", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Overwrite", "overwrite", "If true, overwrites an existing file with the same name. If false, silently skips the download", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Extract ZIP", "unzip", "If true, when the url points to a .zip file, it will be extracted in a subdirectory.", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Run", "run", "The component will run only if set to true.", GH_ParamAccess.item);

            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Files", "files", "A list of all files in the directory.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            string url = "";
            if (!DA.GetData(0, ref url)) return;
            
            string dirPath = "";
            if (!DA.GetData(1, ref dirPath)) return;
            if (!Directory.Exists(dirPath)) throw new ArgumentException($"The directory specified does not exist : {dirPath}");

            bool run = false;
            if (!DA.GetData("Run", ref run)) return;
            if (!run)
            {
                DA.SetDataList(0, Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories));
                return;
            }

            string reName = "";
            DA.GetData(2, ref reName);

            bool overwrite = true;
            DA.GetData(3, ref overwrite);

            bool unzip = true;
            DA.GetData(4, ref unzip);


            var uri = new Uri(url);

            var fileNameWithExtension = uri.Segments.Last();
            var fileExtension = Path.GetExtension(fileNameWithExtension);
            var fileName = Path.GetFileNameWithoutExtension(fileNameWithExtension);
            var filePath = reName != "" ? dirPath + reName + fileExtension : dirPath + fileNameWithExtension;

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                if (fileExtension != ".zip" || unzip == false)
                {
                    if (!(overwrite == false && File.Exists(filePath)))
                    {
                        File.WriteAllText(filePath, response.Content.ReadAsStringAsync().Result);
                    }
                }
                else
                {
                    // its a zip and unzip == true
                    var subDirPath = reName != "" ? Path.Combine(dirPath, reName) : Path.Combine(dirPath, fileName);
                    if (Directory.Exists(subDirPath))
                    {
                        if (overwrite == false)
                        {
                            DA.SetDataList(0, Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories));
                            return;
                        }
                        else Directory.Delete(subDirPath, true); // warning
                    }

                    Directory.CreateDirectory(subDirPath);
                    using (var archive = new ZipArchive(response.Content.ReadAsStreamAsync().Result))
                    {
                        archive.ExtractToDirectory(subDirPath);
                    }
                }
            }

            DA.SetDataList(0, Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories));

        }

    }
}