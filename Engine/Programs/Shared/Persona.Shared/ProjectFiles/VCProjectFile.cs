using Persona;
using System.IO;
using System.Reflection.Emit;
using static GlobalConfigurationPlatforms;

public class VCProjectFile : FileReference
{
    public static string FileExtension = ".vcxproj";

    string Content;

    string CurrentTab;

    public string MD5Hash { get; }

    public VCProjectFile(string InName, DirectoryReference InDirectory)
        : base(InName + FileExtension, InDirectory)
    {
        Content = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n";
        MD5Hash = PersonaEngine.MakeMD5(GetFullName());
    }

    void MoveTab()
    {
        int LastBackSlashIndex = CurrentTab.LastIndexOf('\t');
        if (LastBackSlashIndex >= 0)
        {
            CurrentTab = CurrentTab.Substring(0, LastBackSlashIndex);
        }
    }

    public void WriteLine(string Line)
    {
        Content += (CurrentTab + Line);
    }

    public void BeginProject()
    {
        Content += "<Project DefaultTargets=\"Build\" ToolsVersion=\"17.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">\r\n";
        CurrentTab += "\t";
    }

    public void EndProject()
    {
        CurrentTab = "";
        Content += "</Project>";
    }

    public void BeginItemGroup()
    {
        Content += (CurrentTab + "<ItemGroup>\r\n");
        CurrentTab += "\t";
    }

    public void BeginItemGroup(string Label)
    {
        Content += (CurrentTab + "<ItemGroup Label=\"" + Label + "\">\r\n");
        CurrentTab += "\t";
    }

    public void EndItemGroup()
    {
        MoveTab();
        Content += (CurrentTab + "</ItemGroup>\r\n");
    }

    public void AddConfiguration(string Configuration, string Platform)
    {
        Content += (CurrentTab + "<ProjectConfiguration Include=\"" + Configuration + "|" + Platform + "\">\r\n");
        Content += (CurrentTab + "\t<Configuration>" + Configuration + "</Configuration>\r\n");
        Content += (CurrentTab + "\t<Platform>" + Platform + "</Platform>\r\n");
        Content += (CurrentTab + "</ProjectConfiguration>\r\n");
    }

    public void Insert(string Tab, string Attribute)
    {
        Content += (CurrentTab + "<" + Tab + ">" + Attribute + "</" + Tab + ">\r\n");
    }

    public void Insert(string Tab)
    {
        Content += (CurrentTab + "<" + Tab + " />\r\n");
    }

    public void BeginPropertyGroup(string Label)
    {
        Content += (CurrentTab + "<PropertyGroup Label=\"" + Label + "\">\r\n");
        CurrentTab += "\t";
    }

    public void BeginPropertyGroupWithCondition(string Configuration, string Platform)
    {
        Content += (CurrentTab + "<PropertyGroup Condition=\"\'$(Configuration)|$(Platform)\'==\'" + Configuration + "|" + Platform + "\'\">\r\n");
        CurrentTab += "\t";
    }

    public void BeginPropertyGroup(string Configuration, string Platform, string Label)
    {
        Content += (CurrentTab + "<PropertyGroup Condition=\"\'$(Configuration)|$(Platform)\'==\'" + Configuration + "|" + Platform + "\'\" Label=\"" + Label + "\">\r\n");
        CurrentTab += "\t";
    }

    public void EndPropertyGroup()
    {
        MoveTab();
        Content += (CurrentTab + "</PropertyGroup>\r\n");
    }

    public void AddSourceFile(FileReference SourceFile)
    {
        Content += (CurrentTab + "<ClCompile Include=\"" + SourceFile.GetFullName() + "\" />\r\n");
    }

    public void BeginSourceFile(FileReference SourceFile)
    {
        Content += (CurrentTab + "<ClCompile Include=\"" + SourceFile.GetFullName() + "\">\r\n");
        CurrentTab += "\t";
    }

    public void EndSourceFile()
    {
        MoveTab();
        Content += (CurrentTab + "</ClCompile>\r\n");
    }

    public void AddHeaderFile(FileReference HeaderFile)
    {
        Content += (CurrentTab + "<ClInclude Include=\"" + HeaderFile.GetFullName() + "\" />\r\n");
    }

    public void BeginHeaderFile(FileReference SourceFile)
    {
        Content += (CurrentTab + "<ClInclude Include=\"" + SourceFile.GetFullName() + "\">\r\n");
        CurrentTab += "\t";
    }

    public void EndHeaderFile()
    {
        MoveTab();
        Content += (CurrentTab + "</ClInclude>\r\n");
    }

    public void AddOtherFile(FileReference OtherFile)
    {
        Content += (CurrentTab + "<None Include=\"" + OtherFile.GetFullName() + "\" />\r\n");
    }

    public void AddAdditionalIncludeDirectories(string AdditionalIncludeDirectories)
    {
        Content += (CurrentTab + "<AdditionalIncludeDirectories>" + AdditionalIncludeDirectories + "</AdditionalIncludeDirectories>\r\n");
    }

    public void AddForcedIncludeFiles(string ForcedIncludeDirectories)
    {
        Content += (CurrentTab + "<ForcedIncludeFiles>" + ForcedIncludeDirectories + "</ForcedIncludeFiles>\r\n");
    }

    public void AddAdditionalOptions(string AdditionalOptions)
    {
        Content += (CurrentTab += "<AdditionalOptions>$(AdditionalOptions) " + AdditionalOptions + "</AdditionalOptions>\r\n");
    }

    public void ImportProject(string ProjectName)
    {
        Content += (CurrentTab + "<Import Project=\"" + ProjectName + "\" />\r\n");
    }

    public void ImportProject(string ProjectName, string Condition, string Label)
    {
        Content += (CurrentTab + "<Import Project=\"" + ProjectName + "\"" + " Condition=\"" + Condition + "\"" + " Label=\"" + Label + "\" />\r\n");
    }

    public void BeginImportGroup(string Configuration, string Platform, string Label)
    {
        Content += (CurrentTab + "<ImportGroup Condition=\"\'$(Configuration)|$(Platform)\'==\'" + Configuration + "|" + Platform + "\'\" Label=\"" + Label + "\">\r\n");
        CurrentTab += "\t";
    }

    public void EndImportGroup()
    {
        MoveTab();
        Content += (CurrentTab + "</ImportGroup>\r\n");
    }

    public void FinishFile()
    {
        WriteFileContent(Content, FileMode.Create);
    }
}