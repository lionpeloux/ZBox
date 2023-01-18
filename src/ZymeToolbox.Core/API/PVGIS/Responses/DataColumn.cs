using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ZymeToolbox.Core.API.PVGIS.Responses
{
    public class DataColumn<T> : List<T>
    {
        public string Header { get; }
        public string Description { get; }
        public string Unit { get; }

        public DataColumn(string header, string description, string unit)
        {
            Header = header;
            Description = description;
            Unit = unit;

        }

        public void AddFromJson(JsonElement element)
        {
            JsonElement jsonProp;
            if (!element.TryGetProperty(Header, out jsonProp)) return;

            switch (typeof(T))
            {
                case var type when type == typeof(int):
                    Add((T)Convert.ChangeType(jsonProp.GetInt32(), typeof(T)));
                    break;
                case var type when type == typeof(double):
                    Add((T)Convert.ChangeType(jsonProp.GetDouble(), typeof(T)));
                    break;
                case var type when type == typeof(bool):
                    Add((T)Convert.ChangeType(jsonProp.GetDouble(), typeof(T)));
                    break;
                case var type when type == typeof(string):
                    Add((T)Convert.ChangeType(jsonProp.GetString(), typeof(T)));
                    break;
                default:
                    throw new NotImplementedException(string.Format("Casting JSON value '{0}' to type '{1}' is not yet implemented.", jsonProp.GetString(), typeof(T)));
            }
        }
    }
}


