using Persona;
using System.Collections.Generic;

namespace ModuleBuildTool
{
    public static class PersonaBuildToolGenerator
    {
        static CSProjectFile CreatePersonaBuildToolProjectFile()
        {
            DirectoryReference PBTDirectory = PersonaEngine.EngineDirectroy.Combine("\\Programs\\PersonaBuildTool");
            DirectoryReference SharedDirectory = PersonaEngine.EngineDirectroy.Combine("\\Programs\\Shared\\Persona.Shared");
            DirectoryReference BuildDirectory = PersonaEngine.EngineDirectroy.Combine("\\Programs\\Shared\\Persona.Build");
            DirectoryReference PERulesDirectory = PersonaEngine.EngineDirectroy.Combine("\\Intermediate\\Build\\BuildRules");
            CSProjectFile PBTProjectFile = new CSProjectFile("PersonaBuildTool", PBTDirectory);
            CSProjectFile SharedProjectFile = new CSProjectFile("Persona.Shared", SharedDirectory);
            CSProjectFile BuildProjectFile = new CSProjectFile("Persona.Build", BuildDirectory);
            CSProjectFile PERulesProjectFile = new CSProjectFile("PERules", PERulesDirectory);

            PBTProjectFile.BeginProject();
            {
                PBTProjectFile.BeginPropertyGroup();
                {
                    PBTProjectFile.Insert("TargetFramework", "net6.0");
                    PBTProjectFile.Insert("OutputType", "Exe");
                    PBTProjectFile.Insert("OutputPath", "..\\..\\Binaries\\DotNet\\PersonaBuildTool");
                    PBTProjectFile.Insert("AppendTargetFrameworkToOutputPath", "false");
                    PBTProjectFile.Insert("GenerateAssemblyInfo", "false");
                    PBTProjectFile.Insert("GenerateTargetFrameworkAttribute", "false");
                    PBTProjectFile.Insert("Configurations", "Debug;Release;Development");
                    PBTProjectFile.Insert("AssemblyName", "PersonaBuildTool");
                    PBTProjectFile.Insert("RootNamespace", "PersonaBuildTool");
                    PBTProjectFile.Insert("EnableDefaultCompileItems", "false");
                    PBTProjectFile.Insert("ProjectGuid", "{" + PBTProjectFile.MD5Hash + "}");
                }
                PBTProjectFile.EndPropertyGroup();

                PBTProjectFile.BeginItemGroup("ProjectConfigurations");
                {
                    PBTProjectFile.AddConfiguration("Debug", "Any CPU");
                    PBTProjectFile.AddConfiguration("Release", "Any CPU");
                    PBTProjectFile.AddConfiguration("Development", "Any CPU");
                }
                PBTProjectFile.EndItemGroup();

                PBTProjectFile.ImportProject("..\\PersonaEngine.csproj.props");

                PBTProjectFile.BeginItemGroup();
                {
                    PBTProjectFile.AddReference(SharedProjectFile);
                    PBTProjectFile.AddReference(PERulesProjectFile);
                    PBTProjectFile.AddReference(BuildProjectFile);
                }
                PBTProjectFile.EndItemGroup();

                PBTProjectFile.BeginItemGroup();
                {
                    PBTProjectFile.AddCompilePath("**\\*.cs");
                }
                PBTProjectFile.EndItemGroup();
            }
            PBTProjectFile.EndProject();

            PBTProjectFile.FinishFile();

            return PBTProjectFile;
        }

        public static CSSolutionFile CreatePersonaBuildTool(CSProjectFile PERulesFile)
        {
            DirectoryReference PBTDirectory = PersonaEngine.EngineDirectroy.Combine("\\Programs\\PersonaBuildTool");
            CSSolutionFile PBTSolutionFile = new CSSolutionFile("PersonaBuildTool", PBTDirectory);

            CSProjectFile PBTProjectFile = CreatePersonaBuildToolProjectFile();
            CSProjectFile SharedProjectFile = new CSProjectFile("Persona.Shared", PersonaEngine.EngineDirectroy.Combine("\\Programs\\Shared\\Persona.Shared"));
            CSProjectFile BuildProjectFile = new CSProjectFile("Persona.Build", PersonaEngine.EngineDirectroy.Combine("\\Programs\\Shared\\Persona.Build"));

            PBTSolutionFile.BeginCSProject("PersonaBuildTool", PBTProjectFile);
            {
                List<CSProjectFile> Dependencies = new List<CSProjectFile>();
                Dependencies.Add(SharedProjectFile);
                Dependencies.Add(BuildProjectFile);
                PBTSolutionFile.AddProjectDependency(Dependencies);
            }
            PBTSolutionFile.EndCSProject();

            PBTSolutionFile.BeginCSProject("Persona.Shared", SharedProjectFile);
            PBTSolutionFile.EndCSProject();

            PBTSolutionFile.BeginCSProject("Persona.Build", BuildProjectFile);
            PBTSolutionFile.EndCSProject();

            PBTSolutionFile.BeginCSProject("PERules", PERulesFile);
            PBTSolutionFile.EndCSProject();

            PBTSolutionFile.BeginGlobal();
            {
                PBTSolutionFile.BeginGlobalSection("SolutionConfigurationPlatforms", "preSolution");
                {
                    for(int ConfigurationIndex = 0; ConfigurationIndex < GlobalConfigurationPlatforms.CSConfigurations.Length; ConfigurationIndex++)
                    {
                        PBTSolutionFile.AddSolutionConfiguration(GlobalConfigurationPlatforms.CSConfigurations[ConfigurationIndex], "Any CPU");
                    }
                }
                PBTSolutionFile.EndGlobalSection();

                PBTSolutionFile.BeginGlobalSection("ProjectConfigurationPlatforms", "postSolution");
                {
                    PBTSolutionFile.SetProjectConfiguration(SharedProjectFile);
                    PBTSolutionFile.SetProjectConfiguration(BuildProjectFile);
                    PBTSolutionFile.SetProjectConfiguration(PERulesFile);
                    PBTSolutionFile.SetProjectConfiguration(PBTProjectFile);
                }
                PBTSolutionFile.EndGlobalSection();
            }
            PBTSolutionFile.EndGlobal();

            PBTSolutionFile.FinishFile();

            return PBTSolutionFile;
        }
    }
}