using System;

namespace PersonaBuildTool
{

    public class WindowsBuildPlatform : PersonaBuildPlatform
    {
        public WindowsBuildPlatform()
        {
            SDK = new WindowsPlatformSDK();
        }

        public override PersonaBuildToolChain CreateToolChain(PersonaBuildTarget Target)
        {
            return new VCToolChain(Target);
        }

        public override void SetGlobalEnvironment(PersonaBuildTarget Target, ref PECompilingEnvironment CompilingEnvironment, ref PELinkingEnvironment LinkingEnvironment)
        {
            string TargetArchitecture = "";
            switch (Target.TargetArch)
            {
                case EArchitecture.EA_X86: TargetArchitecture = "x86"; break;
                case EArchitecture.EA_X64: TargetArchitecture = "x64"; break;
                case EArchitecture.EA_Arm: TargetArchitecture = "arm"; break;
                case EArchitecture.EA_ARM64: TargetArchitecture = "arm64"; break;
                default: throw new Exception("ERROR: Unknown Architecture to Select MSVC Version.");
            }

            foreach (string IncludeDir in SDK.GetPlatformSDKIncludes())
            {
                CompilingEnvironment.SystemIncludeDirectories.Add(IncludeDir);
            }

            foreach (string LibraryDir in SDK.GetPlatformSDKLibraries())
            {
                LinkingEnvironment.SystemLibraryDirectories.Add(LibraryDir + "\\" + TargetArchitecture);
            }
        }
    }

}