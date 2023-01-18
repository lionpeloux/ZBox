using Geolocation;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ZymeToolbox.Core.API.OneBuilding;


namespace ZymeToolbox.Climat.Grasshopper.Components
{
    public class GHComp_OneBuilding_WeatherDataIndex : GH_Component
    {
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{1CBDFF8A-71A1-48DD-8BC9-4E53E6F05F00}");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        private List<string> urls = new List<string>();
        private static List<WeatherData> Index = null;

        public GHComp_OneBuilding_WeatherDataIndex()
          : base("OneBuilding Weather Data Index", "OB Weather Data Index", 
                "Build an index of all available weather data from Climate.OneBuilding.Org. " +
                "This is achieved through parsing the input files. " +
                "Note that the index is built only once (unless urls parameter is changed) to allow reactive filtering",
            "ZBox", "2 | Climat API")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Url to .KML File(s)", "urls", "Urls to the .kml file(s) requiered to build the index. Url(s) can actually be path(s) to local file(s).", GH_ParamAccess.list, Services.WeatherDataIndexFiles);
            pManager.AddTextParameter("Filter Title", "filterTitle", "A string to filter the title (cas insensitive)", GH_ParamAccess.item, "");
            pManager.AddNumberParameter("Filter Location", "filterPtLatitude", "Latitude of the point to filter from.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Filter Location", "filterPtLongitude", "Longitude of the point to filter from.", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Filter Radius in km", "filterRadius", "Selection radius to filter the outputs. No filter if <= 0.", GH_ParamAccess.item, 0.0);

            //pManager.AddTextParameter("Path to .Json File", "path_json", "Path to the .json file", GH_ParamAccess.item);
            //pManager.AddBooleanParameter("Write Json Index", "write", "Dump the data to a .json index", GH_ParamAccess.item, false);

            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Weather Data Title", "title", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("WMO Station ID", "stationID", "", GH_ParamAccess.list);
            pManager.AddTextParameter("Station Name", "stationName", "", GH_ParamAccess.list);
            pManager.AddIntegerParameter("WMO Region ID", "regionID", "", GH_ParamAccess.list);
            pManager.AddTextParameter("Data Source", "dataSource", "", GH_ParamAccess.list);
            pManager.AddTextParameter("Data Set", "dataSet", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Latitude", "latitude", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Longitude", "longitude", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Elevation", "elevation", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Start Year", "startYear", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("End Year", "endYear", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Number Of Years", "numYears", "", GH_ParamAccess.list);
            pManager.AddNumberParameter("Time Zone", "timeZone", "", GH_ParamAccess.list);
            pManager.AddTextParameter("URL", "url", "", GH_ParamAccess.list);
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

            //bool write = false;
            //if (DA.GetData(2, ref write))
            //{
            //    if (write == true)
            //    {
            //        string path_json = "";
            //        if (DA.GetData(1, ref path_json))
            //        {
            //            if (path_json != null)
            //            {
            //                var jsonString = JsonSerializer.Serialize(dataPoints);
            //                File.WriteAllText(path_json, jsonString);
            //            }

            //        }
            //    }
            //}


            DA.SetDataList(0, wdList.Select(wd => wd.Title));
            DA.SetDataList(1, wdList.Select(wd => wd.WMOStationID));
            DA.SetDataList(2, wdList.Select(wd => wd.StationName));
            DA.SetDataList(3, wdList.Select(wd => wd.WMORegionID));
            DA.SetDataList(4, wdList.Select(wd => wd.DataSource));
            DA.SetDataList(5, wdList.Select(wd => wd.Provider));

            DA.SetDataList(6, wdList.Select(wd => wd.Latitude));
            DA.SetDataList(7, wdList.Select(wd => wd.Longitude));
            DA.SetDataList(8, wdList.Select(wd => wd.Elevation));
            DA.SetDataList(9, wdList.Select(wd => wd.StartYear));
            DA.SetDataList(10, wdList.Select(wd => wd.EndYear));
            DA.SetDataList(11, wdList.Select(wd => wd.NumOfYears));
            DA.SetDataList(12, wdList.Select(wd => wd.TimeZoneOffset.TotalHours));
            DA.SetDataList(13, wdList.Select(wd => wd.Url));
        }



    }
}