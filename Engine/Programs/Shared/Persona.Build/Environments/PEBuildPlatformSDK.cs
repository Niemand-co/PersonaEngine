using Persona;
using System;
using System.Collections.Generic;

namespace PersonaBuildTool
{

    public abstract class PEBuildPlatformSDK
    {
        public abstract List<string> GetPlatformSDKIncludes();

        public abstract List<string> GetPlatformSDKLibraries();
    }

}