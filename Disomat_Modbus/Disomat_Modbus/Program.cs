namespace Disomat_Modbus
{
    using static System.Formats.Asn1.AsnWriter;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;
    using System;

    using SharpModbusLib;

    class Program
    {
        //      READ ERROR CODE MEANINGS: https://www.simplymodbus.ca/exceptions.htm
        //      GOOD DOCUMENTATION: https://ozeki.hu/p_5873-modbus-function-codes.html

        //      =====Scale Displayed values=====:

        //      GROSS WEIGHT    =   1800, 1801
        //      [ 00 00 00 00 00 06 01 03 07 08 00 02 ]

        //      TARE WEIGHT     =   1802 - 1803
        //      [ 00 00 00 00 00 06 01 03 07 0A 00 02 ]

        //      dW/dT           =   1804 - 1805
        //      [ 00 00 00 00 00 06 01 04 07 0C 00 02 ]

        //      NET WEIGHT      =   1806 - 1807
        //      [ 00 00 00 00 00 06 01 03 07 0E 00 02 ]


        //      =====Scale 1 values=====:

        //      GROSS Weight    =   1840, 1841
        //      [ 00 00 00 00 00 06 01 03 07 30 00 02 ]

        //      TARE WEIGHT     =   1842, 1843
        //      [ 00 00 00 00 00 06 01 03 07 32 00 02 ]

        //      dW/dT           =   1844, 1845
        //      [ 00 00 00 00 00 06 01 03 07 34 00 02 ]

        //      NET WEIGHT      =   1846, 1847
        //      [ 00 1E 00 00 00 06 01 03 07 36 00 02 ]


        //      =====General values=====:

        //      TARE COMMAND    =   16
        //      [ 00 00 00 00 00 06 01 06 00 10 00 01 ]
        //          - Tare() => 16, 1
        //          - Nop() => 16, 0
        //          - acknowledge() => 16, 128
        //          - clearTare() => 16, 2
        //          - setToZero() => 16, 3 

        //      TARE INDICATOR  =   4870
        //          - Descrete input 

        public static void Main(string[] args)
        {

        }
    }
}
