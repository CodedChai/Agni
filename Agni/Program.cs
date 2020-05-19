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

            int updateConnectedArduinoCycle = 0;

            while (true)
            {
                Thread.Sleep(1000);
                arduinoController.WriteDataToArduinos(agniMonitor.GetHardwareTemperatures());

                if(updateConnectedArduinoCycle == 0)
                {
                    arduinoController.ConnectToAllArduinos();
                }

                updateConnectedArduinoCycle = (updateConnectedArduinoCycle + 1) % 10;
            }
        }

    }
}