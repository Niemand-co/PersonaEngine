using Persona;
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Setup.Configuration;
using Microsoft.Win32;
using System.Linq;
using System.IO;

namespace PersonaBuildTool
{

    public enum EWindowsCompiler
    {
        EWC_VisualStudio2019,
        EWC_VisualStudio2022,
        EWC_Clang,
    }

    public partial class VersionNumber
    {
        public int Major { set; get; }
        public int Minor { set; get; }
        public int Patch { set; get; }

        public VersionNumber()
        {
            Major = 0;
            Minor = 0;
            Patch = 0;
        }

        public override string ToString()
        {
            return Major.ToString() + "." + Minor.ToString() + "." + Patch.ToString();
        }

        public static VersionNumber GetVersionFromString(string VersionStr)
        {
            VersionNumber Version = new VersionNumber();

            string[] Parts = VersionStr.Split('.');
            Version.Major = int.Parse(Parts[0]);
            Version.Minor = int.Parse(Parts[1]);
            Version.Patch = int.Parse(Parts[2]);

            return Version;
        }

        public static bool operator < (VersionNumber V1, VersionNumber V2)
        {
            return (V1.Major < V2.Major) ? true : ((V1.Minor < V2.Minor) ? true : (V1.Patch < V2.Patch));
        }

        public static bool operator > (VersionNumber V1, VersionNumber V2)
        {
            return (V1.Major > V2.Major) ? true : ((V1.Minor > V2.Minor) ? true : (V1.Patch > V2.Patch));
        }

        public static bool operator == (VersionNumber V1, VersionNumber V2)
        {
            return V1.Major == V2.Major && V1.Minor == V2.Minor && V1.Patch == V2.Patch;
        }

        public static bool operator != (VersionNumber V1, VersionNumber V2)
        {
            return V1.Major != V2.Major || V1.Minor != V2.Minor || V1.Patch != V2.Patch;
        }
    };

    public class VisualStudioInstallation
    {
        public EWindowsCompiler Compiler;

        public VersionNumber Version;

        public bool bCommunity;

        public bool bPreview;

        public DirectoryReference BaseDir;

        public VisualStudioInstallation(EWindowsCompiler compiler, VersionNumber version, bool bCommunity, bool bPreview, DirectoryReference baseDir)
        {
            this.Compiler = compiler;
            this.Version = version;
            this.bCommunity = bCommunity;
            this.bPreview = bPreview;
            this.BaseDir = baseDir;
        }
    }

    public static class MicrosoftPlatformSDK
    {
        public static bool TryGetToolChain(EVCToolChain ToolChain, PersonaBuildTarget Target, out FileReference OutCompiler, out FileReference OutLinker, out DirectoryReference OutToolChainRootDir, out DirectoryReference OutCPPRuntimeLibraryDir)
        {
            List<VisualStudioInstallation> VSInstallations = FindVisualStudioInstallations();

            if (ToolChain == EVCToolChain.EVCC_MSVC)
            {
                string MSVCLocation = VSInstallations[0].BaseDir.GetFullName() + "\\VC\\Tools\\MSVC";
                DirectoryReference MSVCDirectory = new DirectoryReference(MSVCLocation);
                List<VersionNumber> Versions = new List<VersionNumber>();
                MSVCDirectory.EnumrateDirectories(Directory =>
                {
                    string MSVCVersionStr = Directory.GetFolderName();
                    VersionNumber Version = VersionNumber.GetVersionFromString(MSVCVersionStr);
                    Versions.Add(Version);
                });

                if (Versions.Count == 0)
                {
                    throw new Exception("ERROR: There's no MSVC Version.");
                }
                Versions.Sort((V1, V2) => 
                {
                    return V1 > V2 ? -1 : V1 < V2 ? 1 : 0;
                });

                string HostArchitecture = "";
                switch(PersonaPlatformInfo.GetCurrentPlatform().CurrentArch)
                {
                    case EArchitecture.EA_X86:   HostArchitecture = "Hostx86"; break;
                    case EArchitecture.EA_X64:   HostArchitecture = "Hostx64"; break;
                    default: throw new Exception("ERROR: Unsupported Architecture to Select MSVC Version.");
                }

                string TargetArchitecture = "";
                switch(Target.TargetArch)
                {
                    case EArchitecture.EA_X86:   TargetArchitecture = "x86"; break;
                    case EArchitecture.EA_X64:   TargetArchitecture = "x64"; break;
                    case EArchitecture.EA_Arm:   TargetArchitecture = "arm"; break;
                    case EArchitecture.EA_ARM64: TargetArchitecture = "arm64"; break;
                    default: throw new Exception("ERROR: Unknown Architecture to Select MSVC Version.");
                }

                OutToolChainRootDir = MSVCDirectory.Combine("\\" + Versions[0].ToString());
                DirectoryReference FinalLocation = OutToolChainRootDir.Combine("\\bin\\" + HostArchitecture + "\\" + TargetArchitecture);
                OutCompiler = new FileReference("cl.exe", FinalLocation);
                OutLinker = new FileReference("link.exe", FinalLocation);
                OutCPPRuntimeLibraryDir = OutToolChainRootDir.Combine("\\lib\\" + TargetArchitecture);
                return true;
            }
            else
            {
                OutToolChainRootDir = null;
                OutCompiler = null;
                OutLinker = null;
                OutCPPRuntimeLibraryDir = null;
                return false;
            }
        }

