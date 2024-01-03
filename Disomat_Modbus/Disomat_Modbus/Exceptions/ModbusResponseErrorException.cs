namespace Disomat_Modbus.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class ModbusResponseErrorException : Exception
    {
        //public ModbusResponseErrorException() { }

        public ModbusResponseErrorException(string errorMessage, byte errorCode, string sentFramme, string recievedFrame) : base($"Modbus responded with error code {errorCode}:  {errorMessage}.") 
        {
            ErrorCode = errorCode;
            SentFrame = sentFramme;
            RecievedFrame = recievedFrame;
        }

        public string SentFrame { get; }
        public string RecievedFrame { get; }
        public byte ErrorCode { get; }
    }
}
