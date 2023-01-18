using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace ZymeToolbox.Core.API.PVGIS.Responses
{

    public abstract class Response
    {
        public string UrlQuery { get; private set; }
        public JsonElement Meta { get; private set; }
        public JsonElement Inputs { get; private set; }
        public JsonElement Outputs { get; private set; }

        public Location Location { get; private set; }

        protected Response(string urlQuery)
        {
            UrlQuery = urlQuery;

            // check is the query is compatible for a json response.
            if (!urlQuery.Contains("outputformat=json"))
                throw new ArgumentException($"PVGIS Response is meant to work only with a json payload. Provide a query with 'outputformat=json'.");
            
            using (var client = new HttpClient())
            {
                Debug.WriteLine("send request to : " + UrlQuery);
                var response = client.GetAsync(UrlQuery).Result;
                Debug.WriteLine("response statut : " + response.StatusCode);
                response.EnsureSuccessStatusCode();

                var responseContent = response.Content;
                using (var doc = JsonDocument.Parse(response.Content.ReadAsStringAsync().Result))
                {
                    var root = doc.RootElement;
                    Inputs = root.GetProperty("inputs").Clone();
                    Outputs = root.GetProperty("outputs").Clone();
                    Meta = root.GetProperty("meta").Clone();
                    Location = Location.Create(Inputs.GetProperty("location"));
                }
            }
        }

        public MeteoData GetMeteoData()
        {
            JsonElement val;
            if (Inputs.TryGetProperty("meteo_data", out val))
            {
                return MeteoData.Create(val);
            }
            else
            {
                throw new ArgumentException("'meteo_data' does not exist under the 'inputs' node");
            }
        }
    }
}
