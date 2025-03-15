using Persona;
using System;
using System.Collections.Generic;
using System.IO;

namespace PersonaBuildTool
{

    public class WindowsPlatformSDK : PEBuildPlatformSDK
    {
        public DirectoryReference RootDirectory;

        public string Version;

        public WindowsPlatformSDK()
        {
            try
            {
                MicrosoftPlatformSDK.GetWindowsSDKDirectory(out RootDirectory, out Version, null);
            }
            catch
            {
                Console.WriteLine("ERROR: Failed to get Windows SDK.");
            }
        }

        public override List<string> GetPlatformSDKIncludes()
        {
            List<string> Includes = new List<string>();

            string IncludeDir = RootDirectory.GetFullName() + "Include\\" + Version + "\\";
            Includes.Add(IncludeDir + "ucrt");
            Includes.Add(IncludeDir + "um");
            Includes.Add(IncludeDir + "shared");

            return Includes;
        }

        public override List<string> GetPlatformSDKLibraries()
        {
            List<string> Libraries = new List<string>();

            string LibraryDir = RootDirectory.GetFullName() + "Lib\\" + Version + "\\";
            Libraries.Add(LibraryDir + "ucrt");
            Libraries.Add(LibraryDir + "um");

            return Libraries;
        }
    }

}