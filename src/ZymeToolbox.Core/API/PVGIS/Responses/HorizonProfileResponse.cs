using ZymeToolbox.Core.API.PVGIS.Queries;
using ZymeToolbox.Core.Types;

namespace ZymeToolbox.Core.API.PVGIS.Responses
{
    public class HorizonProfileResponse : Response
    {
        /// <summary>
        /// A list of SunPosition describing the horizon profile (June 21th).
        /// </summary>
        public DataColumn<SunPosition> HorizonProfile { get; private set; }

        /// <summary>
        /// A list of SunPosition describing the winter solstice (December 21th).
        /// </summary>
        public DataColumn<SunPosition> WinterSolstice { get; private set; }

        /// <summary>
        /// A list of SunPosition describing the summer solstice (June 21th).
        /// </summary>
        public DataColumn<SunPosition> SummerSolstice { get; private set; }

        private HorizonProfileResponse(string urlQuery) : base(urlQuery)
        {
            HorizonProfile = new DataColumn<SunPosition>("(A, H_hor)", "Horizon Profile", "(Degree, Degree)");
            WinterSolstice = new DataColumn<SunPosition>("(A_sun(w), H_sun(w))", "Winter Solstice (December 21th)", "(Degree, Degree)");
            SummerSolstice = new DataColumn<SunPosition>("(A_sun(s), H_sun(s))", "Summer Solstice 'June 21th", "(Degree, Degree)");
        }

        internal static HorizonProfileResponse Create(HorizonProfileQuery query)
        {
            var urlQuery = query.Build();
            var content = new HorizonProfileResponse(urlQuery);

            var hp = content.Outputs.GetProperty("horizon_profile");
            var ws = content.Outputs.GetProperty("winter_solstice");
            var ss = content.Outputs.GetProperty("summer_solstice");

            foreach (var item in hp.EnumerateArray())
            {
                content.HorizonProfile.Add(new SunPosition()
                {
                    Azimuth = item.GetProperty("A").GetDouble(),
                    Elevation = item.GetProperty("H_hor").GetDouble(),
                });
            }

            foreach (var item in ws.EnumerateArray())
            {
                content.WinterSolstice.Add(new SunPosition()
                {
                    Azimuth = item.GetProperty("A_sun(w)").GetDouble(),
                    Elevation = item.GetProperty("H_sun(w)").GetDouble(),
                });
            }

            foreach (var item in ss.EnumerateArray())
            {
                content.SummerSolstice.Add(new SunPosition()
                {
                    Azimuth = item.GetProperty("A_sun(s)").GetDouble(),
                    Elevation = item.GetProperty("H_sun(s)").GetDouble(),
                });
            }

            return content;
        }

    }



}