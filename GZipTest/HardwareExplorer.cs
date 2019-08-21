using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest
{
    /// <summary>
    /// calculate max size of list from free RAM size
    /// </summary>
    static class HardwareExplorer
    {
        /// <summary>
        /// calculate max size of the potetial bite list
        /// </summary>
        /// <returns></returns>
        public static int GetSizeOfByteList()
        {
            ulong ramSize = GetSizeOfRam();
            if (ramSize < 100000) 
            {
                //min size
                ramSize = getDefaultValue();
            }
            else if(ramSize > 2000000)
            {
                //max size
                ramSize = 2000000;
            }

            int calculation = Convert.ToInt32((ramSize * 1000) / 8);
            Debug.WriteLine("HardwareExplorer calculate max list size to: " + calculation);
            return calculation;
        }

        /// <summary>
        /// return size of free ram in Kb (or default)
        /// </summary>
        private static ulong GetSizeOfRam()
        {
            ulong avalebleMemoryKb = 0;
            try
            {
                using (PerformanceCounter ram = new PerformanceCounter("Memory", "Available MBytes", null))
                {
                    avalebleMemoryKb = Convert.ToUInt64(ram.NextValue()) * 1000;
                }
            }
            catch (Exception)
            {
                avalebleMemoryKb = getDefaultValue();
            }
            return avalebleMemoryKb;
        }

        /// <summary>
        /// return default value in (Mb). When available RAM is too small
        /// </summary>
        private static ulong getDefaultValue()
        {
            return (100 * 1000);
        }
    }
}
