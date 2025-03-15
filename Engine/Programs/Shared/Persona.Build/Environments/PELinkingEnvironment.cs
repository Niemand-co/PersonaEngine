using System;
using System.Collections.Generic;

namespace PersonaBuildTool
{

    public class PELinkingEnvironment
    {
        public HashSet<string> SystemLibraryDirectories;

        public HashSet<string> DependencyLibraries;

        public PELinkingEnvironment()
        {
            SystemLibraryDirectories = new HashSet<string>();
            DependencyLibraries = new HashSet<string>();
        }

        public PELinkingEnvironment(PELinkingEnvironment Other)
        {
            SystemLibraryDirectories = Other.SystemLibraryDirectories;
            DependencyLibraries = Other.DependencyLibraries;
        }
    }

}