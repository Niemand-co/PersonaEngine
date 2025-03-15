using Persona;
using System;
using System.IO;
using System.Security.Cryptography;

public class VCSolutionFile : FileReference
{
    public static string FileExtension = ".sln";

    string Content;

    string CurrentTab;

    public VCSolutionFile(string InName, DirectoryReference InDirectory)
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

    public void BeginCPPProject(string ProjectName, VCProjectFile ProjectFile)
    {
        Content += ("Project(\"{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}\") = \"" + ProjectName + "\", \"" + Directory.GetRelativePath(ProjectFile.GetFullName()) + "\", \"{" + ProjectFile.MD5Hash + "}\"\r\n");
        CurrentTab += "\t";
    }

    public void BeginCSProject(string ProjectName, CSProjectFile ProjectFile)
    {
        Content += ("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = \"" + ProjectName + "\", \"" + Directory.GetRelativePath(ProjectFile.GetFullName()) + "\", \"{" + ProjectFile.MD5Hash + "}\"\r\n");
        CurrentTab += "\t";
    }

    public void BeginCSSDKProject(string ProjectName, CSProjectFile ProjectFile)
    {
        Content += ("Project(\"{9A19103F-16F7-4668-BE54-9A1E7A4F7556}\") = \"" + ProjectName + "\", \"" + Directory.GetRelativePath(ProjectFile.GetFullName()) + "\", \"{" + ProjectFile.MD5Hash + "}\"\r\n");
        CurrentTab += "\t";
    }

    public void BeginFolderProject(string ProjectName, string MD5Code)
    {
        Content += ("Project(\"{2150E333-8FDC-42A3-9474-1A3956D46DE8}\") = \"" + ProjectName + "\", \"" + ProjectName + "\", \"{" + MD5Code + "}\"\r\n");
        CurrentTab += "\t";
    }

    public void EndProject()
    {
        MoveTab();
        Content += ("EndProject\r\n");
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

    public void AddCustomGlobalSection(string Section)
    {
        Content += (CurrentTab + Section);
    }

    public void AddSolutionConfiguration(string Configuration, string Platform)
    {
        Content += (CurrentTab + Configuration + "|" + Platform + " = " + Configuration + "|" + Platform + "\r\n");
    }

    public void SetProjectConfiguration(VCProjectFile ProjectFile)
    {
        for (int ConfigurationIndex = 0; ConfigurationIndex < GlobalConfigurationPlatforms.Configurations.Length; ConfigurationIndex++)
        {
            for (int PlatformIndex = 0; PlatformIndex < GlobalConfigurationPlatforms.Platforms.Length; PlatformIndex++)
            {
                string[] Configuration = GlobalConfigurationPlatforms.Configurations[ConfigurationIndex];
                string[] Platform = GlobalConfigurationPlatforms.Platforms[PlatformIndex];
                Content += (CurrentTab + "{" + ProjectFile.MD5Hash + "}." + Configuration[0] + "|" + Platform[0] + ".ActiveCfg = " + Configuration[1] + "|" + Platform[1] + "\r\n");
                Content += (CurrentTab + "{" + ProjectFile.MD5Hash + "}." + Configuration[0] + "|" + Platform[0] + ".Build.0 = " + Configuration[1] + "|" + Platform[1] + "\r\n");
            }
        }
    }

    public void SetProjectConfiguration(CSProjectFile ProjectFile)
    {
        for (int ConfigurationIndex = 0; ConfigurationIndex < GlobalConfigurationPlatforms.Configurations.Length; ConfigurationIndex++)
        {
            for (int PlatformIndex = 0; PlatformIndex < GlobalConfigurationPlatforms.Platforms.Length; PlatformIndex++)
            {
                string[] Configuration = GlobalConfigurationPlatforms.Configurations[ConfigurationIndex];
                string[] Platform = GlobalConfigurationPlatforms.Platforms[PlatformIndex];
                string CSConfiguration = GlobalConfigurationPlatforms.CSConfigurations[ConfigurationIndex];
                string CSPlatform = GlobalConfigurationPlatforms.CSPlatforms[PlatformIndex];
                Content += (CurrentTab + "{" + ProjectFile.MD5Hash + "}." + Configuration[0] + "|" + Platform[0] + ".ActiveCfg = " + CSConfiguration + "|" + CSPlatform + "\r\n");
                Content += (CurrentTab + "{" + ProjectFile.MD5Hash + "}." + Configuration[0] + "|" + Platform[0] + ".Build.0 = " + CSConfiguration + "|" + CSPlatform + "\r\n");
            }
        }
    }

    public void SetNestedProject(string Project, string NestedProject)
    {
        Content += (CurrentTab + "{" + Project + "} = {" + NestedProject + "}\r\n");
    }

    public void FinishFile()
    {
        WriteFileContent(Content, FileMode.Create);
    }
}