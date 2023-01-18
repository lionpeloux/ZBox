using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Xml.Linq;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Core.API.PVGIS.Responses
{
    public class OffGridPVResponse : Response
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
        /// Average daily energy not captured from the given system in kWh/day.
        /// </summary>
        public DataColumn<double> AverageDailyEnergyProductionLost { get; private set; }

        /// <summary>
        /// Percentage of days when the battery became full in %.
        /// </summary>
        public DataColumn<double> FullBatteryRatio { get; private set; }

        /// <summary>
        /// Percentage of days when the battery became empty in %.
        /// </summary>
        public DataColumn<double> EmptyBatteryRatio { get; private set; }
        
        /// <summary>
        /// Charge state at the end of each hour in %.
        /// </summary>
        public DataColumn<double> Histo_ChargeState_Min { get; private set; }

        /// <summary>
        /// Charge state at the end of each hour in %.
        /// </summary>
        public DataColumn<double> Histo_ChargeState_Max { get; private set; }

        /// <summary>
        /// Percentage of days with the charge state in the range [Histo_ChargeState_Min, Histo_ChargeState_Max] in %.
        /// </summary>
        public DataColumn<double> Histo_ChargeState_Percentil { get; private set; }

        /// <summary>
        /// Totals.
        /// </summary>
        public OffGridPVTotals Totals { get; private set; }

        private OffGridPVResponse(string urlQuery) : base(urlQuery)
        {
            Month = new DataColumn<int>("month", "Hourly time stamps.", "?");
            AverageDailyEnergyProduction = new DataColumn<double>("E_d", "Average daily energy production from the given system.", "kWh/day");
            AverageDailyEnergyProductionLost = new DataColumn<double>("E_lost_d", "Average daily energy not captured from the given system.", "kWh/day");
            FullBatteryRatio = new DataColumn<double>("f_f", "Percentage of days when the battery became full.", "%");
            EmptyBatteryRatio = new DataColumn<double>("f_e", "Percentage of days when the battery became empty.", "%");

            Histo_ChargeState_Min = new DataColumn<double>("CS_min", "Charge state at the end of each hour.", "%");
            Histo_ChargeState_Max = new DataColumn<double>("CS_max", "Charge state at the end of each hour.", "%");
            Histo_ChargeState_Percentil = new DataColumn<double>("f_CS", "Percentage of days with the charge state in the range [Histo_ChargeState_Min, Histo_ChargeState_Max].", "%");
        }

        internal static OffGridPVResponse Create(OffGridPVQuery query)
        {
            var urlQuery = query.Build();
            var content = new OffGridPVResponse(urlQuery);

            foreach (var item in content.Outputs.GetProperty("monthly").EnumerateArray())
            {
                content.Month.AddFromJson(item);
                content.AverageDailyEnergyProduction.AddFromJson(item);
                content.AverageDailyEnergyProductionLost.AddFromJson(item);
                content.FullBatteryRatio.AddFromJson(item);
                content.EmptyBatteryRatio.AddFromJson(item);
            }

            foreach (var item in content.Outputs.GetProperty("histogram").EnumerateArray())
            {
                content.Histo_ChargeState_Min.AddFromJson(item);
                content.Histo_ChargeState_Max.AddFromJson(item);
                content.Histo_ChargeState_Percentil.AddFromJson(item);
            }

            var totals = content.Outputs.GetProperty("totals");
            content.Totals = OffGridPVTotals.Create(totals);

            return content;
        }

        public record OffGridPVTotals
        {
            /// <summary>
            /// Number of days used for the calculation.
            /// </summary>
            public double NumberOfDays { get; init; }

            /// <summary>
            /// Percentage of days when the battery became full in %.
            /// </summary>
            public double FullBatteryDaysRatio { get; init; }

            /// <summary>
            /// Percentage of days when the battery became full in %.
            /// </summary>
            public double EmptyBatteryDaysRatio { get; init; }

            /// <summary>
            /// Average energy not captured per day in Wh/d.
            /// </summary>
            public double AverageDailyEnergyLost { get; init; }

            /// <summary>
            /// Average energy missing per day in Wh/d.
            /// </summary>
            public double AverageDailyMissingEnergy { get; init; }

            public static OffGridPVTotals Create(JsonElement element) => new OffGridPVTotals(element);
            private OffGridPVTotals(JsonElement element)
            {
                NumberOfDays = element.GetProperty("d_total").GetDouble();
                FullBatteryDaysRatio = element.GetProperty("f_f").GetDouble();
                EmptyBatteryDaysRatio = element.GetProperty("f_e").GetDouble();

                AverageDailyEnergyLost = element.GetProperty("E_lost").GetDouble();
                AverageDailyMissingEnergy = element.GetProperty("E_miss").GetDouble();
            }

            public List<string> Headers
            {
                get
                {
                    var headers = new List<string>()
                    {
                        "d_total",
                        "f_f",
                        "f_e",
                        "E_lost",
                        "HE_miss"
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
                        "Number of days used for the calculation.",
                        "Percentage of days when the battery became full in %.",
                        "Percentage of days when the battery became empty in %.",
                        "Average energy not captured per day in Wh/d.",
                        "Average energy missing per day in Wh/d."
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
                        NumberOfDays,
                        FullBatteryDaysRatio,
                        EmptyBatteryDaysRatio,
                        AverageDailyEnergyLost,
                        AverageDailyMissingEnergy
                    };
                    return values;
                }
            }
        }
    }

}