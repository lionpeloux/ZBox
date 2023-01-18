using Geolocation;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using ZymeToolbox.Core.API.DOE;

namespace ZymeToolbox.Climat.Grasshopper.Components
{
    public class GHComp_DOE_WeatherDataIndex : GH_Component
    {
        private List<string> urls = new List<string>();
        private static List<WeatherData> Index = null;

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{6F001BE4-A6C9-469F-A6A9-E92564930BBE}");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public GHComp_DOE_WeatherDataIndex()
          : base("DOE Weather Data Index", "DOE Weather Data Index", 
                "Build an index of all available weather data from EnergyPlus.net. " +
                "This is achieved through parsing the input file(s). " +
                "Note that the index is built only once (unless urls parameter is changed) to allow reactive filtering",
            "ZBox", "2 | Climat API")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Url to .GeoJson File(s)", "urls", "Urls to the .geojson file(s) from EnregyPlus requiered to build the index. Url(s) can actually be path(s) to local file(s).", GH_ParamAccess.list, Services.WeatherDataIndexFiles);
            pManager.AddTextParameter("Filter Title", "filterTitle", "A string to filter the title (cas insensitive)", GH_ParamAccess.item, "");
            pManager.AddNumberParameter("Filter Location", "filterPtLatitude", "Latitude of the point to filter from.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Filter Location", "filterPtLongitude", "Longitude of the point to filter from.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Filter Radius in km", "filterRadius", "Selection radius to filter the outputs. No filter if <= 0.", GH_ParamAccess.item, 0.0);

            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Weather Data Title", "title", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("WMO Station ID", "stationID", "", GH_ParamAccess.list);
            pManager.AddTextParameter("Station Name", "stationName", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("WMO Region ID", "regionID", "", GH_ParamAccess.list);
            pManager.AddTextParameter("Country Code", "countryCode", "", GH_ParamAccess.list);
            pManager.AddTextParameter("Zone Code", "zoneCode", "", GH_ParamAccess.list);
            pManager.AddTextParameter("Data Source", "source", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Latitude", "latitude", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Longitude", "longitude", "", GH_ParamAccess.list);
            pManager.AddTextParameter("Folder URL", "urlFolder", "", GH_ParamAccess.list);
            pManager.AddTextParameter("EPW URL", "urlEPW", "Url to .epw file.", GH_ParamAccess.list);
            pManager.AddTextParameter("File Formats", "formats", "CSV List of all available file formats for the given station.", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> newUrls = new List<string>();
            DA.GetDataList(0, newUrls);

            if (newUrls.Count != urls.Count || urls.Except(newUrls).Count() != 0)
            {
                // urls have changed
                urls.Clear();
                urls.AddRange(newUrls);
                Index = Services.GetWeatherDataIndex(urls);
            }
            else
            {
                // urls have not changed
                // if Index is not yet instanciated, create the instance
                if (Index == null)
                {
                    Index = Services.GetWeatherDataIndex(urls);
                }
            }

            var wdList = new List<WeatherData>();
        
            string titleFilter = "";
            double latitude = 0;
            double longitude = 0;
            double radius = 0;
            DA.GetData(1, ref titleFilter);
            DA.GetData(2, ref latitude);
            DA.GetData(3, ref longitude);
            DA.GetData(4, ref radius);

            var filterPoint = new Coordinate(latitude, longitude);

            foreach (var item in Index)
            {
                var include = true;
                if (titleFilter != "")
                    include = include && (item.Title.IndexOf(titleFilter, StringComparison.CurrentCultureIgnoreCase) >= 0);      
                if (radius > 0.0)
                {
                    var coords = new Coordinate(item.Latitude, item.Longitude);
                    var d = GeoCalculator.GetDistance(filterPoint, coords, 2, DistanceUnit.Kilometers);
                    include = include && (d <= radius);
                }
                if (include)
                    wdList.Add(item);                
            }

            DA.SetDataList(0, wdList.Select(wd => wd.Title));
            DA.SetDataList(1, wdList.Select(wd => wd.WMOStationID));
            DA.SetDataList(2, wdList.Select(wd => wd.StationName));
            DA.SetDataList(3, wdList.Select(wd => wd.WMORegionID));
            DA.SetDataList(4, wdList.Select(wd => wd.CountryCode));
            DA.SetDataList(5, wdList.Select(wd => wd.ZoneCode));
            DA.SetDataList(6, wdList.Select(wd => wd.DataSource));
            DA.SetDataList(7, wdList.Select(wd => wd.Latitude));
            DA.SetDataList(8, wdList.Select(wd => wd.Longitude));
            DA.SetDataList(9, wdList.Select(wd => wd.FolderUrl));
            DA.SetDataList(10, wdList.Select(wd => wd.GetFileUrl(WeatherDataFileType.EPW)));
            DA.SetDataList(11, wdList.Select(wd => String.Join(",", wd.FileFormats)));
        }

        
    }
}