        public static List<VisualStudioInstallation> FindVisualStudioInstallations()
        {
            SetupConfiguration Congiguration = new SetupConfiguration();
            IEnumSetupInstances Enumrator =  Congiguration.EnumInstances();

            List<VisualStudioInstallation> Installations = new List<VisualStudioInstallation>();
            ISetupInstance[] Instances = new ISetupInstance[1];
            while (true)
            {
                int NumFetched = 0;
                Enumrator.Next(1, Instances, out NumFetched);
                if (NumFetched == 0)
                {
                    break;
                }

                ISetupInstance2 Instance = (ISetupInstance2)Instances[0];
                VersionNumber Version = VersionNumber.GetVersionFromString(Instance.GetInstallationVersion());
                EWindowsCompiler Compiler = EWindowsCompiler.EWC_VisualStudio2019;
                if(Version.Major == 17)
                {
                    Compiler = EWindowsCompiler.EWC_VisualStudio2022;
                }
                else
                {
                    continue;
                }

                ISetupInstanceCatalog? Catalog = Instance as ISetupInstanceCatalog;
                bool bPreview = Catalog != null && Catalog.IsPrerelease();

                bool bCommunity = Instance.GetProduct().GetId().Equals("Microsoft.VisualStudio.Product.Community", StringComparison.Ordinal);

                Installations.Add(new VisualStudioInstallation(Compiler, Version, bCommunity, bPreview, new DirectoryReference(Instance.GetInstallationPath())));
            }

            Installations.Sort((Instance1, Instance2) =>
            {
                if (Instance1.Version != Instance2.Version)
                {
                    return Instance1.Version > Instance2.Version ? -1 : 1;
                }
                else if (Instance1.bPreview != Instance2.bPreview)
                {
                    return Instance1.bPreview ? -1 : 1;
                }
                else if (Instance1.bCommunity != Instance2.bCommunity)
                {
                    return Instance1.bCommunity ? 1 : -1;
                }
                
                return 0;
            });

            return Installations;
        }

        public partial class WindowsSDKVersion : VersionNumber
        {
            int Unknown;

            WindowsSDKVersion()
                : base()
            {
                Unknown = 0;
            }

            public static WindowsSDKVersion GetVersionFromString(string VersionStr)
            {
                WindowsSDKVersion Version = new WindowsSDKVersion();

                string[] Parts = VersionStr.Split('.');
                Version.Major = int.Parse(Parts[0]);
                Version.Minor = int.Parse(Parts[1]);
                Version.Patch = int.Parse(Parts[2]);
                Version.Unknown = int.Parse(Parts[3]);

                return Version;
            }

            public override string ToString()
            {
                return Major.ToString() + "." + Minor.ToString() + "." + Patch.ToString() + "." + Unknown.ToString();
            }

            public static bool operator <(WindowsSDKVersion V1, WindowsSDKVersion V2)
            {
                return (V1.Major < V2.Major) ? true : ((V1.Minor < V2.Minor) ? true : ((V1.Patch < V2.Patch) ? true : (V1.Unknown < V2.Unknown)));
            }

            public static bool operator >(WindowsSDKVersion V1, WindowsSDKVersion V2)
            {
                return (V1.Major > V2.Major) ? true : ((V1.Minor > V2.Minor) ? true : ((V1.Patch > V2.Patch) ? true : (V1.Unknown > V2.Unknown)));
            }

            public static bool operator ==(WindowsSDKVersion V1, WindowsSDKVersion V2)
            {
                return V1.Major == V2.Major && V1.Minor == V2.Minor && V1.Patch == V2.Patch && V1.Unknown == V2.Unknown;
            }

            public static bool operator !=(WindowsSDKVersion V1, WindowsSDKVersion V2)
            {
                return V1.Major != V2.Major || V1.Minor != V2.Minor || V1.Patch != V2.Patch || V1.Unknown != V2.Unknown;
            }
        }

        public static bool GetWindowsSDKDirectory(out DirectoryReference OutWindowsSDKRootDirectory, out string OutVersion, string? ForcedVersion)
        {
            List<string> VersionStrings = null;
            if (TryReadWindowsSDKFromRegistryKey("SOFTWARE\\Microsoft\\Windows Kits\\Installed Roots", "KitsRoot10", out OutWindowsSDKRootDirectory, out VersionStrings))
            {
                List<WindowsSDKVersion> Versions = new List<WindowsSDKVersion>();
                foreach (string VersionString in VersionStrings)
                {
                    Versions.Add(WindowsSDKVersion.GetVersionFromString(VersionString));
                }
                Versions.Sort((V1, V2) =>
                {
                    if (V1 > V2) return -1;
                    else if (V1 < V2) return 1;
                    else return 0;
                });

                if (!string.IsNullOrEmpty(ForcedVersion))
                {
                    foreach (WindowsSDKVersion V in Versions)
                    {
                        if (V.ToString() == ForcedVersion)
                        {
                            OutVersion = V.ToString();
                            return true;
                        }
                    }
                }

                OutVersion = Versions[0].ToString();
                return true;
            }
            else
            {
                throw new Exception("ERROR: Cannot Find Windows SDK Directory.");
            }
        }

        private static bool TryReadWindowsSDKFromRegistryKey(string KeyName, string ValueName, out DirectoryReference RootDir, out List<string> OutVersions)
        {
            RegistryKey WindowsKitsKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default).OpenSubKey(KeyName);
            string? RootDirStr = WindowsKitsKey.GetValue(ValueName) as string;
            if (string.IsNullOrEmpty(RootDirStr))
            {
                RootDir = null;
                OutVersions = null;
                return false;
            }
            else
            {
                RootDir = new DirectoryReference(RootDirStr);
                string[] Versions = WindowsKitsKey.GetSubKeyNames();
                OutVersions = Versions.ToList<string>();
                return true;
            }
        }
    }

}