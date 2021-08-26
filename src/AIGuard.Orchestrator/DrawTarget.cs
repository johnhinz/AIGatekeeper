namespace AIGuard.Orchestrator
{
    public record DrawTarget
    {
        public bool Target { get; init; }
        public int Width { get; init; }
        public bool Confidence { get; init; }
    }
}