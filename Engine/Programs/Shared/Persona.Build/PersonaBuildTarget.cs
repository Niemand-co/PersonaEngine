using Persona;
using System;
using System.Collections.Generic;

namespace PersonaBuildTool
{
    public enum EOperatingSystem
    {
        EOS_Windows,
        EOS_Linux,
        EOS_OSX,
    }

    public enum EArchitecture
    {
        EA_X86,
        EA_X64,
        EA_Arm,
        EA_ARM64,
    }

    public enum ERuntimeType
    {
        ERT_Editor,
        ERT_Runtime,
        ERT_Server,
    }

    public class PersonaBuildTarget
    {
        public PersonaBuildPlatform TargetPlatform;

        public EArchitecture TargetArch;

        public ERuntimeType TargetRuntimeType;

        public DirectoryReference IntermediateDirectory;

        public string TargetApplicationName;

        public string TargetPlatformName;

        public PersonaBuildTarget(string Application, string Platform, string Configuration)
        {
            GetPlatform(Platform);
            GetRuntimeType(Application);
            TargetApplicationName = Application;
            TargetPlatformName = Platform;
            IntermediateDirectory = GetIntermediateDirectory(Platform, Configuration);
        }

        public void Build(List<PEBuildModule> Modules)
        {
            PersonaBuildToolChain ToolChain = CreateToolChain();

            PECompilingEnvironment GlobalCompilingEnvironment = new PECompilingEnvironment(ToolChain);
            PELinkingEnvironment GlobalLinkingEnvironment = new PELinkingEnvironment();
            SetGlobalEnvironment(ToolChain, ref GlobalCompilingEnvironment, ref GlobalLinkingEnvironment);

            List<FileReference> DynamicLibraries = new List<FileReference>();
            foreach (PEBuildModule Module in Modules)
            {
                if (Module.Rules.bSkipBuild)
                {
                    continue;
                }

                PECompilingEnvironment CompilingEnvironment = new PECompilingEnvironment(GlobalCompilingEnvironment);
                PELinkingEnvironment LinkingEnvironment = new PELinkingEnvironment(GlobalLinkingEnvironment);

                SetEnvironment(Module.Rules, ref CompilingEnvironment, ref LinkingEnvironment);

                FileReference LinkOutput = Module.Build(this, ToolChain, CompilingEnvironment, LinkingEnvironment);
            }

            foreach (PEBuildModule Module in Modules)
            {
                FileReference DynamicLibrary = new FileReference(TargetApplicationName + "-" + Module.Rules.ToString() + ".dll", IntermediateDirectory.Combine("\\" + Module.Rules.ToString()));
                DynamicLibraries.Add(DynamicLibrary);
            }

            if (TargetRuntimeType == ERuntimeType.ERT_Editor)
            {
                ToolChain.ExportExcutableFile(DynamicLibraries, PersonaEngine.EngineDirectroy.Combine("\\Binaries\\" + TargetPlatformName), "PersonaEditor.exe");
            }
        }

        public DirectoryReference GetIntermediateDirectory(string Platform, string Configuration)
        {
            string HostPlatform = "";
            switch (PersonaPlatformInfo.GetCurrentPlatform().CurrentOS)
            {
                case EOperatingSystem.EOS_Windows: HostPlatform = "Win"; break;
                case EOperatingSystem.EOS_Linux: HostPlatform = "Linux"; break;
                case EOperatingSystem.EOS_OSX: HostPlatform = "OSX"; break;
                default: throw new Exception("ERROR: Unsupported OS.");
            }
            switch (PersonaPlatformInfo.GetCurrentPlatform().CurrentArch)
            {
                case EArchitecture.EA_X86: HostPlatform += "86"; break;
                case EArchitecture.EA_X64: HostPlatform += "64"; break;
                case EArchitecture.EA_Arm: HostPlatform += "Arm"; break;
                case EArchitecture.EA_ARM64: HostPlatform += "Arm64"; break;
                default: throw new Exception("ERROR: Unsupported Architecture.");
            }

            string TargetArchitecture = "";
            switch (TargetArch)
            {
                case EArchitecture.EA_X86: TargetArchitecture = "x86"; break;
                case EArchitecture.EA_X64: TargetArchitecture = "x64"; break;
                default: throw new Exception("ERROR: Unsupported Architecture.");
            }

            return PersonaEngine.EngineDirectroy.Combine("\\Intermediate\\Build\\" + HostPlatform + "\\" + TargetArchitecture + "\\" + TargetApplicationName + "\\" + Configuration);
        }

        private PersonaBuildToolChain CreateToolChain()
        {
            return TargetPlatform.CreateToolChain(this);
        }

        private void SetGlobalEnvironment(PersonaBuildToolChain ToolChain, ref PECompilingEnvironment GlobalCompilingEnvironment, ref PELinkingEnvironment GlobalLinkingEnvironment)
        {
            TargetPlatform.SetGlobalEnvironment(this, ref GlobalCompilingEnvironment, ref GlobalLinkingEnvironment);
            ToolChain.SetupEnvironment(ref GlobalCompilingEnvironment, ref GlobalLinkingEnvironment);
        }

        private void SetEnvironment(ModuleRules Rules, ref PECompilingEnvironment CompilingEnvironment, ref PELinkingEnvironment LinkingEnvironment)
        {
            foreach (string DependencyName in Rules.PublicDependencyModuleNames)
            {
                ModuleRules Dependency = ModuleCollector.GetModuleByName(DependencyName);
                CompilingEnvironment.UserIncludeDirectories.Add(Dependency.PublicPath.GetFullName());
            }

            foreach (string DependencyName in Rules.PrivateDependencyModuleNames)
            {
                ModuleRules Dependency = ModuleCollector.GetModuleByName(DependencyName);
                CompilingEnvironment.UserIncludeDirectories.Add(Dependency.PublicPath.GetFullName());
                LinkingEnvironment.DependencyLibraries.Add(DependencyName);
            }

            CompilingEnvironment.Definitions.AddRange(Rules.Definitions);
            CompilingEnvironment.UserIncludeDirectories.Add(Rules.PublicPath.GetFullName());
        }

        private void GetPlatform(string InArchStr)
        {
            if (InArchStr == "Win32")
            {
                TargetArch = EArchitecture.EA_X86;
                TargetPlatform = new WindowsBuildPlatform();
            }
            else if (InArchStr == "Win64")
            {
                TargetArch = EArchitecture.EA_X64;
                TargetPlatform = new WindowsBuildPlatform();
            }
            else
            {
                throw new ArgumentException("ERROR: Unknown Architecture {0} from Build Arguments.", InArchStr);
            }
        }

        private void GetRuntimeType(string InApplicationStr)
        {
            if (InApplicationStr.EndsWith("Editor"))
            {
                TargetRuntimeType = ERuntimeType.ERT_Editor;
            }
            else if (InApplicationStr.EndsWith("Game"))
            {
                TargetRuntimeType = ERuntimeType.ERT_Runtime;
            }
            else if (InApplicationStr.EndsWith("Server"))
            {
                TargetRuntimeType = ERuntimeType.ERT_Server;
            }
            else
            {
                throw new ArgumentException("ERROR: Unknown Application Type {0} from Build Arguments.", InApplicationStr);
            }
        }
    }

}