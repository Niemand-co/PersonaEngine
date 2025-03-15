using Persona;
using PersonaBuildTool;

public class Editor : ModuleRules
{
    public Editor(DirectoryReference Directory) : base(Directory)
    {
        PublicDependencyModuleNames.AddRange(
            new string[]
            {
                
            });

        PrivateDependencyModuleNames.AddRange(
            new string[]
            {
                "Core",
                "Slate",
            });
    }
}