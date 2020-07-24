using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.Service
{
    public class Camera
    {
        public string Name { get; set; }
        public bool Clip { get; set; }
        public List<Item> Watches { get; set; }
    }
}
