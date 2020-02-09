using System;

namespace AIGuard.Repository
{
    public class ItemEntity
    {
        public Guid Id { get; set; }
        public Guid FileId { get; set; }
        public float Confidence { get; set; }
        public string Label { get; set; }
        public int YMin { get; set; }
        public int XMin { get; set; }
        public int YMax { get; set; }
        public int XMax { get; set; }
    }
}