namespace ZymeToolbox.Core.API.OneBuilding
{
    internal class Placemark
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string StyleUrl { get; set; }
        public string AltitudeMode { get; set; }
        public string Coordinates { get; set; }

        public Placemark()
        {
    
        }

        public override string ToString()
        {
            return $"{Name}, {Coordinates}";
        }
    }
}
