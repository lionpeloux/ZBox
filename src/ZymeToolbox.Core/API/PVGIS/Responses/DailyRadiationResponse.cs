using System.Text.Json;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Core.API.PVGIS.Responses
{
    public class DailyRadiationResponse : Response
    {
        /// <summary>
        /// Number of the month.
        /// </summary>
        public DataColumn<int> Month { get; private set; }

        /// <summary>
        /// Hourly time stamp (either local or utc).
        /// </summary>
        public DataColumn<string> Time { get; private set; }

        /// <summary>
        /// Global clear-sky solar irradiance on the (inclined) plane of array in W/m2.
        /// </summary>
        public DataColumn<double> GlobalClearSkyIrradiance_POA { get; private set; }

        /// <summary>
        /// Global solar irradiance on the (inclined) plane of array in W/m2.
        /// </summary>
        public DataColumn<double> GlobalIrradiance_POA { get; private set; }

        /// <summary>
        /// Direct (beam) solar irradiance on the (inclined) plane of array in W/m2.
        /// </summary>
        public DataColumn<double> DirectIrradiance_POA { get; private set; }

        /// <summary>
        /// Diffuse solar irradiance on the (inclined) plane of array in W/m2.
        /// </summary>
        public DataColumn<double> DiffuseIrradiance_POA { get; private set; }

        /// <summary>
        /// Global clear-sky solar irradiance on the 2-axis tracking plane in W/m2.
        /// </summary>
        public DataColumn<double> GlobalClearSkyIrradiance_2Axis { get; private set; }

        /// <summary>
        /// Global solar irradiance on the 2-axis tracking plane in W/m2.
        /// </summary>
        public DataColumn<double> GlobalIrradiance_2Axis { get; private set; }

        /// <summary>
        /// Direct (beam) solar irradiance on the 2-axis tracking plane in W/m2.
        /// </summary>
        public DataColumn<double> DirectIrradiance_2Axis { get; private set; }

        /// <summary>
        /// Diffuse solar irradiance on the 2-axis tracking plane in W/m2.
        /// </summary>
        public DataColumn<double> DiffuseIrradiance_2Axis { get; private set; }

        /// <summary>
        /// Air temperature at 2 meters in degree celsius.
        /// </summary>
        public DataColumn<double> Temperature_2m { get; private set; }

        private DailyRadiationResponse(string urlQuery) : base(urlQuery)
        {
            Month = new DataColumn<int>("month", "Number of the month.", "");
            Time = new DataColumn<string>("time", "Hourly time stamps.", "UTC or Local");

            GlobalClearSkyIrradiance_POA = new DataColumn<double>("Gcs(i)", "Global clear-sky solar irradiance on the (inclined) plane of array.", "W/m2");
            GlobalIrradiance_POA = new DataColumn<double>("G(i)", "Global solar irradiance on the (inclined) plane of array.", "W/m2");
            DirectIrradiance_POA = new DataColumn<double>("Gb(i)", "Direct (beam) solar irradiance on the (inclined) plane of array.", "W/m2");
            DiffuseIrradiance_POA = new DataColumn<double>("Gd(i)", "Diffuse solar irradiance on the (inclined) plane of array.", "W/m2");

            GlobalClearSkyIrradiance_2Axis = new DataColumn<double>("Gcs(n)", "Global clear-sky solar irradiance on the 2-axis tracking plane.", "W/m2");
            GlobalIrradiance_2Axis = new DataColumn<double>("G(n)", "Global solar irradiance on the 2-axis tracking plane.", "W/m2");
            DirectIrradiance_2Axis = new DataColumn<double>("Gb(n)", "Direct (beam) solar irradiance on the 2-axis tracking plane.", "W/m2");
            DiffuseIrradiance_2Axis = new DataColumn<double>("Gd(n)", "Diffuse solar irradiance on the 2-axis tracking plane.", "W/m2");

            Temperature_2m = new DataColumn<double>("T2m", "Air temperature at 2 meters.", "°C");
        }

        internal static DailyRadiationResponse Create(DailyRadiationQuery query)
        {
            var urlQuery = query.Build();
            var content = new DailyRadiationResponse(urlQuery);

            var hr = content.Outputs.GetProperty("daily_profile");
            var timeFormat = "HH:mm";

            foreach (var item in hr.EnumerateArray())
            {
                content.Month.AddFromJson(item);
                content.Time.AddFromJson(item);

                content.GlobalClearSkyIrradiance_POA.AddFromJson(item);
                content.GlobalIrradiance_POA.AddFromJson(item);
                content.DirectIrradiance_POA.AddFromJson(item);
                content.DiffuseIrradiance_POA.AddFromJson(item);

                content.GlobalClearSkyIrradiance_2Axis.AddFromJson(item);
                content.GlobalIrradiance_2Axis.AddFromJson(item);
                content.DirectIrradiance_2Axis.AddFromJson(item);
                content.DiffuseIrradiance_2Axis.AddFromJson(item);

                content.Temperature_2m.AddFromJson(item);
            }

            return content;
        }

    }


}