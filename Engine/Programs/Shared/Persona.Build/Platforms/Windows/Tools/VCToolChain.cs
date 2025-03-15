using Persona;
using System;
using System.Collections.Generic;
using System.IO;

namespace PersonaBuildTool
{
    public enum EVCToolChain
    {
        EVCC_MSVC,
        EVCC_CLANG,
    }


    public class VCToolChain : PersonaBuildToolChain
    {
        EVCToolChain ToolChain;

        FileReference Compiler;

        FileReference Linker;

        DirectoryReference ToolChainRootDir;

        DirectoryReference CPPRuntimeLibraryDir;

        public VCToolChain(PersonaBuildTarget Target)
        {
            if (!MicrosoftPlatformSDK.TryGetToolChain(EVCToolChain.EVCC_MSVC, Target, out Compiler, out Linker, out ToolChainRootDir, out CPPRuntimeLibraryDir))
            {
                throw new Exception("ERROR: Failed to Get VC Tool Chain.");
            }
        }

        public override void SetupEnvironment(ref PECompilingEnvironment CompilingEnvironment, ref PELinkingEnvironment LinkingEnvironment)
        {
            CompilingEnvironment.SystemIncludeDirectories.Add(ToolChainRootDir.GetFullName() + "\\include");
            LinkingEnvironment.SystemLibraryDirectories.Add(CPPRuntimeLibraryDir.GetFullName());
        }

        public override FileReference CompileCPPFile(FileReference CPPFile, string CompilingArguments, DirectoryReference OutputDir)
        {
            string SourceFileName = CPPFile.Name;
            FileReference OutputFile = new FileReference(SourceFileName.Substring(0, SourceFileName.LastIndexOf(".cpp")) + ".obj", OutputDir);

            string Command = "\"" + Compiler.GetFullName() + "\"" + CompilingArguments + " \"" + CPPFile.GetFullName() + "\" /Fo\"" + OutputFile.GetFullName() + "\"";
            CommandLine.RunAndLog(Command);
            return OutputFile;
        }

        public override FileReference LinkObjectFiles(List<FileReference> LinkInputFile, string LinkingArguments, DirectoryReference OutputDir, string ModuleName)
        {
            if (LinkInputFile.Count == 0)
            {
                return null;
            }

            string Command = "\"" + Linker.GetFullName() + "\"" + LinkingArguments;

            for (int LinkFileIndex = 0; LinkFileIndex < LinkInputFile.Count; ++LinkFileIndex)
            {
                Command += (" \"" + LinkInputFile[LinkFileIndex].GetFullName() + "\"");
            }

            FileReference OutputFile = new FileReference(ModuleName + ".dll", OutputDir);
            Command += (" /out:\"" + OutputFile.GetFullName() + "\"");
            CommandLine.RunAndLog(Command);
            return OutputFile;
        }

        public override FileReference ExportExcutableFile(List<FileReference> LibraryFiles, DirectoryReference OutputDir, string ExcutableFileName)
        {
            List<string> StaticLibraries = new List<string>();
            List<string> MoveCommands = new List<string>();
            for (int LibraryFileIndex = 0; LibraryFileIndex < LibraryFiles.Count; ++LibraryFileIndex)
            {
                string DLLName = LibraryFiles[LibraryFileIndex].Name;
                string ModuleName = DLLName.Substring(0, DLLName.LastIndexOf(".dll"));
                string SourceDir = LibraryFiles[LibraryFileIndex].Directory.GetFullName();
                FileReference SourceDynamicLibrary = new FileReference(DLLName, LibraryFiles[LibraryFileIndex].Directory);
                FileReference SourceStaticLibrary = new FileReference(ModuleName + ".lib", LibraryFiles[LibraryFileIndex].Directory);
                if (SourceDynamicLibrary.Exists())
                {
                    MoveCommands.Add("move /Y \"" + SourceDynamicLibrary.GetFullName() + "\" \"" + OutputDir.GetFullName() + "\\" + DLLName + "\""); ;
                }

                if (SourceStaticLibrary.Exists())
                {
                    StaticLibraries.Add(SourceStaticLibrary.GetFullName());
                }
            }

            foreach (string MoveCommand in MoveCommands)
            {
                CommandLine.RunAndLog(MoveCommand);
            }

            FileReference OutputFile = new FileReference(ExcutableFileName, OutputDir);
            string LinkingCommand = "\"" + Linker.GetFullName() + "\" /ENTRY:main";
            foreach (string StaticLibrary in StaticLibraries)
            {
                LinkingCommand += (" \"" + StaticLibrary + "\"");
            }
            LinkingCommand += (" /OUT:\"" + OutputFile.GetFullName() + "\"");
            CommandLine.RunAndLog(LinkingCommand);
            return OutputFile;
        }
    }

}