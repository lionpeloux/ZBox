using HtmlAgilityPack;
using System.Dynamic;
using ZymeToolbox.Core.API.PVGIS.Queries;

namespace ZymeToolbox.Core.API.PVGIS.Responses
{
    public class OnGridPVResponse: Response
    {
        public PVPerformance Performance_Fixed { get; private set; }

        private OnGridPVResponse(string urlQuery) : base(urlQuery)
        {
        }

        internal static OnGridPVResponse Create(OnGridPVQuery query)
        {
            var urlQuery = query.Build();
            var content = new OnGridPVResponse(urlQuery);

            content.Performance_Fixed = PVPerformance.Create("fixed", content.Outputs);
            return content;
        }
    }
}