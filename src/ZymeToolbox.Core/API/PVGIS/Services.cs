using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using ZymeToolbox.Core.API.PVGIS.Queries;
using ZymeToolbox.Core.API.PVGIS.Responses;

namespace ZymeToolbox.Core.API.PVGIS
{
    public static class Services
    {
        public static string APIVersion => "5.2";
        public static string APIEntryPoint => "https://re.jrc.ec.europa.eu/api/v5_2/";

        public static OnGridPVResponse GetPVPerformance_OnGrid(OnGridPVQuery query) => OnGridPVResponse.Create(query);
        public static OffGridPVResponse GetPVPerformance_OffGrid(OffGridPVQuery query) => OffGridPVResponse.Create(query);
        public static TrackingPVResponse GetPVPerformance_Tracking(TrackingPVQuery query) => TrackingPVResponse.Create(query);

        public static HourlyRadiationResponse GetHourlyRadiation(HourlyRadiationQuery query) =>  HourlyRadiationResponse.Create(query);
        public static DailyRadiationResponse GetDailyRadiation(DailyRadiationQuery query) => DailyRadiationResponse.Create(query);
        public static MonthlyRadiationResponse GetMonthlyRadiation(MonthlyRadiationQuery query) => MonthlyRadiationResponse.Create(query);
        
        public static HorizonProfileResponse GetHorizonProfile(HorizonProfileQuery query) => HorizonProfileResponse.Create(query);
        public static TMYResponse GetTMY(TMYQuery query) => TMYResponse.Create(query);
        
        public static void WriteEPW(string filePath, double latitude, double longitude, int? startYear, int? endYear, bool? useHorizon, List<double>? userHorizon)
        {
            var query = TMYQuery.Create(latitude, longitude, "epw");
            query.WithYears(startYear, endYear);
            query.WithHorizon(useHorizon, userHorizon);
            var urlQuery = query.Build();

            using (var client = new HttpClient())
            {
                Debug.WriteLine("send request to : " + urlQuery);
                var response = client.GetAsync(urlQuery).Result;
                Debug.WriteLine("response statut : " + response.StatusCode);
                response.EnsureSuccessStatusCode();

                var responseContent = response.Content;
                using (var stream = response.Content.ReadAsStreamAsync().Result)
                using (var fileStream = File.Create(filePath))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
