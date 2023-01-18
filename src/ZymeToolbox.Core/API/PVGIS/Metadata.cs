using System.Text.Json;

namespace ZymeToolbox.Core.API.PVGIS
{
    /// <summary>
    /// Use this class to build 
    /// </summary>
    public class Metadata
    {
        //  JSON RESPONSE :
        //
        //  "location": {
        //    "latitude": 45.0,
        //    "longitude": 8.0,
        //    "elevation": 250.0
        //  },

        public Location Location { get; set; }
        public MeteoData MeteoData { get; set; }
    }

    public class Location
    {
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public double Elevation { get; init; }

        private Location() { }

        public static Location Create(double latitude, double longitude, double elevation = 0)
        {
            return new Location
            {
                Latitude = latitude,
                Longitude = longitude,
                Elevation = 0
            };
        }
        public static Location Create(JsonElement pvgisLocation)
        {
            return new Location
            {
                Latitude = pvgisLocation.GetProperty("latitude").GetDouble(),
                Longitude = pvgisLocation.GetProperty("longitude").GetDouble(),
                Elevation = pvgisLocation.GetProperty("elevation").GetDouble(),
            };
        }
    }
    public class MeteoData
    {
        //  JSON RESPONSE :
        //
        //  "meteo_data": {
        //    "radiation_db": "PVGIS-SARAH",
        //    "meteo_db": "ERA-Interim",
        //    "year_min": 2005,
        //    "year_max": 2016,
        //    "use_horizon": true,
        //    "horizon_db": "DEM-calculated"
        //  }

        /// <summary>
        /// Solar radiation database.
        /// </summary>
        public string? RadiationDatabase { get; init; }

        /// <summary>
        /// Database used for meteorological variables other than solar radiation.
        /// </summary>
        public string? MeteoDatabase { get; init; }

        /// <summary>
        /// First year of the calculations.
        /// </summary>
        public int? YearMin { get; init; }

        /// <summary>
        /// Last year of the calculations.
        /// </summary>
        public int? YearMax { get; init; }

        /// <summary>
        /// Include horizon shadows if true.
        /// </summary>
        public bool? HorizonEnable { get; init; }

        /// <summary>
        /// Source of horizon data
        /// </summary>
        public string? HorizonData { get; init; }

        private MeteoData() { }
        public static MeteoData Create(string radiationDatabase, string meteoDatabase, int yearMin, int yearMax, bool horizonEnable, string horizonData)
        {
            return new MeteoData
            {
                RadiationDatabase = radiationDatabase,
                MeteoDatabase = meteoDatabase,
                YearMin = yearMin,
                YearMax = yearMax,
                HorizonEnable = horizonEnable,
                HorizonData = horizonData,
            };
        }
        public static MeteoData Create(JsonElement pvgisMeteoData)
        {
            return new MeteoData
            {
                RadiationDatabase = pvgisMeteoData.GetProperty("radiation_db").GetString(),
                MeteoDatabase = pvgisMeteoData.GetProperty("meteo_db").GetString(),
                YearMin = pvgisMeteoData.GetProperty("year_min").GetInt32(),
                YearMax = pvgisMeteoData.GetProperty("year_max").GetInt32(),
                HorizonEnable = pvgisMeteoData.GetProperty("use_horizon").GetBoolean(),

                // 2 fields with different names in the API : just concatenate the two
                // pvgisMeteoData.GetProperty("horizon_data").GetString(),
                HorizonData = pvgisMeteoData.GetProperty("horizon_db").GetString()
            };
        }
    }


}
