using Persona;
using System.IO;

namespace ModuleBuildTool
{
    public static class PersonaRulesGenerator
    {
        public static CSProjectFile CreatePersonaRules()
        {
            DirectoryReference RulesDirectory = PersonaEngine.EngineDirectroy.Combine("\\Intermediate\\Build\\BuildRules");
            DirectoryReference SharedDirectory = PersonaEngine.EngineDirectroy.Combine("\\Programs\\Shared\\Persona.Shared");
            CSProjectFile PersonaRules = new CSProjectFile("PERules", RulesDirectory);
            CSProjectFile SharedProjectFile = new CSProjectFile("Persona.Shared", SharedDirectory);

            PersonaRules.BeginProject();
            {
                PersonaRules.BeginPropertyGroup();
                {
                    PersonaRules.Insert("TargetFramework", "net6.0");
                    PersonaRules.Insert("OutputType", "Library");
                    PersonaRules.Insert("OutputPath", "..\\..\\..\\Binaries\\DotNet\\PERules");
                    PersonaRules.Insert("Configurations", "Debug;Release");
                    PersonaRules.Insert("AssemblyName", "PERules");
                    PersonaRules.Insert("RootNamespace", "PERules");
                    PersonaRules.Insert("EnableDefaultCompileItems", "false");
                    PersonaRules.Insert("AppendTargetFrameworkToOutputPath", "false");
                    PersonaRules.Insert("ProjectGuid", "{" + PersonaRules.MD5Hash + "}");
                }
                PersonaRules.EndPropertyGroup();

                PersonaRules.BeginItemGroup("ProjectConfigurations");
                {
                    PersonaRules.AddConfiguration("Debug", "Any CPU");
                    PersonaRules.AddConfiguration("Release", "Any CPU");
                }
                PersonaRules.EndItemGroup();

                PersonaRules.ImportProject(PersonaRules.Directory.GetRelativePath(PersonaEngine.EngineDirectroy.Combine("\\Programs\\PersonaEngine.csproj.props")));

                PersonaRules.BeginItemGroup();
                {
                    PersonaRules.AddReference(SharedProjectFile);
                }
                PersonaRules.EndItemGroup();

                PersonaRules.BeginItemGroup();
                {
                    PersonaRules.AddCompilePath("..\\..\\..\\Programs\\ModuleBuildTool\\ModuleRules\\*.cs");
                    PersonaRules.AddCompilePath("..\\..\\..\\Source\\**\\*.cs");
                }
                PersonaRules.EndItemGroup();
            }
            PersonaRules.EndProject();

            PersonaRules.FinishFile();

            return PersonaRules;
        }
    }
}