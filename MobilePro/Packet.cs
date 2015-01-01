using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobilePro
{
    public class Packet
    {
        public int id { get; set; }
        public int source { get; set; }
        public int dest { get; set; }
        public int start { get; set; }
        public int cnt { get; set; }
    }
}
