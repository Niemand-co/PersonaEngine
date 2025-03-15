using Persona;
using System.IO;
using System.Collections.Generic;

public class CSSolutionFile : FileReference
{
    static public string FileExtension = ".sln";

    string Content;

    string CurrentTab;

    public CSSolutionFile(string InName, DirectoryReference InDirectory)
        : base(InName + FileExtension, InDirectory)
    {
        Content = "Microsoft Visual Studio Solution File, Format Version 12.00\r\n" +
                  "# Visual Studio Version 17\r\n" +
                  "VisualStudioVersion = 17.0.31314.256\r\n" +
                  "MinimumVisualStudioVersion = 10.0.40219.1\r\n";
    }

    void MoveTab()
    {
        int LastBackSlashIndex = CurrentTab.LastIndexOf('\t');
        if (LastBackSlashIndex >= 0)
        {
            CurrentTab = CurrentTab.Substring(0, LastBackSlashIndex);
        }
    }

    public void BeginCSProject(string ProjectName, CSProjectFile ProjectFile)
    {
        Content += ("Project(\"{9A19103F-16F7-4668-BE54-9A1E7A4F7556}\") = \"" + ProjectName + "\", \"" + Directory.GetRelativePath(ProjectFile.GetFullName()) + "\", \"{" + ProjectFile.MD5Hash + "}\"\r\n");
        CurrentTab += "\t";
    }

    public void EndCSProject()
    {
        MoveTab();
        Content += "EndProject\r\n";
    }

    public void AddProjectDependency(List<CSProjectFile> ProjectFile)
    {
        Content += (CurrentTab + "ProjectSection(ProjectDependencies) = postProject\r\n");
        foreach(CSProjectFile Project in ProjectFile)
        {
            Content += (CurrentTab + "\t{" + Project.MD5Hash + "} = {" + Project.MD5Hash + "}\r\n");
        }
        Content += (CurrentTab + "EndProjectSection\r\n");
    }

    public void BeginGlobal()
    {
        Content += ("Global\r\n");
        CurrentTab += "\t";
    }

    public void EndGlobal()
    {
        MoveTab();
        Content += ("EndGlobal\r\n");
    }

    public void BeginGlobalSection(string Level, string Priority)
    {
        Content += (CurrentTab + "GlobalSection(" + Level + ") = " + Priority + "\r\n");
        CurrentTab += "\t";
    }

    public void EndGlobalSection()
    {
        MoveTab();
        Content += (CurrentTab + "EndGlobalSection\r\n");
    }

    public void AddSolutionConfiguration(string Configuration, string Platform)
    {
        Content += (CurrentTab + Configuration + "|" + Platform + " = " + Configuration + "|" + Platform + "\r\n");
    }

    public void SetProjectConfiguration(CSProjectFile ProjectFile)
    {
        for (int ConfigurationIndex = 0; ConfigurationIndex < GlobalConfigurationPlatforms.CSConfigurations.Length; ConfigurationIndex++)
        {
            string Configuration = GlobalConfigurationPlatforms.CSConfigurations[ConfigurationIndex];
            Content += (CurrentTab + "{" + ProjectFile.MD5Hash + "}." + Configuration + "|Any CPU.ActiveCfg = " + Configuration + "|Any CPU\r\n");
            Content += (CurrentTab + "{" + ProjectFile.MD5Hash + "}." + Configuration + "|Any CPU.Build.0 = " + Configuration + "|Any CPU\r\n");
        }
    }

    public void FinishFile()
    {
        WriteFileContent(Content, FileMode.Create);
    }
}