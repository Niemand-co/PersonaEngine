using Persona;
using System;
using System.Collections.Generic;
using System.IO;

namespace PersonaBuildTool
{

    public static class VCProjectFileGenerator
    {
        public static VCProjectFile CreateProjectFile(DirectoryReference Directory, string ProjectName)
        {
            DirectoryReference ProjectFileDirectory = PersonaEngine.EngineDirectroy + "\\Intermediate\\ProjectFiles";
            VCProjectFile ProjectFile = new VCProjectFile(ProjectName, ProjectFileDirectory);

            ProjectFile.BeginProject();
            {
                ProjectFile.BeginItemGroup("ProjectConfigurations");
                {
                    for(int ConfigurationIndex = 0; ConfigurationIndex < GlobalConfigurationPlatforms.Configurations.Length; ConfigurationIndex++)
                    {
                        for(int PlatformIndex = 0; PlatformIndex < GlobalConfigurationPlatforms.Platforms.Length; PlatformIndex++)
                        {
                            ProjectFile.AddConfiguration(GlobalConfigurationPlatforms.Configurations[ConfigurationIndex][1], GlobalConfigurationPlatforms.Platforms[PlatformIndex][1]);
                        }
                    }
                }
                ProjectFile.EndItemGroup();

                ProjectFile.ImportProject("$(VCTargetsPath)\\Microsoft.Cpp.default.props");
                ProjectFile.ImportProject("$(VCTargetsPath)\\Microsoft.Cpp.props");

                ProjectFile.BeginPropertyGroup("Globals");
                {
                    ProjectFile.Insert("ProjectGuid", "{" + ProjectFile.MD5Hash + "}");
                    ProjectFile.Insert("VCProjectVersion", "17.0");
                    ProjectFile.Insert("RootNamespace", "PE");
                }
                ProjectFile.EndPropertyGroup();

                for (int ConfigurationIndex = 0; ConfigurationIndex < GlobalConfigurationPlatforms.Configurations.Length; ConfigurationIndex++)
                {
                    for (int PlatformIndex = 0; PlatformIndex < GlobalConfigurationPlatforms.Platforms.Length; PlatformIndex++)
                    {
                        ProjectFile.BeginPropertyGroup(GlobalConfigurationPlatforms.Configurations[ConfigurationIndex][1], GlobalConfigurationPlatforms.Platforms[PlatformIndex][1], "Configuration");
                        {
                            ProjectFile.Insert("ConfigurationType", "Makefile");
                            ProjectFile.Insert("PlatformToolset", "v143");
                        }
                        ProjectFile.EndPropertyGroup();
                    }
                }

                for (int ConfigurationIndex = 0; ConfigurationIndex < GlobalConfigurationPlatforms.Configurations.Length; ConfigurationIndex++)
                {
                    for (int PlatformIndex = 0; PlatformIndex < GlobalConfigurationPlatforms.Platforms.Length; PlatformIndex++)
                    {
                        ProjectFile.BeginImportGroup(GlobalConfigurationPlatforms.Configurations[ConfigurationIndex][1], GlobalConfigurationPlatforms.Platforms[PlatformIndex][1], "PropertySheets");
                        {
                            ProjectFile.ImportProject("$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props", "exists('$(UserRootDir)\\Microsoft.Cpp.$(Platform).user.props')", "LocalAppDataPlatform");
                        }
                        ProjectFile.EndImportGroup();

                        ProjectFile.BeginPropertyGroupWithCondition(GlobalConfigurationPlatforms.Configurations[ConfigurationIndex][1], GlobalConfigurationPlatforms.Platforms[PlatformIndex][1]);
                        {
                            ProjectFile.Insert("IncludePath");
                            ProjectFile.Insert("ReferencePath");
                            ProjectFile.Insert("LibraryPath");
                            ProjectFile.Insert("LibraryWPath");
                            ProjectFile.Insert("SourcePath");
                            ProjectFile.Insert("ExcludePath");
                            ProjectFile.Insert("NMakeBuildCommandLine", GlobalConfigurationPlatforms.BuildCommandLine[ConfigurationIndex][PlatformIndex]);
                            ProjectFile.Insert("NMakeReBuildCommandLine", GlobalConfigurationPlatforms.RebuildCommandLine[ConfigurationIndex][PlatformIndex]);
                            ProjectFile.Insert("NMakeCleanCommandLine", GlobalConfigurationPlatforms.CleanCommandLine[ConfigurationIndex][PlatformIndex]);
                            ProjectFile.Insert("NMakeOutput", GlobalConfigurationPlatforms.OutputTarget[ConfigurationIndex][PlatformIndex]);
                            ProjectFile.Insert("AdditionalOptions", "/std:c++17  /Zc:hiddenFriend /Zc:__cplusplus");
                        }
                        ProjectFile.EndPropertyGroup();
                    }
                }

                ProjectFile.BeginItemGroup();
                {
                    Directory.Iterate(File => {

                        ModuleRules Module = ModuleCollector.GetModuleByPath(File);

                        if (File.Name.EndsWith(".h"))
                        {
                            ProjectFile.BeginHeaderFile(File);
                            {
                                string AdditiionalIncludeDirectories = "$(NMakeIncludeSearchPath);" + ProjectFile.Directory.GetRelativePath(Module.PublicPath);
                                foreach (string ModuleTypeName in Module.PublicDependencyModuleNames)
                                {
                                    ModuleRules Dependency = ModuleCollector.GetModuleByName(ModuleTypeName);
                                    AdditiionalIncludeDirectories += ";" + ProjectFile.Directory.GetRelativePath(Dependency.PublicPath);
                                }
                                ProjectFile.AddAdditionalIncludeDirectories(AdditiionalIncludeDirectories);
                            }
                            ProjectFile.EndHeaderFile();
                        }
                        else if (File.Name.EndsWith(".cpp"))
                        {
                            ProjectFile.BeginSourceFile(File);
                            {
                                string AdditiionalIncludeDirectories = "$(NMakeIncludeSearchPath);" + PersonaEngine.EngineSourceDirectory.GetFullName() + ";" + ProjectFile.Directory.GetRelativePath(Module.PublicPath) + ";" + ProjectFile.Directory.GetRelativePath(Module.PrivatePath);
                                foreach (string ModuleTypeName in Module.PublicDependencyModuleNames)
                                {
                                    ModuleRules Dependency = ModuleCollector.GetModuleByName(ModuleTypeName);
                                    AdditiionalIncludeDirectories += ";" + ProjectFile.Directory.GetRelativePath(Dependency.PublicPath);
                                }
                                foreach (string ModuleTypeName in Module.PrivateDependencyModuleNames)
                                {
                                    ModuleRules Dependency = ModuleCollector.GetModuleByName(ModuleTypeName);
                                    AdditiionalIncludeDirectories += ";" + ProjectFile.Directory.GetRelativePath(Dependency.PublicPath);
                                }
                                ProjectFile.AddAdditionalIncludeDirectories(AdditiionalIncludeDirectories);
                                ProjectFile.AddForcedIncludeFiles(string.Format("$(SolutionDir)Engine\\Intermediate\\Build\\PCHHeaders\\PCH.{0}.h", Module.ToString()));
                                ProjectFile.AddAdditionalOptions(string.Format("/Yu\"$(SolutionDir)Engine\\Intermediate\\Build\\PCHHeaders\\PCH.{0}.h\"", Module.ToString()));
                            }
                            ProjectFile.EndSourceFile();
                        }
                        else
                        {
                            ProjectFile.AddOtherFile(File);
                        }

                        return true;
                    }, null);
                }
                ProjectFile.EndItemGroup();

                ProjectFile.ImportProject("$(VCTargetsPath)\\Microsoft.Cpp.targets");
            }
            ProjectFile.EndProject();

            ProjectFile.FinishFile();

            CreateFiltersFile(Directory, ProjectName);

            return ProjectFile;
        }

