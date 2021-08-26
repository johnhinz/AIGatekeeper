using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.Orchestrator
{
    public record Camera 
    {
        public string Name { get; init; }
        public bool Clip { get; init; }
        public DrawTarget Draw { get; init; }
        public List<Item> Watches { get; init; }
    }
}
