namespace Disomat_Modbus.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WriteSuccessResponse
    {
        public string sentFrame { get; set; }
        public string responseFrame { get; set; }
        public int address { get; set; }
    }
}
