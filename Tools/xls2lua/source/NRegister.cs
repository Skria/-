using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace xls2lua
{
    class NRegister
    {
        public NRegister(string strSubKey)
        {
            m_strSubKey = strSubKey;
        }
        public string GetRegistData(string name)
        {
            string registData = string.Empty;
            RegistryKey software = null;
            RegistryKey aimdir = null;
            do
            {
                RegistryKey hkml = Registry.LocalMachine;
                software = hkml.OpenSubKey("SOFTWARE", true);
                aimdir = software.OpenSubKey(m_strSubKey, true);
                if (null == aimdir)
                    break;
                System.Object v = aimdir.GetValue(name);
                if (null == v)
                    break;
                registData = v.ToString();
            }
            while (false);
            if(null != aimdir)
                aimdir.Close();
            if(null != software)
                software.Close();

            return registData;
        }

        public void SetRegistData(string name, string tovalue)
        {
            RegistryKey software = null;
            RegistryKey aimdir = null;
            do
            {
                RegistryKey hklm = Registry.LocalMachine;
                software = hklm.OpenSubKey("SOFTWARE", true);
                aimdir = software.OpenSubKey(m_strSubKey, true);
                if (null == aimdir)
                    aimdir = software.CreateSubKey(m_strSubKey);
                if (null == aimdir)
                    break;
                aimdir.SetValue(name, tovalue);
            }
            while (false);
            if (null != aimdir)
                aimdir.Close();
            if (null != software)
                software.Close();
        }

        string m_strSubKey;
    }
}
