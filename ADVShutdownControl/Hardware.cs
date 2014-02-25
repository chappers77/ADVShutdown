using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace ADVShudownControl
{
    class Hardware
    {
        private static string strHardwareID = string.Empty;

        public static string HardwareID()
        {
            if (string.IsNullOrEmpty(strHardwareID))
            {
                strHardwareID = baseId();
                if (strHardwareID == "") { strHardwareID = cpuId(); }
            }
            return strHardwareID;
        }

        //Return a hardware identifier
        private static string Identifier(string strWMIClass, string strWMIProperty)
        {
            string strResult = "";
            ManagementClass ManClass = new ManagementClass(strWMIClass);
            ManagementObjectCollection ManObjClass = ManClass.GetInstances();
            foreach (ManagementObject ManObj in ManObjClass)
            {
                //Only get the first one
                if (strResult == "")
                {
                    try
                    {
                        strResult = ManObj[strWMIProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return strResult;
        }
        public static string cpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string strReturnValue = Identifier("Win32_Processor", "UniqueId");
            if (strReturnValue == "") //If no UniqueID, use ProcessorID
            {
                strReturnValue = Identifier("Win32_Processor", "ProcessorId");
                if (strReturnValue == "") //If no ProcessorId, use Name
                {
                    strReturnValue = Identifier("Win32_Processor", "Name");
                    if (strReturnValue == "") //If no Name, use Manufacturer
                    {
                        strReturnValue = Identifier("Win32_Processor", "Manufacturer");
                    }
                }
            }
            return strReturnValue;
        }

        //Motherboard ID
        public static string baseId()
        {
            return Identifier("Win32_BaseBoard", "SerialNumber");
        }
        
    }
}
