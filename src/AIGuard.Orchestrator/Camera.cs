using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.Orchestrator
{
    public class Camera
    {
        public string Name { get; set; }
        public bool Clip { get; set; }
        public bool DrawTarget { get; set; }
        public bool DrawConfidence { get; set; }
        public List<Item> Watches { get; set; }
    }
}
