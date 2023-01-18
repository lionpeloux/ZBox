using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Xml;

namespace ZymeToolbox.Core.API.OneBuilding
{
    public static partial class Services
    {
        /// <summary>
        /// List of all reference .kml files from Climate.OneBuilding.Org.
        /// </summary>
        public static readonly string[] WeatherDataIndexFiles = new string[13] {
            @"https://climate.onebuilding.org/WMO_Region_1_Africa/Region1_Africa_EPW_Processing_locations.kml",
            @"https://climate.onebuilding.org/WMO_Region_2_Asia/Region2_Asia_EPW_Processing_locations.kml",
            @"https://climate.onebuilding.org/WMO_Region_2_Asia/Region2_Region6_Russia_EPW_Processing_locations.kml",
            @"https://climate.onebuilding.org/WMO_Region_3_South_America/Region3_South_America_EPW_Processing_locations.kml",
            @"https://climate.onebuilding.org/WMO_Region_4_North_and_Central_America/Region4_Canada_EPW_Processing_locations.kml",
            @"https://climate.onebuilding.org/WMO_Region_4_North_and_Central_America/Region4_USA_EPW_Processing_locations.kml",
            @"https://climate.onebuilding.org/WMO_Region_4_North_and_Central_America/Region4_NA_CA_Caribbean_EPW_Processing_locations.kml",
            @"https://climate.onebuilding.org/WMO_Region_4_North_and_Central_America/Region4_CaliforniaClimateZones_EPW_Processing_locations.kml",
            @"https://climate.onebuilding.org/WMO_Region_4_North_and_Central_America/Region4_Canada_NRC_Future_EPW_Processing_locations.kml",
            @"https://climate.onebuilding.org/WMO_Region_5_Southwest_Pacific/Region5_Southwest_Pacific_EPW_Processing_locations.kml",
            @"https://climate.onebuilding.org/WMO_Region_6_Europe/Region6_Europe_EPW_Processing_locations.kml",
            @"https://climate.onebuilding.org/WMO_Region_6_Europe/Region2_Region6_Russia_EPW_Processing_locations.kml",
            @"https://climate.onebuilding.org/WMO_Region_7_Antarctica/Region7_Antarctica_EPW_Processing_locations.kml"
        };

        /// <summary>
        /// Return a Weather Data Index from a .kml file specified by an url (or a local path). 
        /// </summary>
        /// <param name="url">Either an url to the file (on a server) or the path to a local copy of the file.</param>
        /// <returns></returns>
        public static List<WeatherData> GetWeatherDataIndex(string url)
        {
            if (System.IO.File.Exists(url))
            {
                // url is actually a path to a file
                using (var stream = new FileStream(url, FileMode.Open, FileAccess.Read))
                {
                    return GetWeatherIndexFromKML(stream);
                }
            }
            else
            {
                // url is actually an url
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();

                    using (var stream = response.Content.ReadAsStreamAsync().Result)
                    {
                        return GetWeatherIndexFromKML(stream);
                    }
                }
            }       
        }
        public static List<WeatherData> GetWeatherDataIndex(IEnumerable<string> urls)
        {
            var wd = new List<WeatherData>();
            foreach (var url in urls)
            {
                wd.AddRange(GetWeatherDataIndex(url));
            }
            return wd;
        }
        private static List<WeatherData> GetWeatherIndexFromKML(Stream stream)
        {
            var placemarks = new List<Placemark>();

            using (var reader = XmlReader.Create(stream))
            {
                int count = 0;

                reader.ReadToFollowing("Placemark");
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "Placemark")
                        {
                            count++;
                            var placemark = new Placemark();
                            ReadPlacemark(reader.ReadSubtree(), ref placemark);
                            if (placemark.Description != null)
                            {
                                placemarks.Add(placemark);
                            }
                        }
                    }
                }
            }

            return placemarks.Select(p => new WeatherData(p)).ToList();
        }
        private static void ReadPlacemark(XmlReader reader, ref Placemark placemark)
        {
            string name, description, styleUrl;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "name":
                            name = reader.ReadElementContentAsString();
                            placemark.Name = name;
                            break;
                        case "description":
                            description = reader.ReadElementContentAsString();
                            description.Replace("![CDATA[", "").Replace("]]", "");
                            placemark.Description = description;
                            break;
                        case "styleUrl":
                            styleUrl = reader.ReadElementContentAsString();
                            placemark.StyleUrl = styleUrl;
                            break;
                        case "Point":
                            ReadPoint(reader.ReadSubtree(), ref placemark);
                            break;
                    }
                }
            }
        }
        private static void ReadPoint(XmlReader reader, ref Placemark placemark)
        {
            string altitudeMode, coordinates;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "altitudeMode":
                            altitudeMode = reader.ReadElementContentAsString();
                            placemark.AltitudeMode = altitudeMode;
                            break;
                        case "coordinates":
                            coordinates = reader.ReadElementContentAsString();
                            placemark.Coordinates = coordinates;
                            break;
                    }
                }
            }
        }
    }
}
