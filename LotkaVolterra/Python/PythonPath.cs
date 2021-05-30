using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace LotkaVolterra.Python
{
    public class PythonPath
    {
        private string possibleLocation1 = @"HKLM\SOFTWARE\Python\PythonCore\";
        private string possibleLocation2 = @"HKCU\SOFTWARE\Python\PythonCore\";
        private string possibleLocation3 = @"HKLM\SOFTWARE\Wow6432Node\Python\PythonCore\";

        public string GetPythonPath(string requiredVersion = "", string maxVersion = "")
        {
            List<string> possiblePythonLocation = PossiblePathLocation();
            Dictionary<string, string> pythonLocation = PythonLocation(possiblePythonLocation);
            return GetHighestVersion(pythonLocation);
        }

        private List<string> PossiblePathLocation()
        {
            List<string> tempList = new List<string>()
            {
                possibleLocation1,
                possibleLocation2,
                possibleLocation3
            };
            return tempList;
        }

        private Dictionary<string, string> PythonLocation(List<string> possibleLocation)
        {
            Dictionary<string, string> pythonLocation = new Dictionary<string, string>();
            foreach (var item in possibleLocation)
            {
                string regKey = item.Substring(0, 4),
                    actualPath = item.Substring(5);
                RegistryKey theKey = (regKey == "HKLM" ? Registry.LocalMachine : Registry.CurrentUser);
                RegistryKey theValue = theKey.OpenSubKey(actualPath);

                if (theValue is not null)
                {
                    foreach (var keyName in theValue.GetSubKeyNames())
                    {
                        RegistryKey productKey = theValue.OpenSubKey(keyName);

                        if (productKey is not null)
                        {
                            try
                            {
                                string pythonExePath = productKey.OpenSubKey("InstallPath").GetValue("ExecutablePath")
                                    .ToString();

                                if (!string.IsNullOrEmpty(pythonExePath))
                                {
                                    pythonLocation.Add(keyName.ToString(), pythonExePath);
                                }
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }
                }
            }

            return pythonLocation;
        }

        private string GetHighestVersion(Dictionary<string, string> pythonLocation, string requiredVersion = "",
            string maxVersion = "")
        {
            if (pythonLocation.Count > 0)
            {
                System.Version desiredVersion = new System.Version(requiredVersion == "" ? "0.0.1" : requiredVersion),
                    maxPVersion = new System.Version(maxVersion == "" ? "999.999.999" : maxVersion);

                string highestVersion = "", highestVersionPath = "";

                foreach (KeyValuePair<string, string> pVersion in pythonLocation)
                {
                    //TODO; if on 64-bit machine, prefer the 64 bit version over 32 and vice versa
                    int index = pVersion.Key.IndexOf("-"); //For x-32 and x-64 in version numbers
                    string formattedVersion = index > 0 ? pVersion.Key.Substring(0, index) : pVersion.Key;

                    System.Version thisVersion = new System.Version(formattedVersion);
                    int comparison = desiredVersion.CompareTo(thisVersion),
                        maxComparison = maxPVersion.CompareTo(thisVersion);

                    if (comparison <= 0)
                    {
                        //Version is greater or equal
                        if (maxComparison >= 0)
                        {
                            desiredVersion = thisVersion;

                            highestVersion = pVersion.Key;
                            highestVersionPath = pVersion.Value;
                        }
                        else
                        {
                            //Console.WriteLine("Version is too high; " + maxComparison.ToString());
                        }
                    }
                    else
                    {
                        //Console.WriteLine("Version (" + pVersion.Key + ") is not within the spectrum.");
                    }
                }

                //Console.WriteLine(highestVersion);
                //Console.WriteLine(highestVersionPath);
                return highestVersionPath;
            }

            return "";
        }
    }
}