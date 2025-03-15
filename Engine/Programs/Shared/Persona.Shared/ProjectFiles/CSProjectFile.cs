using Persona;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class CSProjectFile : FileReference
{
    static string FileExtension = ".csproj";

    string Content;

    string CurrentTab;

    public string MD5Hash { get; }

    public CSProjectFile(string InName, DirectoryReference InDirectory)
        : base(InName + FileExtension, InDirectory)
    {
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

    public void BeginProject()
    {
        Content += "<Project Sdk=\"Microsoft.NET.Sdk\">\r\n";
        CurrentTab += "\t";
    }

    public void EndProject()
    {
        MoveTab();
        Content += "</Project>\r\n";
    }

    public void BeginPropertyGroup()
    {
        Content += (CurrentTab + "<PropertyGroup>\r\n");
        CurrentTab += "\t";
    }

    public void EndPropertyGroup()
    {
        MoveTab();
        Content += (CurrentTab + "</PropertyGroup>\r\n");
    }

    public void Insert(string Tab, string Attribute)
    {
        Content += (CurrentTab + "<" + Tab + ">" + Attribute + "</" + Tab + ">\r\n");
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

    public void ImportProject(string Project)
    {
        Content += (CurrentTab + "<Import Project=\"" + Project + "\" />\r\n");
    }

    public void AddReference(CSProjectFile Project)
    {
        Content += (CurrentTab + "<ProjectReference Include=\"" + Directory.GetRelativePath(Project.GetFullName()) + "\" />\r\n");
    }

    public void AddCompilePath(string Path)
    {
        Content += (CurrentTab += "<Compile Include=\"" + Path + "\" />\r\n");
    }

    public void FinishFile()
    {
        WriteFileContent(Content, FileMode.Create);
    }
}