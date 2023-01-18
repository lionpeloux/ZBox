using System.Text.Json;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Core.API.PVGIS.Responses
{
    public class MonthlyRadiationResponse : Response
    {
        /// <summary>
        /// Year.
        /// </summary>
        public DataColumn<int> Year { get; private set; }
        
        /// <summary>
        /// Number of the month.
        /// </summary>
        public DataColumn<int> Month { get; private set; }

        /// <summary>
        /// Monthly global solar irradiance on the horizontal plane in kWh/m2/month.
        /// </summary>
        public DataColumn<double> GlobalIrradiance_Horizontal { get; private set; }

        /// <summary>
        /// Monthly global solar irradiance on the optimaly oriented plane in kWh/m2/month.
        /// </summary>
        public DataColumn<double> GlobalIrradiance_Optimal { get; private set; }

        /// <summary>
        /// Monthly global solar irradiance on the (inclined) plane of array in kWh/m2/month.
        /// </summary>
        public DataColumn<double> GlobalIrradiance_POA { get; private set; }

        /// <summary>
        /// Monthly direct (beam) solar irradiance on the plane always normal to sun rays in kWh/m2/month.
        /// </summary>
        public DataColumn<double> DirectIrradiance_Normal { get; private set; }

        /// <summary>
        /// Ratio of diffuse to global irradiance.
        /// </summary>
        public DataColumn<double> DiffuseToGlobalIrradianceRatio { get; private set; }

        /// <summary>
        /// 24 hours average of air temperature at 2 meters in degree celsius.
        /// </summary>
        public DataColumn<double> AverageTemperature_2m { get; private set; }

        private MonthlyRadiationResponse(string urlQuery) : base(urlQuery)
        {
            Year = new DataColumn<int>("year", "Year.", "");
            Month = new DataColumn<int>("month", "Month of the year.", "");

            GlobalIrradiance_Horizontal = new DataColumn<double>("H(h)_m", "Monthly global solar irradiance on the horizontal plane.", "kWh/m2/month");
            GlobalIrradiance_Optimal = new DataColumn<double>("H(i_opt)_m", "Monthly global solar irradiance on the optimaly oriented plane.", "kWh/m2/month");
            GlobalIrradiance_POA = new DataColumn<double>("H(i)_m", "Monthly global solar irradiance on the (inclined) plane.", "kWh/m2/month");
            DirectIrradiance_Normal = new DataColumn<double>("Hb(n)_m", "Monthly direct (beam) solar irradiance on the plane always normal to sun rays.", "kWh/m2/month");

            DiffuseToGlobalIrradianceRatio = new DataColumn<double>("Kd", "Ratio of diffuse to global irradiation.", "");
            AverageTemperature_2m = new DataColumn<double>("T2m", "24 hours average of air temperature at 2 meters in degree celsius.", "°C");
        }

        internal static MonthlyRadiationResponse Create(MonthlyRadiationQuery query)
        {
            var urlQuery = query.Build();
            var content = new MonthlyRadiationResponse(urlQuery);

            var hr = content.Outputs.GetProperty("monthly");
            var timeFormat = "HH:mm";

            foreach (var item in hr.EnumerateArray())
            {
                content.Month.AddFromJson(item);
                content.Year.AddFromJson(item);

                content.GlobalIrradiance_Horizontal.AddFromJson(item);
                content.GlobalIrradiance_POA.AddFromJson(item);
                content.GlobalIrradiance_Optimal.AddFromJson(item);
                content.DirectIrradiance_Normal.AddFromJson(item);

                content.DiffuseToGlobalIrradianceRatio.AddFromJson(item);
                content.AverageTemperature_2m.AddFromJson(item);
            }

            return content;
        }

    }


}