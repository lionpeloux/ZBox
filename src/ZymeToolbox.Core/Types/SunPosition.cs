using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZymeToolbox.Core.Types
{
    public record struct SunPosition
    {
        /// <summary>
        /// Azimuth angle in degree.
        /// 0° = South | 90° = West | -90° = East | +/- 180° = North
        /// </summary>
        public double Azimuth { get; init; }

        /// <summary>
        /// Elevation angle in degree.
        /// 0° = Horizontal | +90° = Zenith
        /// </summary>
        public double Elevation { get; init; }
    }

}
