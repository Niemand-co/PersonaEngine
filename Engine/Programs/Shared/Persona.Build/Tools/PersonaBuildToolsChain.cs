using Persona;
using System;
using System.Collections.Generic;

namespace PersonaBuildTool
{

    public abstract class PersonaBuildToolChain
    {
        public abstract void SetupEnvironment(ref PECompilingEnvironment CompilingEnvironment, ref PELinkingEnvironment LinkingEnvironment);

        public abstract FileReference CompileCPPFile(FileReference CPPFile, string CompilingArguments, DirectoryReference OutputDir);

        public abstract FileReference LinkObjectFiles(List<FileReference> LinkInputFile, string LinkingArguments, DirectoryReference OutputDir, string ModuleName);

        public abstract FileReference ExportExcutableFile(List<FileReference> LibraryFiles, DirectoryReference OutputDir, string ExcutableFileName);
    }

}