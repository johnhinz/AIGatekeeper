namespace AIGaurd.Broker
{
    public interface IPrediction
    {
        public bool Success { get; set; }
        public IDetectedObject[] Detections { get; set; }

    }
}
