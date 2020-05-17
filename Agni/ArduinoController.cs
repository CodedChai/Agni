using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using System.Text;

namespace Agni
{
    class ArduinoController
    {

        List<SerialPort> arduinos = new List<SerialPort>();

        public void ConnectToAllArduinos()
        {
            string[] portNames = SerialPort.GetPortNames();

            foreach (string portName in portNames)
            {
                if (IsCurrentlyConnectedToArduino(portName))
                {
                    continue;
                }
                InitializeConnection(portName);
            }

            Console.WriteLine("Connected to " + arduinos.Count + " arduino(s).");
        }

        private bool IsCurrentlyConnectedToArduino(string portName)
        {
            return arduinos.Find(port => port.PortName.Equals(portName)) != null;
        }

        public void WriteDataToArduinos(String data)
        {

            foreach (SerialPort port in arduinos)
            {
                try
                {
                    OpenPort(port);

                    port.WriteLine(data);

                    ClosePort(port);
                } catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    DisconnectArduino(port);
                }
           
            }

        }

        private void OpenPort(SerialPort port)
        {
            if (!port.IsOpen)
            {
                port.Open();
                Console.WriteLine("Opening port " + port.PortName);
            }
        }

        private void ClosePort(SerialPort port)
        {
            if (port.IsOpen)
            {
                port.Close();
            }
        }

        private void DisconnectArduino(SerialPort port)
        {
            port.Close();
            arduinos.Remove(port);
        }

        private void InitializeConnection(string portName)
        {
            SerialPort port = InstantiatePort(portName);

            try
            {
                ConnectToArduinoAsync(port).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private SerialPort InstantiatePort(string portName)
        {
            return new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
        }

        private async Task ConnectToArduinoAsync(SerialPort port)
        {

            OpenPort(port);

            var reader = PipeReader.Create(port.BaseStream);

            Console.WriteLine("Is the port open? " + port.IsOpen);

            using (CancellationTokenSource cancellation = new CancellationTokenSource(1000))
            {
                Console.WriteLine("We are about to try to read stuff...");
                cancellation.Token.Register(() => reader.CompleteAsync());
                var readResult = await reader.ReadAsync(cancellation.Token);

                if (isArduino(readResult))
                {
                    arduinos.Add(port);
                }
            }
            ClosePort(port);
        }

        private bool isArduino(ReadResult readResult)
        {
            return readResult.Buffer.PositionOf((byte)'~').HasValue;
        }

    }
}
