using System;

namespace PersonaBuildTool
{

    public abstract class PersonaBuildPlatform
    {
        public PEBuildPlatformSDK SDK { get; set; }

        public abstract PersonaBuildToolChain CreateToolChain(PersonaBuildTarget Target);

        public abstract void SetGlobalEnvironment(PersonaBuildTarget Target, ref PECompilingEnvironment CompilingEnvironment, ref PELinkingEnvironment LinkingEnvironment);
    }

}