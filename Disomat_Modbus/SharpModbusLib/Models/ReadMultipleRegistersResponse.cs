namespace SharpModbusLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ReadMultipleRegistersResponse
    {
        public string sentFrame {  get; set; }
        public string responseFrame {  get; set; }
        public int requestedStartAddress {  get; set; }
        public int requestedAmountOfAddresses { get; set; }
        public List<Register> registers { get; set; }
    }
}
