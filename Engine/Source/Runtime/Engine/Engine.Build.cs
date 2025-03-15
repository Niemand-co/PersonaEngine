using Persona;
using PersonaBuildTool;

public class Engine : ModuleRules
{
    public Engine(DirectoryReference Directory) : base(Directory)
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