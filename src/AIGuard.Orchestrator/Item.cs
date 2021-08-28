namespace AIGuard.Orchestrator
{
    public record Item
    {
        public string Label { get; init; }
        public float Confidence { get; init; }
    }
}
