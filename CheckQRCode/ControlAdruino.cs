using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckQRCode
{
    public static class ControlAdruino
    {
       public static string Comport { get; set; }
        public static string DelayStart { get; set; }

        public static string DataCodeLen { get; set; }
        public static string DataCodeCheckSheet { get; set; }

        public static string ComportBarcode { get; set; }
    }
}
