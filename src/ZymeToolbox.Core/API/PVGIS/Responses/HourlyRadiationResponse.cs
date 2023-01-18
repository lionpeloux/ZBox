using System.Text.Json;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Core.API.PVGIS.Responses
{
    public class HourlyRadiationResponse : Response
    {
        /// <summary>
        /// Hourly time stamp.
        /// </summary>
        public DataColumn<string> Time { get; private set; }

        /// <summary>
        /// PV system power in kW.
        /// </summary>
        public DataColumn<double> PVPower { get; private set; }

        /// <summary>
        /// Direct (beam) solar irradiance on the (inclined) plane of array in W/m2.
        /// </summary>
        public DataColumn<double> DirectIrradiance_POA { get; private set; }

        /// <summary>
        /// Diffuse solar irradiance on the (inclined) plane of array in W/m2.
        /// </summary>
        public DataColumn<double> DiffuseIrradiance_POA { get; private set; }

        /// <summary>
        /// Reflected (ground) solar irradiance on the (inclined) plane of array in W/m2.
        /// </summary>
        public DataColumn<double> ReflectedIrradiance_POA { get; private set; }

        /// <summary>
        /// Sun height in degree.
        /// </summary>
        public DataColumn<double> SunHeight { get; private set; }

        /// <summary>
        /// Air temperature at 2 meters in degree celsius.
        /// </summary>
        public DataColumn<double> Temperature_2m { get; private set; }

        /// <summary>
        /// Total wind speed at 10 meters in m/s.
        /// </summary>
        public DataColumn<double> WindSpeed_10m { get; private set; }

        /// <summary>
        /// Indicates whether the solar radiation values are reconstructed or not.
        /// </summary>
        public DataColumn<bool> IsReconstructed { get; private set; }

        private HourlyRadiationResponse(string urlQuery) : base(urlQuery)
        {
            Time = new DataColumn<string>("time", "Hourly time stamps.", "?");
            PVPower = new DataColumn<double>("P", "PV system power.", "W");
            DirectIrradiance_POA = new DataColumn<double>("Gb(i)", "Direct (beam) solar irradiance on the (inclined) plane of array.", "W/m2");
            DiffuseIrradiance_POA = new DataColumn<double>("Gd(i)", "Diffuse solar irradiance on the (inclined) plane of array.", "W/m2");
            ReflectedIrradiance_POA = new DataColumn<double>("Gr(i)", "Reflected (ground) solar irradiance on the (inclined) plane of array.", "W/m2");

            SunHeight = new DataColumn<double>("H_sun", "Sun height.", "degree");
            Temperature_2m = new DataColumn<double>("T2m", "Air temperature at 2 meters.", "°C");
            WindSpeed_10m = new DataColumn<double>("WS10m", "Total wind speed at 10 meters.", "m/s");
            IsReconstructed = new DataColumn<bool>("Int", "Is Reconstructed", "bool");
        }

        internal static HourlyRadiationResponse Create(HourlyRadiationQuery query)
        {
            var urlQuery = query.Build();
            var content = new HourlyRadiationResponse(urlQuery);

            var hr = content.Outputs.GetProperty("hourly");
            var timeFormat = "yyyyMMdd:HHmm";

            foreach (var item in hr.EnumerateArray())
            {
                content.Time.AddFromJson(item);
                content.PVPower.AddFromJson(item);
                content.DirectIrradiance_POA.AddFromJson(item);
                content.DiffuseIrradiance_POA.AddFromJson(item);
                content.ReflectedIrradiance_POA.AddFromJson(item);
                content.SunHeight.AddFromJson(item);
                content.Temperature_2m.AddFromJson(item);
                content.WindSpeed_10m.AddFromJson(item);
                content.IsReconstructed.AddFromJson(item);
            }

            return content;
        }

    }


}