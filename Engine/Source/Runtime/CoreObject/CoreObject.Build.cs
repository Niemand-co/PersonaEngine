using Persona;
using PersonaBuildTool;

public class CoreObject : ModuleRules
{
    public CoreObject(DirectoryReference Directory) : base(Directory)
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