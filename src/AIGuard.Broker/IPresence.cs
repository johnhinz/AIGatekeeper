using System;
using System.Collections.Generic;
using System.Text;

namespace AIGuard.Broker
{
    interface IPresence
    {
        public string MAC { get; set; }
        public string IP { get; set; }
        public string Radio { get; set; }
        public string AccessPoint { get; set; }
        public string SSID { get; set; }
        public string SNR { get; set; }
        public string DeviceName { get; set; }
        public int CCQ { get; set; }
        public string Rate { get; set; }
        public int Down { get; set; }
        public int Up { get; set; }
        public string ActiveTime { get; set; }
    }
}
