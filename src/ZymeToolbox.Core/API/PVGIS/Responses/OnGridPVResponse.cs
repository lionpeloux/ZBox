using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Core.API.PVGIS.Responses
{
    public class OnGridPVResponse : Response
    {
        /// <summary>
        /// Month
        /// </summary>
        public DataColumn<int> Month { get; private set; }

        /// <summary>
        /// Average daily energy production from the given system in kWh/day.
        /// </summary>
        public DataColumn<double> AverageDailyEnergyProduction { get; private set; }

        /// <summary>
        /// Average monthly energy production from the given system in kWh/month.
        /// </summary>
        public DataColumn<double> AverageMonthlyEnergyProduction { get; private set; }

        /// <summary>
        /// Average daily sum of global irradiance per square meter received by the modules of the given system in kWh/m2/day.
        /// </summary>
        public DataColumn<double> AverageDailySumOfGlobalIrradiance_POA { get; private set; }

        /// <summary>
        /// Average monthly sum of global irradiance per square meter received by the modules of the given system in kWh/m2/month.
        /// </summary>
        public DataColumn<double> AverageMonthlySumOfGlobalIrradiance_POA { get; private set; }

        /// <summary>
        /// Standard deviation of the monthly energy production due to year-to-year variation in kWh.
        /// </summary>
        public DataColumn<double> DeviationOfMonthlyEnergyProduction { get; private set; }


        /// <summary>
        /// Totals.
        /// </summary>
        public OnGridPVTotals Totals { get; private set; }

        private OnGridPVResponse(string urlQuery) : base(urlQuery)
        {
            Month = new DataColumn<int>("month", "Hourly time stamps.", "?");
            AverageDailyEnergyProduction = new DataColumn<double>("E_d", "Average daily energy production from the given system.", "kWh/day");
            AverageMonthlyEnergyProduction = new DataColumn<double>("E_m", "Average monthly energy production from the given system.", "kWh/month");
            AverageDailySumOfGlobalIrradiance_POA = new DataColumn<double>("H(i)_d", "Average daily sum of global irradiance per square meter received by the modules of the given system.", " kWh/m2/day");
            AverageMonthlySumOfGlobalIrradiance_POA = new DataColumn<double>("H(i)_m", "Average monthly sum of global irradiance per square meter received by the modules of the given system.", " kWh/m2/month");
            DeviationOfMonthlyEnergyProduction = new DataColumn<double>("SD_m", "Standard deviation of the monthly energy production due to year-to-year variation.", "kWh");
        }

        internal static OnGridPVResponse Create(OnGridPVQuery query)
        {
            var urlQuery = query.Build();
            var content = new OnGridPVResponse(urlQuery);

            var mf = content.Outputs.GetProperty("monthly").GetProperty("fixed");

            foreach (var item in mf.EnumerateArray())
            {
                content.Month.AddFromJson(item);
                content.AverageDailyEnergyProduction.AddFromJson(item);
                content.AverageMonthlyEnergyProduction.AddFromJson(item);
                content.AverageDailySumOfGlobalIrradiance_POA.AddFromJson(item);
                content.AverageMonthlySumOfGlobalIrradiance_POA.AddFromJson(item);
                content.DeviationOfMonthlyEnergyProduction.AddFromJson(item);
            }

            var tf = content.Outputs.GetProperty("totals").GetProperty("fixed");
            content.Totals = OnGridPVTotals.Create(tf);

            return content;
        }

        public record OnGridPVTotals
        {
            /// <summary>
            /// Average daily energy production from the given system in kWh/day.
            /// </summary>
            public double AverageDailyEnergyProduction { get; init; }

            /// <summary>
            /// Average monthly energy production from the given system in kWh/month.
            /// </summary>
            public double AverageMonthlyEnergyProduction { get; init; }

            /// <summary>
            /// Average annual energy production from the given system in kWh/year.
            /// </summary>
            public double AverageYearlyEnergyProduction { get; init; }

            /// <summary>
            /// Average daily sum of global irradiation per square meter received by the modules of the given system in kWh/m2/day.
            /// </summary>
            public double AverageDailySumOfGlobalIrradiance { get; init; }

            /// <summary>
            /// Average monthly sum of global irradiation per square meter received by the modules of the given system in kWh/m2/month.
            /// </summary>
            public double AverageMonthlySumOfGlobalIrradiance { get; init; }

            /// <summary>
            /// Average yearly sum of global irradiation per square meter received by the modules of the given system in kWh/m2/year.
            /// </summary>
            public double AverageYearlySumOfGlobalIrradiance { get; init; }

            /// <summary>
            /// Standard deviation of the monthly energy production due to year-to-year variation in kWh.
            /// </summary>
            public double DeviationOfMonthlyEnergyProduction { get; private set; }

            /// <summary>
            /// Standard deviation of the annual energy production due to year-to-year variation in kWh.
            /// </summary>
            public double DeviationOYearlyEnergyProduction { get; private set; }

            /// <summary>
            /// Angle of incidence loss in %.
            /// </summary>
            public double AngleOfIncidenceLoss { get; private set; }

            /// <summary>
            /// Spectral loss in %.
            /// </summary>
            public double SpectralLoss { get; private set; }

            /// <summary>
            /// Temperature and irradiance loss in %.
            /// </summary>
            public double TemperatureAndIrradianceLoss { get; private set; }


            /// <summary>
            /// Total loss in %.
            /// </summary>
            public double TotalLoss { get; private set; }

            /// <summary>
            /// Levelized cost of the PV electricity in system_cost_currency/kWh.
            /// </summary>
            public double LevelizedCostOfPVElectricity { get; private set; }

            public static OnGridPVTotals Create(JsonElement element) => new OnGridPVTotals(element);
            private OnGridPVTotals(JsonElement element)
            {
                AverageDailyEnergyProduction = element.GetProperty("E_d").GetDouble();
                AverageMonthlyEnergyProduction = element.GetProperty("E_m").GetDouble();
                AverageYearlyEnergyProduction = element.GetProperty("E_y").GetDouble();

                AverageDailySumOfGlobalIrradiance = element.GetProperty("H(i)_d").GetDouble();
                AverageMonthlySumOfGlobalIrradiance = element.GetProperty("H(i)_m").GetDouble();
                AverageYearlySumOfGlobalIrradiance = element.GetProperty("H(i)_m").GetDouble();

                DeviationOfMonthlyEnergyProduction = element.GetProperty("SD_m").GetDouble();
                DeviationOYearlyEnergyProduction = element.GetProperty("SD_y").GetDouble();

                AngleOfIncidenceLoss = element.GetProperty("l_aoi").GetDouble();
                SpectralLoss = Convert.ToDouble(element.GetProperty("l_spec").GetString());
                TemperatureAndIrradianceLoss = element.GetProperty("l_tg").GetDouble();

                TotalLoss = element.GetProperty("l_total").GetDouble();
            }

            public List<string> Headers
            {
                get
                {
                    var headers = new List<string>()
                    {
                        "E_d",
                        "E_m",
                        "E_y",
                        "H(i)_d",
                        "H(i)_m",
                        "H(i)_y",
                        "SD_m",
                        "SD_y",
                        "l_aoi",
                        "l_spec",
                        "l_tg",
                        "l_total"
                    };

                    return headers;
                }
            }
            public List<string> Descriptions
            {
                get
                {
                    var descriptions = new List<string>()
                    {
                        "Average daily energy production from the given system in kWh/day.",
                        "Average monthly energy production from the given system in kWh/month.",
                        "Average annual energy production from the given system in kWh/year.",
                        "Average daily sum of global irradiation per square meter received by the modules of the given system in kWh/m2/day.",
                        "Average monthly sum of global irradiation per square meter received by the modules of the given system in kWh/m2/month.",
                        "Average yearly sum of global irradiation per square meter received by the modules of the given system in kWh/m2/year.",
                        "Standard deviation of the monthly energy production due to year-to-year variation in kWh.",
                        "Standard deviation of the annual energy production due to year-to-year variation in kWh.",
                        "Angle of incidence loss in %.",
                        "Spectral loss in %.",
                        "Temperature and irradiance loss in %.",
                        "Total loss in %."
                    };

                    return descriptions;
                }

            }
            public List<double> Values
            {
                get
                {
                    var values = new List<double>()
                    {
                        AverageDailyEnergyProduction,
                        AverageMonthlyEnergyProduction,
                        AverageYearlyEnergyProduction,
                        AverageDailySumOfGlobalIrradiance,
                        AverageMonthlySumOfGlobalIrradiance,
                        AverageYearlySumOfGlobalIrradiance,
                        DeviationOfMonthlyEnergyProduction,
                        DeviationOYearlyEnergyProduction,
                        AngleOfIncidenceLoss,
                        SpectralLoss,
                        TemperatureAndIrradianceLoss,
                        TotalLoss
                    };

                    return values;
                }
            }
        }
    }

}