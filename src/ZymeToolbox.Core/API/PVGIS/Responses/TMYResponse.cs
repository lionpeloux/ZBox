using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Core.API.PVGIS.Responses
{
    public class TMYResponse : Response
    {
        /// <summary>
        /// Hourly time stamp in UTC
        /// </summary>
        public DataColumn<string> Time { get; private set; }

        /// <summary>
        /// Global solar irradiance on the horizontal plane in W/m2.
        /// </summary>
        public DataColumn<double> GlobalIrradiance { get; private set; }
        /// <summary>
        /// Direct (beam) solar irradiance (normal to sun) in W/m2.
        /// </summary>
        public DataColumn<double> DirectIrradiance { get; private set; }

        /// <summary>
        /// Diffuse solar irradiance on the horizontal plane in W/m2.
        /// </summary>
        public DataColumn<double> DiffuseIrradiance { get; private set; }

        /// <summary>
        /// Surface infrared (thermal) irradiance on the horizontal plane in W/m2.
        /// </summary>
        public DataColumn<double> InfraredIrradiance { get; private set; }

        /// <summary>
        /// Air temperature at 2 meters in degree celsius.
        /// </summary>
        public DataColumn<double> Temperature_2m { get; private set; }

        /// <summary>
        /// Relative humidity of air in %.
        /// </summary>
        public DataColumn<double> RelativeHumidity { get; private set; }

        /// <summary>
        /// Wind speed at 10 meters in m/s.
        /// </summary>
        public DataColumn<double> WindSpeed_10m { get; private set; }

        /// <summary>
        /// Wind direction at 10 meters in degree.
        /// 0° = North | 90° = Est | 180° = South | 270° = West.
        /// </summary>
        public DataColumn<double> WindDirection_10m { get; private set; }

        /// <summary>
        /// Air pressure in Pa.
        /// </summary>
        public DataColumn<double> AirPressure { get; private set; }



        private TMYResponse(string urlQuery) : base(urlQuery)
        {
            Time = new DataColumn<string>("time(UTC)", "Hourly time stamps.", "UTC");
            GlobalIrradiance = new DataColumn<double>("G(h)", "Global solar irradiance on the horizontal plane.", "W/m2");
            DirectIrradiance = new DataColumn<double>("Gb(n)", "Direct (beam) solar irradiance (normal to sun).", "W/m2");
            DiffuseIrradiance = new DataColumn<double>("Gd(h)", "Diffuse solar irradiance on the horizontal plane.", "W/m2");
            InfraredIrradiance = new DataColumn<double>("IR(h)", "Surface infrared (thermal) irradiance on the horizontal plane.", "W/m2");

            Temperature_2m = new DataColumn<double>("T2m", "Air temperature at 2 meters.", "°C");
            RelativeHumidity = new DataColumn<double>("RH", "Relative humidity of air.", "%");
            AirPressure = new DataColumn<double>("SP", "Air pressure.", "Pa");

            WindSpeed_10m = new DataColumn<double>("WS10m", "Wind speed at 10 meters.", "m/s");
            WindDirection_10m = new DataColumn<double>("WD10m", "Wind direction at 10 meters.", "degree");
        }

        internal static TMYResponse Create(TMYQuery query)
        {
            var urlQuery = query.Build();
            var content = new TMYResponse(urlQuery);

            var tmy = content.Outputs.GetProperty("tmy_hourly");
            var timeFormat = "yyyyMMdd:HHmm";

            foreach (var item in tmy.EnumerateArray())
            {
                content.Time.AddFromJson(item);
                content.GlobalIrradiance.AddFromJson(item);
                content.DirectIrradiance.AddFromJson(item);
                content.DiffuseIrradiance.AddFromJson(item);
                content.InfraredIrradiance.AddFromJson(item);
                content.Temperature_2m.AddFromJson(item);
                content.RelativeHumidity.AddFromJson(item);
                content.AirPressure.AddFromJson(item);
                content.WindSpeed_10m.AddFromJson(item);
                content.WindDirection_10m.AddFromJson(item);
            }

            return content;
        }

    }
   
}