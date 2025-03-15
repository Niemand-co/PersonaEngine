using Persona;
using System;
using System.Collections.Generic;

namespace PersonaBuildTool
{
    public enum EPrecompiledHeaderAction
    {
        EPCHA_None,
        EPCHA_Create,
        EPCHA_Include,
    }

    public class PECompilingEnvironment
    {
        public HashSet<string> SystemIncludeDirectories;

        public HashSet<string> UserIncludeDirectories;

        public List<string> Definitions = new List<string>();

        public FileReference? PrecompiledHeaderFileName;

        public EPrecompiledHeaderAction PrecompiledHeaderAction;

        public PECompilingEnvironment(PersonaBuildToolChain ToolChain)
        {
            SystemIncludeDirectories = new HashSet<string>();
            UserIncludeDirectories = new HashSet<string>();
            PrecompiledHeaderFileName = null;
            PrecompiledHeaderAction = EPrecompiledHeaderAction.EPCHA_None;
        }

        public PECompilingEnvironment(PECompilingEnvironment Other)
        {
            SystemIncludeDirectories = Other.SystemIncludeDirectories;
            UserIncludeDirectories = Other.UserIncludeDirectories;
            PrecompiledHeaderFileName = Other.PrecompiledHeaderFileName;
            PrecompiledHeaderAction = Other.PrecompiledHeaderAction;
        }
    }

}