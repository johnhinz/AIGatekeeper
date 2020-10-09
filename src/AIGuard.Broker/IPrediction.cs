namespace AIGuard.Broker
{
    public interface IPrediction
    {
        public bool Success { get; set; }
        public string Base64Image { get; set; }
        public string FileName { get; set; }
        public IDetectedObject[] Detections { get; set; }

    }
}
