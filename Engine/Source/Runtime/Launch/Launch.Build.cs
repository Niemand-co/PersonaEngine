using Persona;
using PersonaBuildTool;

public class Launch : ModuleRules
{
    public Launch(DirectoryReference Directory) : base(Directory)
    {
        PublicDependencyModuleNames.AddRange(
            new string[]
            {

            });

        PrivateDependencyModuleNames.AddRange(
            new string[]
            {
                "Core",
                "Engine",
            });
    }
}