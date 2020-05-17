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
            return new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
        }

        private static async Task ReadSerialBytesAsync (SerialPort port)
        {

            if (!port.IsOpen)
            {
                port.Open();
                Console.WriteLine("Opening port " + port.PortName);
            }
            var reader = PipeReader.Create(port.BaseStream);

            Console.WriteLine("Is the port open? " + port.IsOpen);

            using(CancellationTokenSource cancellation = new CancellationTokenSource(1000))
            {
                Console.WriteLine("We are about to try to read stuff...");
                cancellation.Token.Register(() => reader.CompleteAsync());
                var readResult = await reader.ReadAsync(cancellation.Token);

                Console.WriteLine("Reading stuff " + readResult.Buffer + " " + readResult.ToString());
      
                Console.WriteLine("Is this one of the arduinos: " + readResult.Buffer.PositionOf((byte)'~').HasValue);
            }

            if (port.IsOpen)
            {
                port.Close();
            }
        }
    }
}