using System;
using System.Reflection;
using System.Collections.Generic;
using Persona;

namespace PersonaBuildTool
{

    static class PersonaBuildTool
    {
        private static int Main(string[] ArgumentsArray)
        {
            PToolMode ToolMode = null;
            Editor e = new Editor(new DirectoryReference("Test"));
            //ArgumentsArray = new string[2] { "-Rebuild", "-Target=PersonaEditor Win64 Development" };
            if (ArgumentsArray == null || ArgumentsArray.Length == 0)
            {
                throw new ArgumentNullException("There's no arguments for PBT!!!");
            }
            
            string PBTMode = ArgumentsArray[0];
            if (PBTMode == "-Generate")
            {
                ToolMode = new PGenerateProjectToolMode();
                ModuleCollector.Initialize(PersonaEngine.EngineSourceDirectory);
            }
            else if (PBTMode == "-Build")
            {
                ToolMode = new PBuildToolMode();
                ModuleCollector.Initialize(PersonaEngine.EngineSourceDirectory, true);
            }
            else if (PBTMode == "-Rebuild")
            {
                ToolMode = new PRebuildToolMode();
                ModuleCollector.Initialize(PersonaEngine.EngineSourceDirectory, true);
            }
            else
            {
                ToolMode = new PCleanToolMode();
            }

            // Process commandline arguments
            ToolMode.ProcessCommandLine(ArgumentsArray.AsSpan(1));

            ToolMode.Execute();

            return 0;
        }
    }

}