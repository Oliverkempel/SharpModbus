namespace SharpModbusLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Register
    {
        public int address {  get; set; }
        public byte[] bytes { get; set; }
    }
}
