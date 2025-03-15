using Persona;

namespace ModuleBuildTool
{
    public class ModuleBuildTool
    {
        public static int Main(string[] ArgumentsArray)
        {
            CSProjectFile PERules = PersonaRulesGenerator.CreatePersonaRules();
            PersonaBuildToolGenerator.CreatePersonaBuildTool(PERules);
            return 0;
        }
    }
}