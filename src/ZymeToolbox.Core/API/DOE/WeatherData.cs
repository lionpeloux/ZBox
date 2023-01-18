using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ZymeToolbox.Core.API.DOE
{

    public enum WeatherDataFileType
    {
        EPW,
        DDY,
        STAT,
        MOS,
        ZIP,
    }
    public record WeatherData
    {
        private static Regex regexTitle = new Regex(@"(?<country>(\D*))_(?<name>([\D\.]*))(?<id>(\d*))_(?<source>(\w*))");
        private static Regex regexRegion = new Regex(@"(wmo_region)_(?<region>(\d))");
        private static Regex regexUrl = new Regex(@"<a href=(?<url>([^\>]*))");

        private string _fileBaseUrl;

        public string Title { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public int WMORegionID { get; init; }
        public int? WMOStationID { get; init; }
        public string CountryCode { get; init; }
        public string ZoneCode { get; init; }
        public string StationName { get; init; }
        public string DataSource { get; init; }

        public string FolderUrl { get; init; }


        public List<string> FileFormats { get; init; }


        private WeatherData(JsonElement element)
        {
            Match match;
            var props = element.GetProperty("properties");

            // Title
            Title = props.GetProperty("title").GetString();

            // FolderUrl
            var dir = props.GetProperty("dir").GetString();
            match = regexUrl.Match(dir);
            if (match.Success) FolderUrl = match.Groups["url"].Value;
            else throw new Exception($"Unable to extract FolderUrl for data : {Title}");

            // FileUrl | WMO Region ID | CountryCode | ZoneCode
            var all = props.GetProperty("all").GetString();
            match = regexUrl.Match(all);
            if (match.Success)
            {
                var fileUrl = match.Groups["url"].Value;
                var segments = fileUrl.Split('/').ToList();
                segments.RemoveAt(segments.Count - 1);

                _fileBaseUrl = String.Join("/", segments);

                var region_str = segments[3];
                CountryCode = segments[4];
                if (segments.Count == 7)
                    ZoneCode = segments[5];

                match = regexRegion.Match(region_str);
                if (match.Success)
                    WMORegionID = Convert.ToInt32(match.Groups["region"].Value);
                else
                    throw new Exception($"Unable to extract WMORegionID for data : {Title}");
            }
            else throw new Exception($"Unable to extract FolderUrl for data : {Title}");

            // StationID | StationName | DataSource
            match = regexTitle.Match(Title);
            if (match.Success)
            {
                var id = match.Groups["id"].Value;
                if (!(id == "" || id == null))
                {
                    WMOStationID = Convert.ToInt32(id);
                }
                StationName = match.Groups["name"].Value;
                DataSource = match.Groups["source"].Value;
            }
            else
            {
                StationName = Title;
            }

            // Coords : [lon lat]
            var coords = element.GetProperty("geometry").GetProperty("coordinates").EnumerateArray().Select(e => e.GetDouble()).ToList();
            Longitude = coords[0];
            Latitude = coords[1];

            // File Formats
            FileFormats = new List<string>();
            foreach (var prop in props.EnumerateObject())
            {
                var name = prop.Name;
                switch (name)
                {
                    case "title":
                        break;
                    case "dir":
                        break;
                    case "all":
                        FileFormats.Add("zip");
                        break;
                    default:
                        FileFormats.Add(name);
                        break;
                }
            }
        }
        public static WeatherData Create(JsonElement element) => new WeatherData(element);

        public string GetFileUrl(WeatherDataFileType fileType)
        {
            var fileExtension = fileType.ToString().ToLower();
            return $"{_fileBaseUrl}/{Title}.{fileExtension}";
        }
    }
}
