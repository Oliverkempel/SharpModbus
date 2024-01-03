namespace Disomat_Modbus
{
    using Disomat_Modbus.Exceptions;
    using Disomat_Modbus.Models;
    using Microsoft.Win32;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Net.Sockets;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class TersusModbus
    {
        public string ipAdress { get; set; }
        public int port { get; set; }
        public byte slaveID { get; set; }

        public TcpClient client = new TcpClient();

        public TersusModbus(string IpAdress, int Port, byte SlaveID = 01) 
        {
            ipAdress = IpAdress;
            port = Port;
            slaveID = SlaveID;
            connect();
        }

        public void connect()
        {
            try
            {
                client.Connect(ipAdress, port);
            } 
            catch(Exception ex)
            {
                throw new Exception($"Could not establish connection to the Modbus Slave.", ex);
            }
        }

        public void disconnect()
        {
            try
            {
                client.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not Close connection to slave", ex);
            }
        }

        // FUNCTION CODE: 01 - Read Coils
        public ReadDiscreteInputsResponse readCoils(short startReg, short amountOfRegs)
        {
            short frameLength = 6;
            byte functionCode = 01;

            byte[] frameBytes = buildFrame(startReg, amountOfRegs, frameLength, slaveID, functionCode);

            //Inits response object
            ModbusFrameResponse response = new ModbusFrameResponse();

            //Sends frame and recieves response
            response = sendFrame(frameBytes, amountOfRegs);

            BitArray responseArray = new BitArray(response.dataBytes);

            List<DiscreteInputOutput> result = new List<DiscreteInputOutput>();

            for (int i = 0; i < amountOfRegs; i++)
            {
                result.Add(new DiscreteInputOutput { address = startReg + i, value = responseArray[i] });
            }

            ReadDiscreteInputsResponse Response = new ReadDiscreteInputsResponse { DiscreteInputs = result, requestedAmountOfAddresses = amountOfRegs, requestedStartAddress = startReg, responseFrame = response.responseHexFrame, sentFrame = response.sentHexFrame };

            return Response;
        }

        // FUNCTION CODE: 02 - Read discrete inputs
        public ReadDiscreteInputsResponse readDiscreteInputs(short startReg, short amountOfRegs)
        {
            short frameLength = 6;
            byte functionCode = 02;

            byte[] frameBytes = buildFrame(startReg, amountOfRegs, frameLength, slaveID, functionCode);

            //Inits response object
            ModbusFrameResponse response = new ModbusFrameResponse();

            //Sends frame and recieves response
            response = sendFrame(frameBytes, amountOfRegs);

            BitArray responseArray = new BitArray(response.dataBytes);

            List<DiscreteInputOutput> result = new List<DiscreteInputOutput>();

            for (int i = 0; i < amountOfRegs; i++)
            {
                result.Add(new DiscreteInputOutput { address = startReg + i, value = responseArray[i] });
            }

            ReadDiscreteInputsResponse Response = new ReadDiscreteInputsResponse { DiscreteInputs = result, requestedAmountOfAddresses = amountOfRegs, requestedStartAddress = startReg, responseFrame = response.responseHexFrame, sentFrame = response.sentHexFrame };

            return Response;
        }

        // FUNCTION CODE: 03 - Read multiple Holding Registers
        public ReadMultipleRegistersResponse readMultipleHoldingRegisters(short startReg, short amountOfRegs)
        {
            //Hard coded as this will always be the same length
            short frameLength = 6;
            //Hard coded function code for reading multiple holding registers;
            byte functionCode = 03;

            byte[] frameBytes = buildFrame(startReg, amountOfRegs, frameLength, slaveID, functionCode);

            //Inits response object
            ModbusFrameResponse response = new ModbusFrameResponse();
            
            //Sends frame and recieves response
            response = sendFrame(frameBytes, amountOfRegs);

            //Inits register list
            List<Register> Registers = new List<Register>();

            //Add registers - One register = 2 bytes           
            int curReg = startReg;
            for (int i = 0; i < response.dataBytes.Length; i = i + 2)
            {
                Registers.Add(new Register { bytes = new byte[] { response.dataBytes[i], response.dataBytes[i + 1] }, address = curReg });
                curReg += 1;
            }

            //returns response
            return new ReadMultipleRegistersResponse {registers = Registers, requestedAmountOfAddresses = amountOfRegs, requestedStartAddress = startReg, responseFrame = response.responseHexFrame, sentFrame = response.sentHexFrame };
        }

        // FUNCTION CODE: 04 - Read multiple input registers
        public ReadMultipleRegistersResponse readInputRegisters(short startReg, short amountOfRegs)
        {
            short frameLength = 6;
            byte functionCode = 04;

            byte[] frameBytes = buildFrame(startReg, amountOfRegs, frameLength, slaveID, functionCode);

            ModbusFrameResponse response = new ModbusFrameResponse();

            response = sendFrame(frameBytes, amountOfRegs);

            //Inits register list
            List<Register> Registers = new List<Register>();

            //Add registers - One register = 2 bytes           
            int curReg = startReg;
            for (int i = 0; i < response.dataBytes.Length; i = i + 2)
            {
                Registers.Add(new Register { bytes = new byte[] { response.dataBytes[i], response.dataBytes[i + 1] }, address = curReg });
                curReg += 1;
            }

            return new ReadMultipleRegistersResponse { registers = Registers, requestedAmountOfAddresses = amountOfRegs, requestedStartAddress = startReg, responseFrame = response.responseHexFrame, sentFrame = response.sentHexFrame };
        }

        // FUNCTION CODE: 05 - Write single coil
        public WriteSuccessResponse writeSingleCoil(short regAddress, bool value)
        {
            short frameLength = 6;
            byte functionCode = 05;
            byte[] writeValue = new byte[2];

            if(value)
            {
                writeValue = new byte[]{ 0xff, 0x00 };
            } else if(!value)
            {
                writeValue = new byte[] { 0x00, 0x00 };
            }

            byte[] frameBytes = buildWriteSingleFrame(regAddress, writeValue, frameLength, slaveID, functionCode);

            //Inits response object
            ModbusFrameResponse response = new ModbusFrameResponse();

            //Sends frame and recieves response
            response = sendFrame(frameBytes, 1);

            return new WriteSuccessResponse {address = regAddress, responseFrame = response.responseHexFrame, sentFrame = response.sentHexFrame };
        }

        // FUNCTION CODE: 06 - Write single holding register
        public WriteSuccessResponse writeSingleHoldingRegister(short regAddress, short value)
        {
            short frameLength = 6;
            byte functionCode = 06;

            //Flip to big Endian (i think)
            byte[] writebytes = BitConverter.GetBytes(value);
            Array.Reverse(writebytes);

            byte[] frameBytes = buildWriteSingleFrame(regAddress, writebytes, frameLength, slaveID, functionCode);

            //Inits response object
            ModbusFrameResponse response = new ModbusFrameResponse();

            //Sends frame and recieves response
            response = sendFrame(frameBytes, 1);

            return new WriteSuccessResponse { address = regAddress, responseFrame = response.responseHexFrame, sentFrame = response.sentHexFrame };
        }

        // FUNCTION CODE: 15 - Write multiple coils
        public WriteSuccessResponse writeMultipleCoils(short startReg, bool[] values)
        {
            short frameLength = 6;
            byte functionCode = 15;

            //Flip to big Endian (i think)
            //byte[] writebytes = BitConverter.GetBytes(value);
            //Array.Reverse(writebytes);

            int amountOfBytesRequired = (values.Length + 7) / 8;


            byte[] bytes = new byte[amountOfBytesRequired];

            BitArray valBitArr = new BitArray(values);

            valBitArr.CopyTo(bytes, 0);


            byte[] frameBytes = buildWriteMultipleFrame(startReg, bytes, Convert.ToInt16(values.Length), slaveID, functionCode);

            //Inits response object
            ModbusFrameResponse response = new ModbusFrameResponse();

            //Sends frame and recieves response
            response = sendFrame(frameBytes, Convert.ToInt16(values.Length));

            return new WriteSuccessResponse { address = startReg, responseFrame = response.responseHexFrame, sentFrame = response.sentHexFrame };
        }

        public byte[] buildFrame(short startReg, short amountOfRegs, short frameLength, byte slaveID, byte functionCode)
        {
            //Is TransactionID and ProtocolID can be left at zero
            byte[] startBytes = { 0x00, 0x00, 0x00, 0x00 };
            Array.Reverse(startBytes);

            byte[] lengthBytes = BitConverter.GetBytes(frameLength);
            Array.Reverse(lengthBytes);

            byte[] startAddressBytes = BitConverter.GetBytes(startReg);
            Array.Reverse(startAddressBytes);

            byte[] registerQuantityBytes = BitConverter.GetBytes(amountOfRegs);
            Array.Reverse(registerQuantityBytes);

            byte[] frameBytes = startBytes.Concat(lengthBytes).Concat(new byte[] { slaveID }).Concat(new byte[] { functionCode }).Concat(startAddressBytes).Concat(registerQuantityBytes).ToArray();

            return frameBytes;
        }

        public byte[] buildWriteSingleFrame(short regAddress, byte[] writeValue, short frameLength, byte slaveID, byte functionCode)
        {
            //Is TransactionID and ProtocolID can be left at zero
            byte[] startBytes = { 0x00, 0x00, 0x00, 0x00 };
            Array.Reverse(startBytes);

            byte[] lengthBytes = BitConverter.GetBytes(frameLength);
            Array.Reverse(lengthBytes);

            byte[] regAddressBytes = BitConverter.GetBytes(regAddress);
            Array.Reverse(regAddressBytes);

            //byte[] writeValueBytes = BitConverter.GetBytes(writeValue);
            //Array.Reverse(writeValueBytes);

            byte[] frameBytes = startBytes.Concat(lengthBytes).Concat(new byte[] { slaveID }).Concat(new byte[] { functionCode }).Concat(regAddressBytes).Concat(writeValue).ToArray();

            return frameBytes;
        }

        public byte[] buildWriteMultipleFrame(short regAddress, byte[] writeValues, short addressQuantity, byte slaveID, byte functionCode)
        {
            short frameLength = Convert.ToInt16(7 + writeValues.Count());
            //Is TransactionID and ProtocolID can be left at zero
            byte[] startBytes = { 0x00, 0x00, 0x00, 0x00 };
            Array.Reverse(startBytes);

            byte[] lengthBytes = BitConverter.GetBytes(frameLength);
            Array.Reverse(lengthBytes);

            byte[] regAddressBytes = BitConverter.GetBytes(regAddress);
            Array.Reverse(regAddressBytes);

            byte[] addressQuantityBytes = BitConverter.GetBytes(addressQuantity);
            Array.Reverse(addressQuantityBytes);

            byte byteCount = Convert.ToByte(writeValues.Length);

            //byte[] writeValueBytes = BitConverter.GetBytes(writeValues);
            //Array.Reverse(writeValueBytes);

            byte[] frameBytes = startBytes.Concat(lengthBytes).Concat(new byte[] { slaveID }).Concat(new byte[] { functionCode }).Concat(regAddressBytes).Concat(addressQuantityBytes).Concat(new byte[]{ byteCount }).Concat(writeValues).ToArray();

            return frameBytes;
        }


        //Maby noi ideal to pass amount in
        public ModbusFrameResponse sendFrame(byte[] frame, Int16 amountRequested)
        {
            int frameDatalength;
            int frameResponseLength;
            string hexResponseFrame;
            byte[] dataBytes;


            NetworkStream netStream = client.GetStream();

            netStream.Write(frame, 0, frame.Length);

            netStream.Flush();

            //Byte array to store response in
            byte[] responseBuffer = new byte[1024];

            int amountOfReadBytes = netStream.Read(responseBuffer, 0, responseBuffer.Length);

            Array.Resize(ref responseBuffer, amountOfReadBytes);

            if(amountRequested == 1)
            {
                frameDatalength = 2;
            } else
            {
                frameDatalength = responseBuffer[8];

            }

            hexResponseFrame = BitConverter.ToString(responseBuffer);

            Array.Reverse(responseBuffer);

            dataBytes = responseBuffer.Take(frameDatalength).ToArray();

            Array.Reverse(dataBytes);

            //Array.Reverse(dataBytes);
            Array.Reverse(responseBuffer);

            ModbusFrameResponse Response = new ModbusFrameResponse { responseHexFrame = hexResponseFrame, amountOfDataBytes = frameDatalength, dataBytes = dataBytes, sentHexFrame = BitConverter.ToString(frame), functionCode = responseBuffer[7] };


            if (IsBitSet(Response.functionCode, 7))
            {
                switch (Response.dataBytes.LastOrDefault())
                {
                    case 01:
                        throw new ModbusResponseErrorException("Illegal function", Response.dataBytes[Response.dataBytes.Length - 1], Response.sentHexFrame, Response.responseHexFrame);
                    case 02:
                        throw new ModbusResponseErrorException("Illegal Data Address", Response.dataBytes[Response.dataBytes.Length - 1], Response.sentHexFrame, Response.responseHexFrame);
                    case 03:
                        throw new ModbusResponseErrorException("Illegal Data Value", Response.dataBytes[Response.dataBytes.Length - 1], Response.sentHexFrame, Response.responseHexFrame);
                    case 04:
                        throw new ModbusResponseErrorException("Slave device failure", Response.dataBytes[Response.dataBytes.Length - 1], Response.sentHexFrame, Response.responseHexFrame);
                    case 05:
                        throw new ModbusResponseErrorException("Acknowledge", Response.dataBytes[Response.dataBytes.Length - 1], Response.sentHexFrame, Response.responseHexFrame);
                    case 06:
                        throw new ModbusResponseErrorException("Slave device busy", Response.dataBytes[Response.dataBytes.Length - 1], Response.sentHexFrame, Response.responseHexFrame);
                    case 07:
                        throw new ModbusResponseErrorException("Negative acknowledgment", Response.dataBytes[Response.dataBytes.Length - 1], Response.sentHexFrame, Response.responseHexFrame);
                    case 08:
                        throw new ModbusResponseErrorException("Memory parity error", Response.dataBytes[Response.dataBytes.Length - 1], Response.sentHexFrame, Response.responseHexFrame);
                    case 10:
                        throw new ModbusResponseErrorException("Gateway path unavailable", Response.dataBytes[Response.dataBytes.Length - 1], Response.sentHexFrame, Response.responseHexFrame);
                    case 11:
                        throw new ModbusResponseErrorException("Gateway target device failed to respond", Response.dataBytes[Response.dataBytes.Length - 1], Response.sentHexFrame, Response.responseHexFrame);
                }
            }

                return Response;
        }



        // BELOW IS HELPER METHODS
        public float convertRegistersToFloat(Register reg1, Register reg2)
        {
            //THIS CONVERTS FROM BIG ENDIAN TO SMALL ENDIAN
            byte[] bytes = reg1.bytes.Concat(reg2.bytes).ToArray();

            Array.Reverse(bytes);

            float weightFloat = BitConverter.ToSingle(bytes, 0);

            return weightFloat;
        }

        bool IsBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

    }
}
