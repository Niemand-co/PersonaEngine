using Persona;
using System;
using System.Security.Cryptography.X509Certificates;

namespace PersonaBuildTool
{

    public class PEBuildModule
    {
        public ModuleRules Rules { get; }

        public PEBuildModule(ModuleRules InRules)
        {
            Rules = InRules;
        }

        public virtual FileReference Build(PersonaBuildTarget Target, PersonaBuildToolChain ToolChain, PECompilingEnvironment CompilingEnvironment, PELinkingEnvironment LinkingEnvironment)
        {
            return null;
        }
    }

}