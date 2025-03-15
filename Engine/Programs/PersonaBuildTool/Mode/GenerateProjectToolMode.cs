using Persona;
using System;
using System.IO;
using System.Collections.Generic;

namespace PersonaBuildTool
{

    public class PGenerateProjectToolMode : PToolMode
    {
        public PGenerateProjectToolMode()
        {

        }

        public override void Execute()
        {
            CreateAllPCHHeaderFiles();

            VCProjectFile ProjectFile = VCProjectFileGenerator.CreateProjectFile(PersonaEngine.EngineSourceDirectory, "PE");

            List<VCProjectFile> CPPProjectFiles = new List<VCProjectFile>();
            CPPProjectFiles.Add(ProjectFile);

            VCProjectFileGenerator.CreateSolutionFile(CPPProjectFiles);
        }

        public override void ProcessCommandLine(Span<string> Arguments)
        {
            
        }

        private void CreateAllPCHHeaderFiles()
        {
            foreach (ModuleRules Module in ModuleCollector.Modules)
            {
                CreatePCHHeaderFile(Module);
            }
        }

        private void CreatePCHHeaderFile(ModuleRules Module)
        {
            string PrivatePCHHeaderFile = Module.PrivatePCHHeaderFile;

            string PCHHeaderFileContent = string.Format("// PCH for \"{0}\"\r\n", Module.ToString());

            string ModuleAPIDefine = string.Format("#define {0}_API DLLEXPORT\r\n", Module.ToString().ToUpper());

            PCHHeaderFileContent += ModuleAPIDefine;

            foreach (string Definition in Module.Definitions)
            {
                if (Definition.Contains("="))
                {
                    string DefinitionName = Definition.Substring(0, Definition.LastIndexOf('='));
                    string DefinitionValue = Definition.Substring(Definition.LastIndexOf('=') + 1);
                    PCHHeaderFileContent += string.Format("#define {0} {1}\r\n", DefinitionName, DefinitionValue);
                }
            }

            if (PrivatePCHHeaderFile != null)
            {
                PCHHeaderFileContent += string.Format("#include \"{0}\"", PrivatePCHHeaderFile);
            }

            FileReference PCHHeaderFile = new FileReference(string.Format("PCH.{0}.h", Module.ToString()), PersonaEngine.EngineDirectroy.Combine("\\Intermediate\\Build\\PCHHeaders"));
            PCHHeaderFile.WriteFileContent(PCHHeaderFileContent, FileMode.Create);
        }
    }

}