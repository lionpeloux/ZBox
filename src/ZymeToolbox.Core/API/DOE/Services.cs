using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml;
using ZymeToolbox.Core.API.OneBuilding;

namespace ZymeToolbox.Core.API.DOE
{
    public static class Services
    {
        /// <summary>
        /// List of all reference .geojson files from EnergyPlus.net.
        /// </summary>
        public static readonly string[] WeatherDataIndexFiles = new string[1] {
                @"https://raw.githubusercontent.com/NREL/EnergyPlus/develop/weather/master.geojson"
            };

        /// <summary>
        /// Return a Weather Data Index from a .geojson file specified by an url (or a local path). 
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
                    return GetWeatherIndexFromJSON(stream);
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
                        return GetWeatherIndexFromJSON(stream);
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
        private static List<WeatherData> GetWeatherIndexFromJSON(Stream stream)
        {
            var wd = new List<WeatherData>();

            using (var doc = JsonDocument.Parse(stream))
            {
                var root = doc.RootElement;
                var features = root.GetProperty("features");
                foreach (var feature in features.EnumerateArray())
                {
                    wd.Add(WeatherData.Create(feature));
                }
            }
            return wd;
        }
    }
}
