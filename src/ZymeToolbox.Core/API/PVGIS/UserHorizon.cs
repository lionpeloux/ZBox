using System;
using System.Collections.Generic;
using System.Linq;
using ZymeToolbox.Core.Types;

namespace ZymeToolbox.Core.API.PVGIS
{
    /// <summary>
    /// A list of equidistant SunPosition describing a user defined horizon profile.
    /// For instance, starting at north and moving clockwise the series '0,10,20,30,40,15,25,5' would mean the horizon height is 0° for north, 10° for north-east, 20° for east, 30° for south-east, etc.
    /// </summary>
    public class UserHorizon
    {
        private SunPosition[] _sunPositions;
        public int Length => _sunPositions.Length;
        public SunPosition this[int index]
        {
            get { return _sunPositions[index]; }
        }

        private UserHorizon(SunPosition[] sunPositions)
        { 
            _sunPositions = sunPositions;
        }
        public static UserHorizon Create(IEnumerable<double> elevations)
        {
            var count = elevations.Count();

            if (count == 1)
            {
                return new UserHorizon(new SunPosition[2] {
                    new SunPosition() { Azimuth = 0, Elevation =elevations.First() },
                    new SunPosition() { Azimuth = 360, Elevation =elevations.First() },
                    });
            }

            var deltaAzimuth = 360.0 / (count - 1);

            var i = 0;
            var sunPositions = new SunPosition[count];
            foreach (var e in elevations)
            {
                sunPositions[i] = new SunPosition() { Azimuth = i * deltaAzimuth, Elevation = e };
                i++;
            }
            return new UserHorizon(sunPositions); 
        }

        public string ToQuery()
        {
            return string.Join(",", _sunPositions.Select(sp => Math.Round(sp.Elevation, 3).ToString()));
        }
    }
}
