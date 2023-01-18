using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static ZymeToolbox.Core.API.OneBuilding.Services;

namespace ZymeToolbox.Core.API.OneBuilding
{
    public record WeatherData
    {
        private static Regex regex_1 = new Regex("Data Source (?<datasource>(.*))");
        private static Regex regex_2 = new Regex(@"(?<provider>(.*))( - )(.*)\[(?<numyears>(\d*))](.*)=(?<startyear>(\d{4}))-(?<endyear>(\d{4}))");
        private static Regex regex_3 = new Regex(@"(\D*)(?<id>(\d*))");
        private static Regex regex_6 = new Regex(@"(?<sign>(\+|\-))(?<timezone>(\d*\.\d*))");
        private static Regex regex_11 = new Regex(@"URL (?<url>(.*))");
        private static Regex regex_region = new Regex(@"(.*)WMO_Region_(?<region>(\d))");

        public string Title { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public double Elevation { get; init; }
        public int WMORegionID { get; init; }
        public int WMOStationID { get; init; }
        public string StationName { get; init; }
        public string? Url { get; init; }

        public string Provider { get; init; }
        public string DataSource { get; init; }
        public int StartYear { get; init; }
        public int EndYear { get; init; }
        public int NumOfYears { get; init; }

        public string TimeZone { get; init; }
        public TimeSpan TimeZoneOffset { get; init; }

        internal WeatherData(Placemark placemark)
        {
            StationName = placemark.Name;
            
            var coords = placemark.Coordinates.Split(',');
            Latitude = Convert.ToDouble(coords[1]);
            Longitude = Convert.ToDouble(coords[0]);
            Elevation = Convert.ToDouble(coords[2]);

            if (placemark.Description !=null)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(placemark.Description);

                Match match;
                var cells = doc.DocumentNode.SelectNodes("/table/tr/td").ToList();

                for (int i = 0; i < cells.Count; i++)
                {
                    var content = cells[i].InnerText;
                    switch (i)
                    {
                        case 0:
                            Title = content.StripHTMLTags();
                            break;
                        case 1:
                            DataSource = regex_1.Match(content).Groups[1].ToString();
                            break;
                        case 2:
                            match = regex_2.Match(content);
                            if (match.Success)
                            {
                                Provider = match.Groups["provider"].Value;
                                NumOfYears = Convert.ToInt32(match.Groups["numyears"].Value);
                                StartYear = Convert.ToInt32(match.Groups["startyear"].Value);
                                EndYear = Convert.ToInt32(match.Groups["endyear"].Value);
                            }
                            break;
                        case 3:
                            match = regex_3.Match(content);
                            if (match.Success)
                            {
                                WMOStationID = Convert.ToInt32(match.Groups["id"].Value);
                            }
                            break;
                        case 6:
                            match = regex_6.Match(content);
                            if (match.Success)
                            {
                                string sign = match.Groups["sign"].Value;
                                string timezone = match.Groups["timezone"].Value;
                                TimeZone = sign + timezone;
                                TimeZoneOffset = TimeSpan.FromHours(Convert.ToDouble(timezone));
                                if (sign == "-") TimeZoneOffset.Negate();
                            }
                            break;
                        case 11:
                            match = regex_11.Match(content);
                            if (match.Success)
                            {
                                Url = match.Groups["url"].Value;
                                match = regex_region.Match(Url);
                                if (match.Success)
                                    WMORegionID = Convert.ToInt32(match.Groups["region"].Value);
                            }
                            break;
                        default:
                            break;
                    }

                }

            }

        }

        public override string ToString()
        {
            if (Url != null)
            {
                return Url.Split('/').Last();
            }
            else return "";
                
        }

        public void ExtractUrlInfo()
        {
            var parts = Url.Split('/');
            var regionStr = parts[1];
            var countryStr = parts[2];
            var cityStr = parts[3];
            var fileStr = parts[4];



        }

    }
}
