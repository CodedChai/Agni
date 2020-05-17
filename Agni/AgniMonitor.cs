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

        Computer computer;
        HardwareUpdateVisitor updateVisitor;

        readonly List<HardwareType> VALID_HARDWARE_TYPES = new List<HardwareType> { HardwareType.CPU, HardwareType.GpuNvidia,  HardwareType.GpuAti };

        public AgniMonitor()
        {
            computer = new Computer();
            updateVisitor = new HardwareUpdateVisitor();

            computer.Open();
            computer.CPUEnabled = true;
            computer.GPUEnabled = true;
        }

        private string GetHardwareTemperature(IHardware hardware)
        {
            for (int j = 0; j < hardware.Sensors.Length; j++)
            {
                if (hardware.Sensors[j].SensorType == SensorType.Temperature)
                {
                    Console.WriteLine(hardware.Sensors[j].Name + ": " + hardware.Sensors[j].Value.ToString() + "\r");

                    // Terrible hack but it works.. we just want the package temp for CPU and this won't affect GPU temps so it'll work
                    if (!hardware.Sensors[j].Name.Contains("#"))
                    {
                        // This is the temperature
                        return hardware.Sensors[j].Value.ToString();
                    }
                }
            }

            // Default to returning two zeros since we always expect two digits
            return "00";
        }

        public string GetHardwareTemperatures()
        {
            computer.Accept(updateVisitor);
            String result = "";

            // The arduino expects CPU to be first so we rely on the ordering of our valid hardware types list
            foreach(HardwareType hardwareType in VALID_HARDWARE_TYPES)
            {
                IHardware currentHardware = Array.Find(computer.Hardware, hardware => hardware.HardwareType.Equals(hardwareType));
                if(currentHardware != null)
                {
                    result += GetHardwareTemperature(currentHardware);
                }
            }

            return result;
        }
    }
}
