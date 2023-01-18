namespace ZymeToolbox.Core.Types
{

    /// <summary>
    /// Ladybug Location
    /// </summary>
    /// <see cref=">https://github.com/ladybug-tools/ladybug/blob/master/ladybug/location.p"/>
    public class Location
    {
        /// <summary>
        /// Nale of the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Optional state in which the city is located.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Name of the country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Location elevation above see level in meter.
        /// </summary>
        public double Elevation { get; set; }

        /// <summary>
        /// Location latitude in degrees.
        /// Between -90° and 90°.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Location longitude in degrees.
        /// Between -180° (west) and 180° (east).
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Get a number between -180 and +180 for the meridian west of Greenwich.
        /// </summary>
        public double Meridian { get; set; }

        /// <summary>
        /// Time zone between -12 hours (west) and +14 hours (east).
        /// 
        /// </summary>
        public double TimeZone { get; set; }

        /// <summary>
        /// ID of the location if the location is representing a weather station (WMO).
        /// </summary>
        public int StationID { get; set; }

        /// <summary>
        /// Source of data (e.g. TMY, TMY3).
        /// </summary>
        public string Source { get; set; }

        public override string ToString()
        {
            return $"{City}, lat:{Latitude:F2}, lon:{Longitude:F2}, tz:{TimeZone:F1}, elev:{Elevation:F2}";
        }

    }
}
