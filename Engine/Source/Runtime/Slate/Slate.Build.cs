using Persona;
using PersonaBuildTool;

public class Slate : ModuleRules
{
    public Slate(DirectoryReference Directory) : base(Directory)
    {
        PublicDependencyModuleNames.AddRange(
            new string[]
            {
            });

        PrivateDependencyModuleNames.AddRange(
            new string[]
            {
                "Core",
            });
    }
}