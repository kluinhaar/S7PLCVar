using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;

public class myS7class
{
    const int INPUTS = 1;
    const int OUTPUTS = 2;

    static libnodave.daveOSserialType fds;
    static libnodave.daveInterface di;
    static libnodave.daveConnection dc;

    public static int Main(string[] args){
        int rack, slot;
        int DB = 0, DataByte = 0, len = 0;
        int res;
        int p0=0, p1=0;
        int area = 0;
        int bitnumber = 0;
        int IO = 0;
        var WriteBuffer = new byte[] { 0x00, 0x00, 0x00, 0x00 };

        bool myBit=false;
        byte myByte = 0;
        System.Int16 myWord = 0;
        System.Int32 myDWord = 0;
        System.Single mySingle = 0.0f;
        Byte[] byteArray = { 0x00, 0x00, 0x00, 0x00 };
        string str = "";
        string Address = "";
        string PLC = "255.255.255.255";

        libnodave.daveSetDebug(libnodave.daveDebugRawRead);

        if (args.Length != 4 && args.Length != 5)
        {
            Console.WriteLine("");
            Console.WriteLine("Usage:");
            Console.WriteLine("S7PLCVar.exe [ipaddress] [rack] [slot] [address] [value]");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine(" Read bit value from I7.3");
            Console.WriteLine(" S7PLCVar.exe 172.24.40.191 0 3 I7.3");
            Console.WriteLine("");
            Console.WriteLine(" Read bit value from DB10.DBX14.5");
            Console.WriteLine(" S7PLCVar.exe 172.24.40.191 0 3 DB10.DBX14.5");
            Console.WriteLine("");
            Console.WriteLine(" Write DWord value 1234 to DB10.DBD14");
            Console.WriteLine(" S7PLCVar.exe 172.24.40.191 0 3 DB10.DBD14 1234");
            Console.WriteLine("");
            Console.WriteLine(" Write Output bit True to Q3.5");
            Console.WriteLine(" S7PLCVar.exe 172.24.40.191 0 3 Q3.5 True");
            Console.WriteLine("");
            return -2;
        }
        else
        {
            PLC = args[0];
            rack = Convert.ToInt32(args[1]);
            slot = Convert.ToInt32(args[2]);
            Address = args[3].ToUpper();

            if(args.Length == 5)
                IO = OUTPUTS;
            else
                IO = INPUTS;

            // ***************************************************************************
            //                   Datablocks
            // ***************************************************************************
            if ( Address.Contains(".DBX") )
            {
                area = libnodave.daveDB;
                len = 1;

                p0 = 2;
                p1 = Address.IndexOf(".DBX") - p0;
                str = Address.Substring(p0, p1);
                DB = Convert.ToInt32(str);

                p0 = Address.IndexOf(".DBX") + 4;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(Convert.ToDouble(str));

                p0 = Address.Length - 1;
                p1 = 1;
                str = Address.Substring(p0, p1);
                bitnumber = Convert.ToInt32(str) + 1;

                if(IO == OUTPUTS)
                    myBit = Convert.ToBoolean(args[4]);
            }
            else if (Address.Contains(".DBB"))
            {
                area = libnodave.daveDB;
                len = 1;

                p0 = 2;
                p1 = Address.IndexOf(".DBB") - p0;
                str = Address.Substring(p0, p1);
                DB = Convert.ToInt32(str);

                p0 = Address.IndexOf(".DBB") + 4;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                if(IO == OUTPUTS)
                    myByte = Convert.ToByte(args[4]);

            }
            else if (Address.Contains(".DBW"))
            {
                area = libnodave.daveDB;
                len = 2;

                p0 = 2;
                p1 = Address.IndexOf(".DBW") - p0;
                str = Address.Substring(p0, p1);
                DB = Convert.ToInt32(str);

                p0 = Address.IndexOf(".DBW") + 4;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                if(IO == OUTPUTS)
                    myWord = Convert.ToInt16(args[4]);
            }
            else if (Address.Contains(".DBD"))
            {
                area = libnodave.daveDB;
                len = 4;

                p0 = 2;
                p1 = Address.IndexOf(".DBD") - p0;
                str = Address.Substring(p0, p1);
                DB = Convert.ToInt32(str);

                p0 = Address.IndexOf(".DBD") + 4;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                if(IO == OUTPUTS){
		    if(args[4].Contains(".")){
			mySingle = Convert.ToSingle(args[4]);
			byteArray = BitConverter.GetBytes(mySingle);
			myDWord = BitConverter.ToInt32(byteArray, 0);
		    }
		    else{
			myDWord = Convert.ToInt32(args[4]);
		    }
		}
            }
            // ***************************************************************************
            //                   INPUTS
            // ***************************************************************************
            else if (Address.Substring(0, 3).Contains("PIB"))
            {
                area = libnodave.daveInputs;
                len = 1;

                p0 = 3;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);
            }
            else if (Address.Substring(0, 3).Contains("PIW"))
            {
                area = libnodave.daveInputs;
                len = 2;

                p0 = 3;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);
            }
            else if (Address.Substring(0, 3).Contains("PID"))
            {
                area = libnodave.daveInputs;
                len = 4;

                p0 = 3;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);
            }
            else if (Address.Substring(0, 2).Contains("IB"))
            {
                area = libnodave.daveInputs;
                len = 1;

                p0 = 2;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);
            }
            else if (Address.Substring(0, 2).Contains("IW"))
            {
                area = libnodave.daveInputs;
                len = 2;

                p0 = 2;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);
            }
            else if (Address.Substring(0, 2).Contains("ID"))
            {
                area = libnodave.daveInputs;
                len = 4;

                p0 = 2;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);
            }
            else if (Address.Substring(0, 1).Contains("I"))
            {
                area = libnodave.daveInputs;

                p0 = 1;
                p1 = Address.IndexOf(".") - 1;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                p1 = p1 + 2;
                str = Address.Substring(p1, 1);
                bitnumber = Convert.ToInt32(str) + 1;

                len = 1;
            }


            // ***************************************************************************
            //                   OUTPUTS
            // ***************************************************************************
            else if (Address.Substring(0, 3).Contains("PQB"))
            {
                area = libnodave.daveOutputs;
                len = 1;

                p0 = 3;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                myByte = Convert.ToByte(args[4]);
            }
            else if (Address.Substring(0, 3).Contains("PQW"))
            {
                area = libnodave.daveOutputs;
                len = 2;

                p0 = 3;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                myWord = Convert.ToInt16(args[4]);
            }
            else if (Address.Substring(0, 3).Contains("PQD"))
            {
                area = libnodave.daveOutputs;
                len = 4;

                p0 = 3;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                if(IO == OUTPUTS){
		    if(args[4].Contains(".")){
			mySingle = Convert.ToSingle(args[4]);
			byteArray = BitConverter.GetBytes(mySingle);
			myDWord = BitConverter.ToInt32(byteArray, 0);
		    }
		    else{
			myDWord = Convert.ToInt32(args[4]);
		    }
		}
            }
            else if (Address.Substring(0, 3).Contains("QB"))
            {
                area = libnodave.daveOutputs;
                len = 1;

                p0 = 2;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                myByte = Convert.ToByte(args[4]);
            }
            else if (Address.Substring(0, 3).Contains("QW"))
            {
                area = libnodave.daveOutputs;
                len = 2;

                p0 = 2;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                myWord = Convert.ToInt16(args[4]);
            }
            else if (Address.Substring(0, 3).Contains("QD"))
            {
                area = libnodave.daveOutputs;
                len = 4;

                p0 = 2;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                if(IO == OUTPUTS){
		    if(args[4].Contains(".")){
			mySingle = Convert.ToSingle(args[4]);
			byteArray = BitConverter.GetBytes(mySingle);
			myDWord = BitConverter.ToInt32(byteArray, 0);
		    }
		    else{
			myDWord = Convert.ToInt32(args[4]);
		    }
		}
            }
            else if (Address.Substring(0, 1).Contains("Q"))
            {
                area = libnodave.daveOutputs;
                len = 1;

                p0 = 1;
                p1 = Address.IndexOf(".") - 1;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                p1 = p1 + 2;
                str = Address.Substring(p1, 1);
                bitnumber = Convert.ToInt32(str) + 1;

                myBit = Convert.ToBoolean(args[4]);
            }

            // ***************************************************************************
            //                   Merkers / Flags
            // ***************************************************************************
            else if (Address.Substring(0, 3).Contains("MB"))
            {
                area = libnodave.daveFlags;
                len = 1;

                p0 = 2;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                if (IO == OUTPUTS)
                    myByte = Convert.ToByte(args[4]);
            }
            else if (Address.Substring(0, 3).Contains("MW"))
            {
                area = libnodave.daveFlags;
                len = 2;

                p0 = 2;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                if (IO == OUTPUTS)
                    myWord = Convert.ToInt16(args[4]);
            }
            else if (Address.Substring(0, 3).Contains("MD"))
            {
                area = libnodave.daveFlags;
                len = 4;

                p0 = 2;
                p1 = Address.Length - p0;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                if(IO == OUTPUTS){
		    if(args[4].Contains(".")){
			mySingle = Convert.ToSingle(args[4]);
			byteArray = BitConverter.GetBytes(mySingle);
			myDWord = BitConverter.ToInt32(byteArray, 0);
		    }
		    else{
			myDWord = Convert.ToInt32(args[4]);
		    }
		}
            }
            else if (Address.Substring(0, 1).Contains("M"))
            {
                area = libnodave.daveFlags;
                len = 1;

                p0 = 1;
                p1 = Address.IndexOf(".") - 1;
                str = Address.Substring(p0, p1);
                DataByte = Convert.ToInt32(str);

                p1 = p1 + 2;
                str = Address.Substring(p1, 1);
                bitnumber = Convert.ToInt32(str) + 1;

                if (IO == OUTPUTS)
                    myBit = Convert.ToBoolean(args[4]);
            }
            // ***************************************************************************
            else if (Address.Substring(0, 1).Contains("C"))
            {
                area = libnodave.daveCounter;
            }
            else if (Address.Substring(0, 1).Contains("T"))
            {
                area = libnodave.daveTimer;
            }

            fds.rfd = libnodave.openSocket(102, PLC);
            fds.wfd = fds.rfd;
            if (fds.rfd > 0)
            {
                di = new libnodave.daveInterface(fds, "IF1", 0, libnodave.daveProtoISOTCP, libnodave.daveSpeed187k);
                di.setTimeout(1000000);
                res=di.initAdapter();
                if(res==0) 
                    {
                        dc = new libnodave.daveConnection(di, 0, rack, slot);
                        if (0 == dc.connectPLC())
                        {
                            if (IO == 0)
                            {
                                Console.WriteLine("Onbekende Input of Output modus.");
                                return -3;
                            }
                            if (IO == OUTPUTS)
                            {
                                if (area == libnodave.daveDB || area == libnodave.daveOutputs || area == libnodave.daveFlags)
                                {
                                    if (bitnumber > 0)
                                    {
                                        res = dc.readBytes(area, DB, DataByte, len, null);
                                        if (res == 0)
                                        {
                                            // Eerst Byte inlezen
                                            myByte = (byte)dc.getU8();

                                            // Daarna Bitje in Byte aanpassen.
                                            if (myBit)
                                            {
                                                myByte = Convert.ToByte(Convert.ToInt16(myByte) + Convert.ToInt16(System.Math.Pow(2, bitnumber - 1)));
                                            }
                                            else
                                            {
                                                myByte = Convert.ToByte(myByte & ~(1 << (bitnumber - 1)));
                                            }
                                        }
                                    }

                                    if (len == 1)
                                    {
                                        WriteBuffer[0] = myByte;
                                        res = dc.writeBytes(area, DB, DataByte, len, WriteBuffer);
                                    }
                                    else if (len == 2)
                                    {
                                        WriteBuffer[1] = Convert.ToByte(myWord & 0xff);
                                        WriteBuffer[0] = Convert.ToByte((myWord >> (8 * 1)) & 0xff);
                                        res = dc.writeBytes(area, DB, DataByte, len, WriteBuffer);
                                    }
                                    else if (len == 4)
				    {
                                    	WriteBuffer[3] = Convert.ToByte(myDWord & 0xff);
                                        WriteBuffer[2] = Convert.ToByte((myDWord >> (8 * 1)) & 0xff);
                                        WriteBuffer[1] = Convert.ToByte((myDWord >> (8 * 2)) & 0xff);
                                        WriteBuffer[0] = Convert.ToByte((myDWord >> (8 * 3)) & 0xff);
                                	res = dc.writeBytes(area, DB, DataByte, len, WriteBuffer);
                                    }
                                }
                            }
                            else if(IO == INPUTS)
                            {
                                res = dc.readBytes(area, DB, DataByte, len, null);
                                if (res == 0)
                                {
                                    if (area == libnodave.daveDB || area == libnodave.daveInputs || area == libnodave.daveFlags)
                                    {
                                        if (bitnumber > 0)
                                        {
                                            myByte = (byte)dc.getU8();
                                            myBit = (myByte & (1 << bitnumber - 1)) != 0;
                                            Console.WriteLine(myBit);
                                        }
                                        else if (len == 1)
                                        {
                                            myByte = (System.Byte)dc.getU8();
                                            Console.WriteLine(myByte);
                                        }
                                        else if (len == 2)
                                        {
                                            myWord = (System.Int16)dc.getU16();
                                            Console.WriteLine(myWord);
                                        }
                                        else if (len == 4)
                                        {
                                            myDWord = (System.Int32)dc.getU32();
                                            Console.WriteLine(myDWord);
                                        }
                                    }
                                    if (area == libnodave.daveCounter)
                                    {
                                        Console.WriteLine(dc.getS8());
                                    }
                                    if (area == libnodave.daveTimer)
                                    { 
                                        Console.WriteLine(dc.getS8());
                                    }
                                }
                            }
                            else
                            { 
                                Console.WriteLine("error " + res + " " + libnodave.daveStrerror(res));
                            }

                        }
                        //if (IO == INPUTS)
                        //    Console.ReadKey();

                        dc.disconnectPLC();
                        }
                    di.disconnectAdapter();
                    libnodave.closeSocket(fds.rfd);
                }
                else
                {
                    Console.WriteLine("Couldn't open TCP connaction to " + args[0]);
                    return -1;
                }
            return 0;
            }
        }
    }

