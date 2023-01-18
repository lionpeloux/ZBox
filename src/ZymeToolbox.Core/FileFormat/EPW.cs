using System;
using System.IO.Compression;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;
using ZymeToolbox.Core.Types;
using System.Linq;
using ZymeToolbox.Core.API.PVGIS.Responses;

namespace ZymeToolbox.Core.FileFormat
{

    /// <summary>
    /// A Ladybug EPW object containing all of the data of an .epw file?
    /// </summary>
    /// <see cref="https://github.com/ladybug-tools/ladybug/blob/master/ladybug/epw.py"/>
    public class EPW
    {
        #region FIELDS

        public Location Location { get; set; }
        #endregion

        #region INFO

        // Properties:

        // location
        // annual_heating_design_day_996
        // annual_heating_design_day_990
        // annual_cooling_design_day_004
        // annual_cooling_design_day_010
        // heating_design_condition_dictionary
        // cooling_design_condition_dictionary
        // extreme_design_condition_dictionary
        // extreme_hot_weeks
        // extreme_cold_weeks
        // typical_weeks
        // ashrae_climate_zone
        // monthly_ground_temperature
        // header
        // years
        // dry_bulb_temperature
        // dew_point_temperature
        // relative_humidity
        // atmospheric_station_pressure
        // extraterrestrial_horizontal_radiation
        // extraterrestrial_direct_normal_radiation
        // horizontal_infrared_radiation_intensity
        // global_horizontal_radiation
        // direct_normal_radiation
        // diffuse_horizontal_radiation
        // global_horizontal_illuminance
        // direct_normal_illuminance
        // diffuse_horizontal_illuminance
        // zenith_luminance
        // wind_direction
        // wind_speed
        // total_sky_cover
        // opaque_sky_cover
        // visibility
        // ceiling_height
        // present_weather_observation
        // present_weather_codes
        // precipitable_water
        // aerosol_optical_depth
        // snow_depth
        // days_since_last_snowfall
        // albedo
        // liquid_precipitation_depth
        // liquid_precipitation_quantity
        // sky_temperature

        #endregion

        /// <summary>
        /// Get the EPW location from the first line of the EPW file as a string.
        /// </summary>
        /// <example>
        /// LOCATION,Denver Golden Nr,CO,USA,TMY3,724666,39.74,-105.18,-7.0,1829.0
        /// </example>
        public static Location GetLocationFromEPWString(string epwFirstLine)
        {
            var items = epwFirstLine.Trim().Split(',');
            var location = new Location()
            {
                City = items[1].Replace("\\", " ").Replace("/", ""),
                Country = items[3],
                Source = items[4],
                StationID = Convert.ToInt32(items[5]),
                Latitude = Convert.ToDouble(items[6]),
                Longitude = Convert.ToDouble(items[7]),
                TimeZone = Convert.ToDouble(items[8]),
                Elevation = Convert.ToDouble(items[9]),
            };
            return location;
        }
        public static Location GetLocationFromEPWFile(string epwUrl)
        {
            var fileExtension = Path.GetExtension(epwUrl);

            if (fileExtension != ".epw")
                throw new ArgumentException($"Wrong extension {fileExtension}. The epwUrl does not point to a .epw file : {epwUrl}");

            if (System.IO.File.Exists(epwUrl))
            {
                // url is actually a path to a file
                using (var fileStream = new FileStream(epwUrl, FileMode.Open, FileAccess.Read))
                using (var sr = new StreamReader(fileStream))
                {

                    return GetLocationFromEPWString(sr.ReadLine());
                }
            }
            else
            {
                // url is actually an url
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(epwUrl).Result;
                    response.EnsureSuccessStatusCode();

                    using (StreamReader sr = new StreamReader(response.Content.ReadAsStreamAsync().Result))
                    {
                        return GetLocationFromEPWString(sr.ReadLine());
                    }
                }
            }         
        }
    }
}
