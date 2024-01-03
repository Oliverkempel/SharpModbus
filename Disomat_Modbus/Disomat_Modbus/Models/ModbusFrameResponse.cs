namespace Disomat_Modbus.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ModbusFrameResponse
    {
        public string sentHexFrame {  get; set; }
        public string responseHexFrame {  get; set; }
        public int amountOfDataBytes { get; set; }
        public byte[] dataBytes { get; set; }

        public byte functionCode { get; set; }
    }
}
