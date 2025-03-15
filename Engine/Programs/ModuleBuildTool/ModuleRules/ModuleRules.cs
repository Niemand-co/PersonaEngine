using Persona;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PersonaBuildTool
{
    public class ModuleRules
    {
        public List<string> PublicIncludePathModuleNames = new List<string>();

        public List<string> PublicDependencyModuleNames = new List<string>();

        public List<string> PrivateIncludePathModuleNames = new List<string>();

        public List<string> PrivateDependencyModuleNames = new List<string>();

        public List<string> Definitions = new List<string>();

        public DirectoryReference PublicPath { get; }

        public DirectoryReference PrivatePath { get; }

        public DirectoryReference ModulePath { get; }

        public int PrioriryToCompile = -1;

        public bool bSkipBuild = false;

        public string SharedPCHHeaderFile = null;

        public string PrivatePCHHeaderFile = null;

        public ModuleRules(DirectoryReference Directory) 
        {
            ModulePath = Directory;
            PublicPath = Directory.Combine("\\Public");
            PrivatePath = Directory.Combine("\\Private");
        }

        public static ModuleRules? CreateModule(string ModuleTypeName, DirectoryReference Directory)
        {
            Type ModuleType = Type.GetType(ModuleTypeName);
            ConstructorInfo Constructor = ModuleType.GetConstructor(new Type[] { typeof(DirectoryReference) });

            if (Constructor != null)
            {
                Object[] Parameters = new Object[] { Directory };
                Object ModuleObject = Constructor.Invoke(Parameters);
                if (ModuleObject is ModuleRules Module)
                {
                    return Module;
                }
            }
            else
            {
                throw new Exception("Failed get contructor from type.");
            }

            return null;
        }
    }
}