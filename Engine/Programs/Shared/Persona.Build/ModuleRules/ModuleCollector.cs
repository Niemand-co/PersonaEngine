using Persona;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PersonaBuildTool
{

    public class ModuleCollector
    {
        static Dictionary<string, int> ModulesByPath = new Dictionary<string, int>();
        static Dictionary<string, int> ModulesByName = new Dictionary<string, int>();

        static public List<ModuleRules> Modules { get; set; }

        static public List<HashSet<int>> ModulesDependencies = null;

        static public Dictionary<string, string> TimeStamps = new Dictionary<string, string>();

        static public void Initialize(DirectoryReference Directory, bool bBuild = false)
        {
            Modules = new List<ModuleRules>();
            Directory.Iterate(File =>
            {
                if(File.Name.EndsWith(".Build.cs"))
                {
                    string ModuelTypeName = File.Name.Substring(0, File.Name.IndexOf('.'));
                    ModuleRules? Module = ModuleRules.CreateModule(ModuelTypeName, File.Directory);

                    if(Module != null)
                    {
                        int ModuleIndex = Modules.Count;
                        Modules.Add(Module);
                        ModulesByPath.Add(File.Directory.Directory, ModuleIndex);
                        ModulesByName.Add(ModuelTypeName, ModuleIndex);
                    }
                }

                return true;
            }, null);

            if(bBuild)
            {
                ModulesDependencies = new List<HashSet<int>>(Modules.Count);
                for (int ModuleIndex = 0; ModuleIndex < Modules.Count; ModuleIndex++)
                {
                    ModulesDependencies.Add(new HashSet<int>());
                }
            }

            try
            { 
                for (int ModuleIndex = 0; ModuleIndex < Modules.Count; ModuleIndex++)
                {

                    HashSet<int> ModuleMap = new HashSet<int>();
                    Stack<int> Path = new Stack<int>();
                    int BreakPoint = IteratePublicDependenciesTree(ModuleMap, ModuleIndex, ref Path, bBuild ? ModuleIndex : null);
                    BreakPoint = IteratePrivateDependenciesTree(ModuleMap, ModuleIndex, ref Path, bBuild ? ModuleIndex : null, out int Priority);
                    if (BreakPoint != -1)
                    {
                        Stack<int> ReversePath = new Stack<int>();
                        string ModulePathStr = Modules[BreakPoint].ToString();
                        while (Path.Count > 0)
                        {
                            int Node = Path.Pop();
                            if(Node != BreakPoint)
                            {
                                ReversePath.Push(Node);
                            }
                            else
                            {
                                break;
                            }
                        }

                        while(ReversePath.Count > 0)
                        {
                            ModulePathStr += ("->" + Modules[ReversePath.Pop()].ToString());
                        }
                        ModulePathStr += ("->" + Modules[BreakPoint].ToString());

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("There's circular reference among modules:\r\n\t{0}\r\nPlease devise reference dependencies.", ModulePathStr);
                        
                        throw new Exception("ERROR: Circular Reference.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.Read();
                Environment.Exit(-1);
            }
        }

        static public ModuleRules GetModuleByPath(FileReference File)
        {
            int PrivateIndex = File.Directory.Directory.LastIndexOf("\\Private");
            int PublicIndex = File.Directory.Directory.LastIndexOf("\\Public");

            string Directory = null;
            if (PrivateIndex != -1)
            {
                Directory = File.Directory.Directory.Substring(0, PrivateIndex);
            }
            else if(PublicIndex != -1)
            {
                Directory = File.Directory.Directory.Substring(0, PublicIndex);
            }

            if(Directory != null)
            {
                return Modules[ModulesByPath[Directory]];
            }
            else
            {
                return null;
            }
        }

        static public ModuleRules GetModuleByName(string ModuleTypeName)
        {
            return Modules[ModulesByName[ModuleTypeName]];
        }

        static public List<ModuleRules> GetModulesToBuild()
        {
            FileReference EngineFileTimeStamps = new FileReference("PersonaEngineFiles.txt", PersonaEngine.EngineDirectroy + "\\Intermediate\\Build");

            if (!EngineFileTimeStamps.Exists())
            {
                for (int ModuleIndex = 0; ModuleIndex < Modules.Count; ++ModuleIndex)
                {
                    Modules[ModuleIndex].bSkipBuild = false;
                }
            }
            else
            {
                EngineFileTimeStamps.ReadToString(out string InTimeSampsStr);
                string[] InTimeStamps = InTimeSampsStr.Split("\r\n");
                foreach (string InTimeStamp in InTimeStamps)
                {
                    int SeperateIndex = InTimeStamp.LastIndexOf(':');
                    if( SeperateIndex < 0 )
                    {
                        continue;
                    }
                    string FileName = InTimeStamp.Substring(0, SeperateIndex);
                    string TimeStampTicks = InTimeStamp.Substring(SeperateIndex + 1);
                    TimeStamps.Add(FileName, TimeStampTicks);
                }

                for (int ModuleIndex = 0; ModuleIndex < Modules.Count; ++ModuleIndex)
                {
                    Modules[ModuleIndex].bSkipBuild = true;

                    ModuleRules CurModule = Modules[ModuleIndex];
                    CurModule.PublicPath.Iterate(File =>
                    {
                        FileInfo Info = new FileInfo(File.GetFullName());
                        if (!TimeStamps.ContainsKey(File.GetFullName()) || TimeStamps[File.GetFullName()] != Info.LastWriteTime.Ticks.ToString())
                        {
                            if (File.Name.EndsWith(".h"))
                            {
                                foreach (int Dependency in ModulesDependencies[ModuleIndex])
                                {
                                    Modules[Dependency].bSkipBuild = false;
                                }
                                return false;
                            }
                            else if (File.Name.EndsWith(".cpp"))
                            {
                                Modules[ModuleIndex].bSkipBuild = false;
                                
                            }
                        }

                        return true;
                    }, null);

                    CurModule.PrivatePath.Iterate(File =>
                    {
                        FileInfo Info = new FileInfo(File.GetFullName());
                        if (!TimeStamps.ContainsKey(File.GetFullName()) || TimeStamps[File.GetFullName()] != Info.LastWriteTime.Ticks.ToString())
                        {
                            if (File.Name.EndsWith(".h") || File.Name.EndsWith(".cpp"))
                            {
                                Modules[ModuleIndex].bSkipBuild = false;
                                return false;
                            }
                        }

                        return true;
                    }, null);
                }
            }
            return Modules;
        }

        static public List<ModuleRules> GetModulesToRebuild()
        {
            for (int ModuleIndex = 0; ModuleIndex < Modules.Count; ++ModuleIndex)
            {
                Modules[ModuleIndex].bSkipBuild = false;
            }

            return Modules;
        }

        static public void FinishBuildingModels()
        {
            string OutTimeStampsStr = "";
            {
                for (int ModuleIndex = 0; ModuleIndex < Modules.Count; ++ModuleIndex)
                {

                    ModuleRules CurModule = Modules[ModuleIndex];
                    CurModule.PublicPath.Iterate(File =>
                    {
                        FileInfo Info = new FileInfo(File.GetFullName());
                        DateTime TimeStamp = Info.LastWriteTime;
                        OutTimeStampsStr += (File.GetFullName() + ":" + TimeStamp.Ticks.ToString() + "\r\n");
                        return true;
                    }, null);

                    CurModule.PrivatePath.Iterate(File =>
                    {
                        FileInfo Info = new FileInfo(File.GetFullName());
                        DateTime TimeStamp = Info.LastWriteTime;
                        OutTimeStampsStr += (File.GetFullName() + ":" + TimeStamp.Ticks.ToString() + "\r\n");
                        return true;
                    }, null);

                }
            }

            FileReference EngineFileTimeStamps = new FileReference("PersonaEngineFiles.txt", PersonaEngine.EngineDirectroy + "\\Intermediate\\Build");
            if (EngineFileTimeStamps.Exists())
            {
                EngineFileTimeStamps.WriteFileContent(OutTimeStampsStr, FileMode.Truncate);
            }
            else
            {
                EngineFileTimeStamps.WriteFileContent(OutTimeStampsStr, FileMode.Create);
            }
        }

        static private int IteratePublicDependenciesTree(HashSet<int> CurModuleMap, int CurIndex, ref Stack<int> CurPath, int? BaseModule)
        {
            if(CurModuleMap.Contains(CurIndex))
            {
                return CurIndex;
            }

            if(BaseModule != null)
            {
                ModulesDependencies[CurIndex].Add((int)BaseModule);
            }

            ModuleRules CurModule = Modules[CurIndex];
            CurModuleMap.Add(CurIndex);
            CurPath.Push(CurIndex);
            for (int DependencyIndex = 0; DependencyIndex < CurModule.PublicDependencyModuleNames.Count; DependencyIndex++)
            {
                int Dependency = ModulesByName[CurModule.PublicDependencyModuleNames[DependencyIndex]];
                int Result = IteratePublicDependenciesTree(CurModuleMap, Dependency, ref CurPath, BaseModule);
                if(Result != -1)
                {
                    return Result;
                }
            }
            CurModuleMap.Remove(CurIndex);
            CurPath.Pop();

            return -1;
        }

        static private int IteratePrivateDependenciesTree(HashSet<int> CurModuleMap, int CurIndex, ref Stack<int> CurPath, int? BaseModule, out int OutPriority)
        {
            if (CurModuleMap.Contains(CurIndex))
            {
                OutPriority = -1;
                return CurIndex;
            }

            if (BaseModule != null)
            {
                ModulesDependencies[CurIndex].Add((int)BaseModule);
            }

            ModuleRules CurModule = Modules[CurIndex];

            if (CurModule.PrivateDependencyModuleNames.Count == 0)
            {
                CurModule.PrioriryToCompile = 0;
            }
            else
            {
                CurModuleMap.Add(CurIndex);
                CurPath.Push(CurIndex);
                for (int DependencyIndex = 0; DependencyIndex < CurModule.PrivateDependencyModuleNames.Count; DependencyIndex++)
                {
                    int Dependency = ModulesByName[CurModule.PrivateDependencyModuleNames[DependencyIndex]];
                    int Result = IteratePrivateDependenciesTree(CurModuleMap, Dependency, ref CurPath, BaseModule, out int Priority);
                    if (Result != -1)
                    {
                        OutPriority = -1;
                        return Result;
                    }

                    CurModule.PrioriryToCompile = Math.Max(CurModule.PrioriryToCompile, Priority);
                }
                CurModuleMap.Remove(CurIndex);
                CurPath.Pop();
            }

            OutPriority = CurModule.PrioriryToCompile + 1;

            return -1;
        }
    }

}