namespace ZymeToolbox.Core.API.PVGIS
{
    /// <summary>
    /// Types of suntracking.
    /// </summary>
    public enum PVTrackingType
    {
        /// <summary>
        /// Fixed POA
        /// </summary>
        Fixed = 0,

        /// <summary>
        /// Single horizontal axis aligned north-south, 
        /// </summary>
        SingleHorizontalAxis_NorthSouth = 1,

        /// <summary>
        /// Two-axis tracking
        /// </summary>
        TwoAxisTracking = 2,

        /// <summary>
        /// Vertical axis tracking
        /// </summary>
        VerticalAxis = 3,

        /// <summary>
        /// Single horizontal axis aligned east-west
        /// </summary>
        SingleHorizontalAxis_EastWest = 4,

        /// <summary>
        /// Single inclined axis aligned north-south
        /// </summary>
        SingleInclinedAxis_NorthSouth = 5,
    }

    /// <summary>
    /// Types of mounting of the PV modules.
    /// </summary>
    public enum PVMountingType
    {
        /// <summary>
        /// Free-standing.
        /// </summary>
        Free = 0,

        /// <summary>
        /// Building-integrated.
        /// </summary>
        Building = 1
    }

    public class PVMounting
    {
        /// <summary>
        /// Type of suntracking used.
        /// </summary>
        public PVTrackingType Tracking { get; private set; } = PVTrackingType.Fixed;

        /// <summary>
        /// Type of mounting used.
        /// </summary>
        public PVMountingType Mounting { get; private set; } = PVMountingType.Free;

        /// <summary>
        /// Inclination angle of the POA from the horizontal plane in degree (0° = hozirontal | 90° = vertical).
        /// Not relevant for 2-axis tracking POA. 
        /// </summary>
        public double? Inclination { get; private set; }

        /// <summary>
        /// Azimuth angle of the fixed POA in degree (0° = South, 90° = West, -90° = East). 
        /// Not relevant for tracking POA.
        /// </summary>
        public double? Azimuth { get; private set; }
    }

}
