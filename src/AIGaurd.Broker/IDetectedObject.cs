namespace AIGaurd.Broker
{
    public interface IDetectedObject : IWatchedObject
    {
        public int YMin { get; set; }
        public int XMin { get; set; }
        public int YMax { get; set; }
        public int XMax { get; set; }
    }
}
