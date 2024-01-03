
namespace Disomat_Modbus.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class DiscreteInputOutput
    {
        public int address { get; set; }
        public bool value { get; set; }
    }
}
