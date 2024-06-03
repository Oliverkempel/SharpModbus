namespace LibraryDEMO
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
            Modbus ModbusClient = null;
            bool ConnSuccsselful = false;
            do
            {
                try
                {
                    //TersusModbus ModbusClient = new TersusModbus("172.31.63.217", 502);
                    //ModbusClient = new Modbus("127.0.0.1", 502);
                    //ModbusClient = new Modbus("10.203.20.200", 502);
                    ModbusClient = new Modbus("172.31.63.220", 502);
                    ConnSuccsselful = true;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Connection error: {ex.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (ConnSuccsselful != true)
                {
                    Console.WriteLine("Press any key to try again");
                    Console.ReadLine();
                }
            } while (!ConnSuccsselful);



            Console.WriteLine("================================================");
            Console.WriteLine("                 SHARP MODBUS");
            Console.WriteLine("================================================");
            Console.WriteLine("");
            Console.WriteLine("Select one of the following actions:");
            Console.WriteLine("1: Read holding registers (IEEE Floats)");
            Console.WriteLine("2: Read Input registers (Raw)");
            Console.WriteLine("3: Read Coils (bool)");
            Console.WriteLine("4: Read discrete inputs (Bool)");

            Console.WriteLine("5: Write single coil");
            Console.WriteLine("6: Write single holding register");
            Console.WriteLine("7: Write multiple coils");
            Console.WriteLine("8: Write multiple holding registers");
            Console.WriteLine("");

            bool selectionValid = false;
            int choice = 1;
            do
            {
                try
                {
                    Console.Write("Input Selection    :    ");
                    choice = Int32.Parse(Console.ReadLine());
                    selectionValid = true;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Invalid selection: {ex.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (choice > 8 || choice <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Selection out of range");
                    Console.ForegroundColor = ConsoleColor.White;
                    selectionValid = false;
                }

            } while (!selectionValid);

            Console.WriteLine();

            // 01 Read holding registers (IEEE Floats)
            if (choice == 1)
            {
                while (true)
                {
                    Int16 inputaddress = 0;
                    Int16 inputaddressamount = 0;

                    bool valid = false;
                    do
                    {
                        try
                        {
                            Console.Write("Register address  :  ");
                            inputaddress = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);

                    valid = false;
                    do
                    {
                        try
                        {

                            Console.Write("Address amount    :  ");
                            inputaddressamount = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        if (inputaddressamount % 2 != 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Please input value divisiable by 2");
                            Console.ForegroundColor = ConsoleColor.White;

                            valid = false;
                        }

                    } while (!valid);


                    SharpModbusLib.Models.ReadMultipleRegistersResponse response = null;
                    try
                    {
                        response = ModbusClient.readMultipleHoldingRegisters(inputaddress, inputaddressamount);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: {ex.Message}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (response != null)
                    {
                        for (int i = 0; i < response.registers.Count(); i += 2)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Register {response.registers[i].address} & {response.registers[i + 1].address}    =    {ModbusClient.convertRegistersToFloat(response.registers[i], response.registers[i + 1])}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    Console.WriteLine("\n");
                    //Console.ReadKey();

                    //inputaddress = 0; inputaddressamount = 0;
                }
            }

            // 02 Read Input Registers (Int16)
            else if (choice == 2)
            {


                while (true)
                {
                    Int16 inputaddress = 0;
                    Int16 inputaddressamount = 0;

                    bool valid = false;
                    do
                    {
                        try
                        {
                            Console.Write("Register address  :  ");
                            inputaddress = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);

                    valid = false;
                    do
                    {
                        try
                        {

                            Console.Write("Address amount    :  ");
                            inputaddressamount = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);


                    SharpModbusLib.Models.ReadMultipleRegistersResponse response = null;
                    try
                    {
                        response = ModbusClient.readInputRegisters(inputaddress, inputaddressamount);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: {ex.Message}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (response != null)
                    {
                        foreach (SharpModbusLib.Models.Register reg in response.registers)
                        {
                            //Reversed to become small Endian
                            Array.Reverse(reg.bytes);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Input register {reg.address}    =     {BitConverter.ToInt16(reg.bytes)}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    Console.WriteLine("\n");


                }
            }

            // 03 Read Coils (Bool)
            else if (choice == 3)
            {
                while (true)
                {
                    Int16 inputaddress = 0;
                    Int16 inputaddressamount = 0;
                    bool valid = false;
                    do
                    {
                        try
                        {
                            Console.Write("Register address  :  ");
                            inputaddress = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);

                    valid = false;
                    do
                    {
                        try
                        {

                            Console.Write("Address amount    :  ");
                            inputaddressamount = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);



                    SharpModbusLib.Models.ReadDiscreteInputsResponse response = null;
                    try
                    {
                        response = ModbusClient.readCoils(inputaddress, inputaddressamount);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: {ex.Message}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (response != null)
                    {
                        foreach (SharpModbusLib.Models.DiscreteInputOutput discInp in response.DiscreteInputs)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Coil {discInp.address}    =    {discInp.value}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    Console.WriteLine("\n");
                }
            }

            // 04 Read Discrete inputs (Bool)
            else if (choice == 4)
            {
                while (true)
                {


                    Int16 inputaddress = 0;
                    Int16 inputaddressamount = 0;
                    bool valid = false;
                    do
                    {
                        try
                        {
                            Console.Write("Register address  :  ");
                            inputaddress = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);

                    valid = false;
                    do
                    {
                        try
                        {

                            Console.Write("Address amount    :  ");
                            inputaddressamount = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);



                    SharpModbusLib.Models.ReadDiscreteInputsResponse response = null;
                    try
                    {
                        response = ModbusClient.readDiscreteInputs(inputaddress, inputaddressamount);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: {ex.Message}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (response != null)
                    {
                        foreach (SharpModbusLib.Models.DiscreteInputOutput discInp in response.DiscreteInputs)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Discrete input {discInp.address}    =    {discInp.value}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }

                    Console.WriteLine("\n");
                }
            }

            // 05 Write single coil
            else if (choice == 5)
            {
                while (true)
                {

                    Int16 inputaddress = 0;
                    bool value = false;
                    bool valid = false;
                    do
                    {
                        try
                        {
                            Console.Write("Register address       :  ");
                            inputaddress = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);

                    valid = false;
                    do
                    {
                        try
                        {

                            Console.Write("Value (true/false)    :  ");
                            value = Boolean.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);



                    SharpModbusLib.Models.WriteSuccessResponse response = null;
                    try
                    {
                        response = ModbusClient.writeSingleCoil(inputaddress, value);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: {ex.Message}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (response != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"The coil was successfully written!");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine("\n");
                }
            }

            // 06 Write single holding register
            else if (choice == 6)
            {
                while (true)
                {

                    Int16 inputaddress = 0;
                    short value = 0;
                    bool valid = false;
                    do
                    {
                        try
                        {
                            Console.Write("Register address     :  ");
                            inputaddress = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);

                    valid = false;
                    do
                    {
                        try
                        {

                            Console.Write("Value (decimal)      :  ");
                            value = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);



                    SharpModbusLib.Models.WriteSuccessResponse response = null;
                    try
                    {
                        response = ModbusClient.writeSingleHoldingRegister(inputaddress, value);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: {ex.Message}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (response != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"The holding register was successfully written!");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine("\n");
                }
            }

            // 07 Write Multiple coils
            else if (choice == 7)
            {
                while (true)
                {

                    Int16 inputaddress = 0;
                    bool valid = false;
                    do
                    {
                        try
                        {
                            Console.Write("Start coil address                   :  ");
                            inputaddress = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);

                    valid = false;
                    bool[] boolArray = new bool[0];
                    do
                    {
                        try
                        {

                            Console.Write("Values (true/false comma separated)  :  ");
                            string inputvalue = Console.ReadLine();
                            inputvalue.Replace(" ", "");

                            string[] parts = inputvalue.Split(",");

                            Array.Resize(ref boolArray, parts.Length);
                            //bool[] array = new bool[parts.Length];


                            for (int i = 0; i < parts.Length; i++)
                            {
                                boolArray[i] = Boolean.Parse(parts[i]);
                            }

                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);



                    SharpModbusLib.Models.WriteSuccessResponse? response = null;
                    try
                    {
                        response = ModbusClient.writeMultipleCoils(inputaddress, boolArray);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: {ex.Message}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (response != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"The coils have succesfully been written!");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine("\n");


                    //WriteSuccessResponse? response = ModbusClient.writeMultipleCoils(19, new bool[] {true, true, false, true, true, false, true, true, true});

                }
            }

            // 08 Write Multiple holding registers
            else if (choice == 8)
            {
                while (true)
                {

                    Int16 inputaddress = 0;
                    bool valid = false;
                    do
                    {
                        try
                        {
                            Console.Write("Start coil address           :  ");
                            inputaddress = Int16.Parse(Console.ReadLine());
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);

                    valid = false;
                    short[] valueArray = new short[0];
                    do
                    {
                        try
                        {

                            Console.Write("Values (comma separated)     :  ");
                            string inputvalue = Console.ReadLine();
                            inputvalue.Replace(" ", "");

                            string[] parts = inputvalue.Split(",");

                            Array.Resize(ref valueArray, parts.Length);
                            //bool[] array = new bool[parts.Length];


                            for (int i = 0; i < parts.Length; i++)
                            {
                                valueArray[i] = Int16.Parse(parts[i]);
                            }

                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"Parse error: {ex.Message}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                    } while (!valid);



                    SharpModbusLib.Models.WriteSuccessResponse? response = null;
                    try
                    {
                        response = ModbusClient.writeMultipleHoldingRegisters(inputaddress, valueArray);
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR: {ex.Message}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    if (response != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"The holding registers have succesfully been written!");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.WriteLine("\n");



                    //ModbusClient.writeMultipleHoldingRegisters(1804, new short[] {1212, 1313, 1414});
                }
            }
        }
    }
}
