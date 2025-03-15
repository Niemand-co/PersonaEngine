using System;
using System.Runtime;
using System.Runtime.InteropServices;

namespace PersonaBuildTool
{

    public class PersonaPlatformInfo
    {

        public EOperatingSystem CurrentOS { get; }

        public EArchitecture CurrentArch { get; }

        public PersonaPlatformInfo()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                CurrentOS = EOperatingSystem.EOS_Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                CurrentOS = EOperatingSystem.EOS_Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                CurrentOS= EOperatingSystem.EOS_OSX;
            }
            else
            {
                throw new Exception("ERROR: Unsupported Operating System.");
            }

            switch(RuntimeInformation.OSArchitecture)
            {
                case Architecture.X86:   CurrentArch = EArchitecture.EA_X86; break;
                case Architecture.X64:   CurrentArch = EArchitecture.EA_X64; break;
                case Architecture.Arm:   CurrentArch = EArchitecture.EA_Arm; break;
                case Architecture.Arm64: CurrentArch = EArchitecture.EA_ARM64; break;
                default: throw new Exception("ERROR: Unsupported Architecture.");
            }
        }

        public static PersonaPlatformInfo GetCurrentPlatform()
        {
            return GPlatformInfo;
        }

        public static PersonaPlatformInfo GPlatformInfo = new PersonaPlatformInfo();
    }

}