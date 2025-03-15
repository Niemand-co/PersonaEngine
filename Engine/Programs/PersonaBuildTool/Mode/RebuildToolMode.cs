using Persona;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonaBuildTool
{
    public class PRebuildToolMode : PToolMode
    {
        string TargetApplication;

        string TargetPlatform;

        string TargetConfiguration;

        public PRebuildToolMode()
        {

        }

        public override void Execute()
        {
            List<ModuleRules> ModulesToBuild = ModuleCollector.GetModulesToRebuild();
            ModulesToBuild.Sort((Module1, Module2) =>
            {
                if (Module1.PrioriryToCompile <= Module2.PrioriryToCompile)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            });

            PersonaBuildTarget Target = new PersonaBuildTarget(TargetApplication, TargetPlatform, TargetConfiguration);

            List<PEBuildModule> Modules = new List<PEBuildModule>(ModulesToBuild.Count);
            foreach (ModuleRules Rules in ModulesToBuild)
            {
                Modules.Add(new PEBuildModuleCPP(Rules));
            }

            Target.Build(Modules);

            ModuleCollector.FinishBuildingModels();
        }

        public override void ProcessCommandLine(Span<string> Arguments)
        {
            for (int ArgIndex = 0; ArgIndex < Arguments.Length; ArgIndex++)
            {
                string Arg = Arguments[ArgIndex];
                if (Arg.StartsWith("-Target"))
                {
                    int EqualIndex = Arg.IndexOf('=');
                    string[] Target = Arg.Substring(EqualIndex + 1).Split(' ');
                    TargetApplication = Target[0];
                    TargetPlatform = Target[1];
                    TargetConfiguration = Target[2];
                }
            }
        }

    }
}