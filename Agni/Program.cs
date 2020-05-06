using OpenHardwareMonitor.Hardware;
using System;

namespace Agni
{
    class Program
    {

        private static void Main(string[] args)
        {
            AgniMonitor agniMonitor = new AgniMonitor();
            while (true)
            {
                agniMonitor.GetSystemInfo();
            }
        }
    }
}