using Persona;
using System.Diagnostics;
using System.IO;

public class VCFiletersFile : FileReference
{
    static string FileExtension = ".filters";

    string Content;

    string CurrentTab;

    public VCFiletersFile(string InName, DirectoryReference InDirectory)
        : base(InName + ".vcxproj" + FileExtension, InDirectory)
    {
        Content = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n";
    }

    void MoveTab()
    {
        int LastBackSlashIndex = CurrentTab.LastIndexOf('\t');
        if (LastBackSlashIndex >= 0)
        {
            CurrentTab = CurrentTab.Substring(0, LastBackSlashIndex);
        }
    }

    public void BeginProject()
    {
        Content += "<Project ToolsVersion=\"17.0\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">\r\n";
        CurrentTab += "\t";
    }

    public void EndProject()
    {
        MoveTab();
        Content += "</Project>\r\n";
    }

    public void BeginItemGroup()
    {
        Content += (CurrentTab + "<ItemGroup>\r\n");
        CurrentTab += "\t";
    }

    public void EndItemGroup()
    {
        MoveTab();
        Content += (CurrentTab + "</ItemGroup>\r\n");
    }

    public void AddSourceFile(FileReference SourceFile)
    {
        Content += (CurrentTab + "<ClCompile Include=\"" + SourceFile.GetFullName() + "\">\r\n");
        Content += (CurrentTab + "\t<Filter>" + PersonaEngine.EngineDirectroy.GetRelativePath(SourceFile.Directory) + "</Filter>\r\n");
        Content += (CurrentTab + "</ClCompile>\r\n");
    }

    public void AddHeaderFile(FileReference HeaderFile)
    {
        Content += (CurrentTab + "<ClInclude Include=\"" + HeaderFile.GetFullName() + "\">\r\n");
        Content += (CurrentTab + "\t<Filter>" + PersonaEngine.EngineDirectroy.GetRelativePath(HeaderFile.Directory) + "</Filter>\r\n");
        Content += (CurrentTab + "</ClInclude>\r\n");
    }

    public void AddOtherFile(FileReference OtherFile)
    {
        Content += (CurrentTab + "<None Include=\"" + Directory.GetRelativePath(OtherFile.GetFullName()) + "\">\r\n");
        Content += (CurrentTab + "\t<Filter>" + PersonaEngine.EngineDirectroy.GetRelativePath(OtherFile.Directory) + "</Filter>\r\n");
        Content += (CurrentTab + "</None>\r\n");
    }

    public void AddDirectory(DirectoryReference InDirectory)
    {
        Content += (CurrentTab + "<Filter Include=\"" + PersonaEngine.EngineDirectroy.GetRelativePath(InDirectory) + "\">\r\n");
        Content += (CurrentTab + "\t<UniqueIdentifier>{" + PersonaEngine.MakeMD5(InDirectory.GetFullName()) + "}</UniqueIdentifier>\r\n");
        Content += (CurrentTab + "</Filter>\r\n");
    }

    public void FinishFile()
    {
        WriteFileContent(Content, FileMode.Create);
    }
}