using System;
using System.Buffers;
using System.IO.Pipelines;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace Agni
{
    class Program
    {

        private static void Main(string[] args)
        {
            AgniMonitor agniMonitor = new AgniMonitor();

            string[] portNames = SerialPort.GetPortNames();
          
                foreach (string portName in portNames)
                {
                    SerialPort port = InstantiatePort(portName);
                    try
                    {
                       
                        ReadSerialBytesAsync(port).GetAwaiter().GetResult();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        continue;
                    }

                    // port.Close();
                }
            
            Console.ReadLine();

            /*while (true)
            {
                agniMonitor.GetSystemInfo();
            }*/
        }

        private static SerialPort InstantiatePort(string portName)
        {
            return new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
        }

        private static async Task ReadSerialBytesAsync (SerialPort port)
        {

            var cancellation = new CancellationTokenSource(300);
            cancellation.CancelAfter(300);
            CancellationToken token = cancellation.Token;

            if (!port.IsOpen)
            {
                port.Open();
                Console.WriteLine("Opening port " + port.PortName);
            }

            var reader = PipeReader.Create(port.BaseStream);

            Console.WriteLine("Is the port open? " + port.IsOpen);

            while (!token.IsCancellationRequested && port.IsOpen)
            {
                try
                {
                    Console.WriteLine("We are about to try to read stuff...");
                    ReadResult readResult = reader.ReadAsync(token).GetAwaiter().GetResult();
                    Console.WriteLine("Reading stuff " + readResult.Buffer + " " + readResult.ToString());
                    // find and handle packets
                    // Normally wrapped in a handle-method and a while to allow processing of several packets at once 
                    // while(HandleIncoming(result))
                    // {
                    Console.WriteLine(readResult.Buffer.PositionOf((byte)'~').HasValue);

                    readResult.Buffer.Slice(10); // Moves Buffer.Start to position 10, which we use later to advance the reader
                                                 // }

                    Console.WriteLine(readResult.Buffer.ToString());

                    // Tell the PipeReader how much of the buffer we have consumed. This will "free" that part of the buffer
                    reader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);

                    // Stop reading if there's no more data coming
                    if (readResult.IsCompleted)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in ReadSerialBytesAsync: " + ex.ToString());
                    throw ex;
                }
            }
            if (port.IsOpen)
            {
                port.Close();
            }
        }
    }
}