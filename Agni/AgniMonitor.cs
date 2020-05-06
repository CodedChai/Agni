using OpenHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agni
{
    class AgniMonitor
    {
        private void GetHardwareTemperature(IHardware hardware)
        {
            for (int j = 0; j < hardware.Sensors.Length; j++)
            {
                if (hardware.Sensors[j].SensorType == SensorType.Temperature)
                {
                    Console.WriteLine(hardware.Sensors[j].Name + ":" + hardware.Sensors[j].Value.ToString() + "\r");
                }
            }
        }

        public void GetSystemInfo()
        {
            HardwareUpdateVisitor updateVisitor = new HardwareUpdateVisitor();
            Computer computer = new Computer();
            computer.Open();
            computer.CPUEnabled = true;
            computer.GPUEnabled = true;
            computer.Accept(updateVisitor);
            for (int i = 0; i < computer.Hardware.Length; i++)
            {
                if (computer.Hardware[i].HardwareType == HardwareType.CPU)
                {
                    GetHardwareTemperature(computer.Hardware[i]);
                }
                else if (computer.Hardware[i].HardwareType == HardwareType.GpuNvidia)
                {
                    GetHardwareTemperature(computer.Hardware[i]);
                }
                else if (computer.Hardware[i].HardwareType == HardwareType.GpuAti)
                {
                    GetHardwareTemperature(computer.Hardware[i]);
                }
            }
            computer.Close();
        }
    }
}