        public static FileReference CreateFiltersFile(DirectoryReference Directory, string ProjectName)
        {
            DirectoryReference FiltersFileDirectory = PersonaEngine.EngineDirectroy + "\\Intermediate\\ProjectFiles";
            VCFiletersFile FiltersFile = new VCFiletersFile(ProjectName, FiltersFileDirectory);

            FiltersFile.BeginProject();
            {
                FiltersFile.BeginItemGroup();
                {
                    Directory.Iterate(File =>
                    {
                        if (File.Name.EndsWith(".h"))
                        {
                            FiltersFile.AddHeaderFile(File);
                        }
                        else if (File.Name.EndsWith(".cpp"))
                        {
                            FiltersFile.AddSourceFile(File);
                        }
                        else
                        {
                            FiltersFile.AddOtherFile(File);
                        }

                        return true;
                    }, Directory =>
                    {
                        FiltersFile.AddDirectory(Directory);
                    });
                }
                FiltersFile.EndItemGroup();
            }
            FiltersFile.EndProject();

            FiltersFile.FinishFile();

            return FiltersFile;
        }

        public static FileReference CreateSolutionFile(List<VCProjectFile> CPPProjectFiles)
        {
            DirectoryReference SolutionFileDirectory = PersonaEngine.RootDirectory;
            VCSolutionFile SolutionFile = new VCSolutionFile("PE", SolutionFileDirectory);
            string ProgramsFolderMD5 = PersonaEngine.MakeMD5("Programs");
            string SharedFolderMD5 = PersonaEngine.MakeMD5("Shared");
            string RulesFolderMD5 = PersonaEngine.MakeMD5("Rules");
            string EngineFolderMD5 = PersonaEngine.MakeMD5("Engine");

            SolutionFile.BeginFolderProject("Programs", ProgramsFolderMD5);
            SolutionFile.EndProject();

            SolutionFile.BeginFolderProject("Shared", SharedFolderMD5);
            SolutionFile.EndProject();

            SolutionFile.BeginFolderProject("Rules", RulesFolderMD5);
            SolutionFile.EndProject();

            SolutionFile.BeginFolderProject("Engine", EngineFolderMD5);
            SolutionFile.EndProject();

            SolutionFile.BeginCPPProject("PE", CPPProjectFiles[0]);
            SolutionFile.EndProject();

            CSProjectFile SharedProjectFile = new CSProjectFile("Persona.Shared", PersonaEngine.EngineDirectroy.Combine("\\Programs\\Shared\\Persona.Shared"));
            SolutionFile.BeginCSProject("Persona.Shared", SharedProjectFile);
            SolutionFile.EndProject();

            CSProjectFile BuildProjectFile = new CSProjectFile("Persona.Build", PersonaEngine.EngineDirectroy.Combine("\\Programs\\Shared\\Persona.Build"));
            SolutionFile.BeginCSProject("Persona.Build", BuildProjectFile);
            SolutionFile.EndProject();

            CSProjectFile RulesProjectFile = new CSProjectFile("PERules", PersonaEngine.EngineDirectroy.Combine("\\Intermediate\\Build\\BuildRules"));
            SolutionFile.BeginCSProject("PERules", RulesProjectFile);
            SolutionFile.EndProject();

            CSProjectFile PersonaBuildToolProjectFile = new CSProjectFile("PersonaBuildTool", PersonaEngine.EngineDirectroy.Combine("\\Programs\\PersonaBuildTool"));
            SolutionFile.BeginCSProject("PersonaBuildTool", PersonaBuildToolProjectFile);
            SolutionFile.EndProject();

            SolutionFile.BeginGlobal();
            {
                SolutionFile.BeginGlobalSection("SolutionConfigurationPlatforms", "preSolution");
                {
                    for (int ConfigurationIndex = 0; ConfigurationIndex < GlobalConfigurationPlatforms.Configurations.Length; ConfigurationIndex++)
                    {
                        for (int PlatformIndex = 0; PlatformIndex < GlobalConfigurationPlatforms.Platforms.Length; PlatformIndex++)
                        {
                            SolutionFile.AddSolutionConfiguration(GlobalConfigurationPlatforms.Configurations[ConfigurationIndex][0], GlobalConfigurationPlatforms.Platforms[PlatformIndex][0]);
                        }
                    }
                }
                SolutionFile.EndGlobalSection();

                SolutionFile.BeginGlobalSection("ProjectConfigurationPlatforms", "postSolution");
                {
                    SolutionFile.SetProjectConfiguration(SharedProjectFile);
                    SolutionFile.SetProjectConfiguration(BuildProjectFile);
                    SolutionFile.SetProjectConfiguration(RulesProjectFile);
                    SolutionFile.SetProjectConfiguration(PersonaBuildToolProjectFile);
                    for(int ProjectFileIndex = 0; ProjectFileIndex < CPPProjectFiles.Count; ProjectFileIndex++)
                    {
                        SolutionFile.SetProjectConfiguration(CPPProjectFiles[ProjectFileIndex]);
                    }
                }
                SolutionFile.EndGlobalSection();

                SolutionFile.BeginGlobalSection("SolutionProperties", "preSolution");
                {
                    SolutionFile.AddCustomGlobalSection("HideSolutionNode = FALSE\r\n");
                }
                SolutionFile.EndGlobalSection();

                SolutionFile.BeginGlobalSection("NestedProjects", "preSolution");
                {
                    SolutionFile.SetNestedProject(CPPProjectFiles[0].MD5Hash, EngineFolderMD5);
                    SolutionFile.SetNestedProject(SharedFolderMD5, ProgramsFolderMD5);
                    SolutionFile.SetNestedProject(SharedProjectFile.MD5Hash, SharedFolderMD5);
                    SolutionFile.SetNestedProject(BuildProjectFile.MD5Hash, SharedFolderMD5);
                    SolutionFile.SetNestedProject(PersonaBuildToolProjectFile.MD5Hash, ProgramsFolderMD5);
                    SolutionFile.SetNestedProject(RulesProjectFile.MD5Hash, RulesFolderMD5);
                }
                SolutionFile.EndGlobalSection();
            }
            SolutionFile.EndGlobal();

            SolutionFile.FinishFile();

            return SolutionFile;
        }
    }

}