namespace ZymeToolbox.Core.API.PVGIS
{
    /// <summary>
    /// PV technology types.
    /// </summary>
    public enum PVTechnologyType
    {
        crystSi, 
        CIS, 
        CdTe,
        Unknown
    }

    public class PVModule
    {
        public PVTechnologyType Technology { get; private set; }

        /// <summary>
        /// Nominal (peak) power of the PV module in kW.
        /// </summary>
        public double PeakPower { get; private set; }

        /// <summary>
        /// Sum of system losses in %.
        /// </summary>
        public double SystemLoss { get; private set; }

        private PVModule(double peakPower, double systemLoss)
        {
            Technology = PVTechnologyType.crystSi;
            PeakPower = peakPower;
            SystemLoss = systemLoss;
        }
        private PVModule(double peakPower, double systemLoss, PVTechnologyType technology)
        {
            Technology = technology;
            PeakPower = peakPower;
            SystemLoss = systemLoss;
        }
    }
}
