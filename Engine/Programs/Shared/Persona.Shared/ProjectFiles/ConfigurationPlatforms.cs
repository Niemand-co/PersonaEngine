using System;

public static class GlobalConfigurationPlatforms
{
    public enum EConfiguration
    {
        EC_DevelopmentEditor = 0,
        EC_DevelopmentGame = 1,
        EC_ReleaseEditor = 2,
        EC_ReleaseGame = 3,
        EC_Num = 4,
    };

    public enum EPlatform
    {
        EP_Win64 = 0,
        EP_Win32 = 1,
        EP_Num = 2,
    };

    public static string[][] Configurations = new string[][]
    {
        new string[3] { "Development Editor", "Development_Editor", "Debug" },
        new string[3] { "Development Game", "Development_Game", "Debug" },
        new string[3] { "Release Editor", "Release_Editor", "Release" },
        new string[3] { "Release Game", "Release_Game", "Release" },
    };

    public static string[][] Platforms = new string[][]
    {
        new string[2] { "Win64", "x64" },
    };

    public static string[] CSConfigurations = new string[]
    {
        "Debug",
        "Release",
        "Debug",
        "Release",
    };

    public static string[] CSPlatforms = new string[]
    {
        "Any CPU",
    };

    public static string[][] BuildCommandLine = new string[][]
    {
        new string[]
        {
            "..\\..\\..\\Build\\BatchFiles\\Build.bat -Target=\"PersonaEditor Win64 Development\"",
        },
        new string[]
        {
            "..\\..\\..\\Build\\BatchFiles\\Build.bat -Target=\"PersonaGame Win64 Development\"",
        },
        new string[]
        {
            "..\\..\\..\\Build\\BatchFiles\\Build.bat -Target=\"PersonaEditor Win64 Release\"",
        },
        new string[]
        {
            "..\\..\\..\\Build\\BatchFiles\\Build.bat -Target=\"PersonaGame Win64 Release\"",
        },
    };

    public static string[][] RebuildCommandLine = new string[][]
    {
        new string[]
        {
            "..\\..\\..\\Build\\BatchFiles\\Rebuild.bat -Target=\"PersonaEditor Win64 Development\"",
        },
        new string[]
        {
            "..\\..\\..\\Build\\BatchFiles\\Rebuild.bat -Target=\"PersonaGame Win64 Development\"",
        },
        new string[]
        {
            "..\\..\\..\\Build\\BatchFiles\\Rebuild.bat -Target=\"PersonaEditor Win64 Release\"",
        },
        new string[]
        {
            "..\\..\\..\\Build\\BatchFiles\\Rebuild.bat -Target=\"PersonaGame Win64 Release\"",
        },
    };

    public static string[][] CleanCommandLine = new string[][]
    {
        new string[]
        {
            "..\\..\\..\\Build\\BatchFiles\\Clean.bat -Target=\"PersonaEditor Win64 Development\"",
        },
        new string[]
        {
            "..\\..\\..\\Build\\BatchFiles\\Clean.bat -Target=\"PersonaGame Win64 Development\"",
        },
        new string[]
        {
            "..\\..\\..\\Build\\BatchFiles\\Clean.bat -Target=\"PersonaEditor Win64 Release\"",
        },
        new string[]
        {
            "..\\..\\..\\Build\\BatchFiles\\Clean.bat -Target=\"PersonaGame Win64 Release\"",
        },
    };

    public static string[][] OutputTarget = new string[][]
    {
        new string[]
        {
            "..\\..\\..\\Engine\\Binaries\\Win64\\PersonaEditor.exe",
        },
        new string[]
        {
            "..\\..\\..\\Engine\\Binaries\\Win64\\PersonaGame.exe",
        },
        new string[]
        {
            "..\\..\\..\\Engine\\Binaries\\Win64\\PersonaEditor.exe",
        },
        new string[]
        {
            "..\\..\\..\\Engine\\Binaries\\Win64\\PersonaGame.exe",
        },
    };
}