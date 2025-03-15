using Persona;
using System;
using System.IO;
using System.Collections.Generic;

namespace PersonaBuildTool
{

    public class PEBuildModuleCPP : PEBuildModule
    {

        public PEBuildModuleCPP(ModuleRules InRules)
            : base(InRules)
        {

        }

        public override FileReference Build(PersonaBuildTarget Target, PersonaBuildToolChain ToolChain, PECompilingEnvironment CompilingEnvironment, PELinkingEnvironment LinkingEnvironment)
        {
            base.Build(Target, ToolChain, CompilingEnvironment, LinkingEnvironment);

            List<FileReference> LinkInputFiles = CompileFiles(Target, ToolChain, CompilingEnvironment);
            return LinkFiles(Target, ToolChain, LinkingEnvironment, LinkInputFiles);
        }

        private List<FileItem> GenerateCpps()
        {
            List<FileItem> SepratedCppFiles = new List<FileItem>();
            Rules.PrivatePath.Iterate(File =>
            {
                if (File.Name.EndsWith(".cpp"))
                {
                    File.ReadToString(out string FileContent);
                    SepratedCppFiles.Add(new FileItem(FileContent));
                }

                return true;
            }, null);

            List<FileItem> CppFiles = new List<FileItem>();
            FileItem CurrentFile = new FileItem();
            foreach (FileItem CppFile in SepratedCppFiles)
            {
                CurrentFile.Combine(CppFile);
                if (CurrentFile.Size() > 1048576)
                {
                    CppFiles.Add(new FileItem(CurrentFile));
                    CurrentFile.Clear();
                }
            }
            if (CurrentFile.Content.Length > 0)
            {
                CppFiles.Add(CurrentFile);
            }
            return CppFiles;
        }

        private string GetGlobalCompilingArguments(PersonaBuildTarget Target, PECompilingEnvironment CompilingEnvironment)
        {
            string GlobalCompilingArguments = " /c";

            foreach (string IncludeDir in CompilingEnvironment.SystemIncludeDirectories)
            {
                GlobalCompilingArguments += (" /I \"" + IncludeDir + "\"");
            }

            foreach (string IncludeDir in CompilingEnvironment.UserIncludeDirectories)
            {
                GlobalCompilingArguments += (" /I \"" + IncludeDir + "\"");
            }

            return GlobalCompilingArguments;
        }

        private string GetGlobalLinkingArguments(PersonaBuildTarget Target, PELinkingEnvironment LinkingEnvironment)
        {
            string GlobalLinkingArguments = " /DLL";

            foreach (string LibraryDir in LinkingEnvironment.SystemLibraryDirectories)
            {
                GlobalLinkingArguments += (" /LIBPATH:\"" + LibraryDir + "\"");
            }

            foreach (string Dependency in LinkingEnvironment.DependencyLibraries)
            {
                string StaticLibrary = string.Format(" /IMPLIB:\"{0}\\{1}\\{2}-{3}.lib", Target.IntermediateDirectory.GetFullName(), Dependency, Target.TargetApplicationName, Dependency);
                GlobalLinkingArguments += (" /IMPLIB:\"" + StaticLibrary + "\"");
            }

            GlobalLinkingArguments += string.Format(" /IMPLIB:\"{0}\\{1}\\{2}-{3}.lib", Target.IntermediateDirectory.GetFullName(), Rules.ToString(), Target.TargetApplicationName, Rules.ToString());

            return GlobalLinkingArguments;
        }

        private List<FileReference> CompileFiles(PersonaBuildTarget Target, PersonaBuildToolChain ToolChain, PECompilingEnvironment CompilingEnvironment)
        {
            List<FileItem> CppFilesToCompile = GenerateCpps();
            string GlobalCompilingArguments = GetGlobalCompilingArguments(Target, CompilingEnvironment);

            int CppFilesCount = CppFilesToCompile.Count;
            string ModuleName = Rules.ToString();
            List<FileReference> LinkInputFiles = new List<FileReference>();
            DirectoryReference OutputDir = Target.IntermediateDirectory.Combine("\\" + Rules.ToString());
            for (int FileIndex = 0; FileIndex < CppFilesCount; ++FileIndex)
            {
                string UnityFileName = string.Format("Module.{0}.{1}_of_{2}.cpp", ModuleName, FileIndex + 1, CppFilesCount);
                FileReference UnityFile = new FileReference(UnityFileName, OutputDir);
                UnityFile.WriteFileContent(CppFilesToCompile[FileIndex].Content, FileMode.Create);

                LinkInputFiles.Add(ToolChain.CompileCPPFile(UnityFile, GlobalCompilingArguments, OutputDir));
            }

            return LinkInputFiles;
        }

        private FileReference LinkFiles(PersonaBuildTarget Target, PersonaBuildToolChain ToolChain, PELinkingEnvironment LinkingEnvironment, List<FileReference> LinkInputFiles)
        {
            if (LinkInputFiles.Count == 0)
            {
                return null;
            }

            string GlobalLinkingArguments = GetGlobalLinkingArguments(Target, LinkingEnvironment);

            DirectoryReference OutputDir = Target.IntermediateDirectory.Combine("\\" + Rules.ToString());
            return ToolChain.LinkObjectFiles(LinkInputFiles, GlobalLinkingArguments, OutputDir, Target.TargetApplicationName + "-" + Rules.ToString());
        }
    }

}