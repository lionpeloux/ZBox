using System;
using System.Collections.Generic;
using System.Text;

namespace ZymeToolbox.Core.API.OneBuilding
{
    public sealed class Index
    {
        private static Index _instance;
        public List<WeatherData> DataPoints;

        private Index() 
        { 

        }
        public static Index Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Index();
                }
                return _instance;
            }
        }

    }
}
