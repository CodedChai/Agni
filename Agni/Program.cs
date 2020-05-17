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
            ArduinoController arduinoController = new ArduinoController();

            arduinoController.ConnectToAllArduinos();

            while (true)
            {
                Console.ReadLine();

                arduinoController.WriteDataToArduinos(agniMonitor.GetHardwareTemperatures());
            }
        }

    }
}