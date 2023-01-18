using HtmlAgilityPack;
using System.Dynamic;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Core.API.PVGIS.Responses
{
    public class TrackingPVResponse : Response
    {
        public PVPerformance Performance_VerticalAxis { get; private set; }
        public PVPerformance Performance_InclinedAxis { get; private set; }
        public PVPerformance Performance_TwoAxis { get; private set; }

        private TrackingPVResponse(string urlQuery) : base(urlQuery)
        {
        }

        internal static TrackingPVResponse Create(TrackingPVQuery query)
        {
            var urlQuery = query.Build();
            var content = new TrackingPVResponse(urlQuery);

            content.Performance_VerticalAxis = PVPerformance.Create("vertical_axis", content.Outputs);
            content.Performance_InclinedAxis = PVPerformance.Create("inclined_axis", content.Outputs);
            content.Performance_TwoAxis = PVPerformance.Create("two_axis", content.Outputs);

            return content;
        }
    }
}