#tool nuget:?package=xunit.runner.console
#addin nuget:?package=Cake.Figlet&version=1.3.1

var target                  = Argument("target", "Default");
var configuration           = "Release";

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
// We define where the build artifacts should be places
// this is relative to the project root folder
var buildArtifacts          = Directory("./artifacts");
var deployment              = Directory("./artifacts/deployment");
var version                 = "1.1.0";

///////////////////////////////////////////////////////////////////////////////
// MODULES
///////////////////////////////////////////////////////////////////////////////
var modules                 = Directory("./src");
// To skip building a project in the source folder add the project folder name
// as string to the list e.g. "Yaapii.SimEngine.Tmx.Setup".
var blacklistedModules      = new List<string>() { };

// Unit tests
var unitTests               = Directory("./tests");
// To skip executing a test in the tests folder add the test project folder name
// as string to the list e.g. "TmxTest.Yaapii.Olp.Tmx.AllInOneRobot".
var blacklistedUnitTests    = new List<string>() { }; 

///////////////////////////////////////////////////////////////////////////////
// CONFIGURATION VARIABLES
///////////////////////////////////////////////////////////////////////////////
var isAppVeyor              = AppVeyor.IsRunningOnAppVeyor;
var isWindows               = IsRunningOnWindows();

// For GitHub release
var owner                   = "icarus-consulting";
var repository              = "Yaapii.JSON";

// For NuGetFeed
var nuGetSource             = "https://api.nuget.org/v3/index.json";
var appVeyorNuGetFeed       = "https://ci.appveyor.com/nuget/icarus/api/v2/package";

// API key tokens for deployment
var gitHubToken             = "";
var nugetReleaseToken       = "";
var appVeyorFeedToken       = "";

///////////////////////////////////////////////////////////////////////////////
// Version
///////////////////////////////////////////////////////////////////////////////
Task("Version")
.WithCriteria(() => isAppVeyor && BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
.Does(() => 
{
    Information(Figlet("Version"));
    
    version = BuildSystem.AppVeyor.Environment.Repository.Tag.Name;
    Information($"Set version to '{version}'");
});

///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
.Does(() =>
{
    Information(Figlet("Clean"));
    
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
    foreach(var module in GetSubDirectories(modules))
    {
        var name = module.GetDirectoryName();
        if(!blacklistedModules.Contains(name))
        {
            CleanDirectories(
                new DirectoryPath[] 
                { 
                    $"{module}/bin",
                    $"{module}/obj",
                }
            );
        }
    }
});

///////////////////////////////////////////////////////////////////////////////
// Restore
///////////////////////////////////////////////////////////////////////////////
Task("Restore")
.Does(() =>
{
    Information(Figlet("Restore"));
    
    NuGetRestore($"./{repository}.sln");
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Build")
.IsDependentOn("Version")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.Does(() =>
{
    Information(Figlet("Build"));

    var settings = 
        new DotNetCoreBuildSettings()
        {
            Configuration = configuration,
            NoRestore = true,
            MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersionPrefix(version)
        };
        var skipped = new List<string>();
    foreach(var module in GetSubDirectories(modules))
    {
        var name = module.GetDirectoryName();
        if(!blacklistedModules.Contains(name))
        {
            Information($"Building {name}");
            
            DotNetCoreBuild(
                module.FullPath,
                settings
            );
        }
        else
        {
            skipped.Add(name);
        }
    }
    if (skipped.Count > 0)
    {
        Warning("The following builds have been skipped:");
        foreach(var name in skipped)
        {
            Warning($"  {name}");
        }
    }
});

///////////////////////////////////////////////////////////////////////////////
// Unit Tests
///////////////////////////////////////////////////////////////////////////////
Task("UnitTests")
.IsDependentOn("Build")
.Does(() => 
{
    Information(Figlet("Unit Tests"));

    var settings = 
        new DotNetCoreTestSettings()
        {
            Configuration = configuration,
            NoRestore = true
        };
    var skipped = new List<string>();   
    foreach(var test in GetSubDirectories(unitTests))
    {
        var name = test.GetDirectoryName();
        if(blacklistedUnitTests.Contains(name))
        {
            skipped.Add(name);
        }
        else if(!name.StartsWith("TmxTest"))
        {
            Information($"Testing {name}");
            DotNetCoreTest(
                test.FullPath,
                settings
            );
        }
    }
    if (skipped.Count > 0)
    {
        Warning("The following tests have been skipped:");
        foreach(var name in skipped)
        {
            Warning($"  {name}");
        }
    }
});

///////////////////////////////////////////////////////////////////////////////
// NuGet
///////////////////////////////////////////////////////////////////////////////
Task("NuGet")
.IsDependentOn("Version")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.Does(() =>
{
    Information(Figlet("NuGet"));
    
    var settings = new DotNetCorePackSettings()
    {
        Configuration = configuration,
        OutputDirectory = buildArtifacts,
        NoRestore = true,
        NoBuild = true,
        VersionSuffix = ""
    };
    settings.ArgumentCustomization = args => args.Append("--include-symbols").Append("-p:SymbolPackageFormat=snupkg");
    settings.MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersionPrefix(version);

    foreach (var module in GetSubDirectories(modules))
    {
        var name = module.GetDirectoryName();
        if(!blacklistedModules.Contains(name))
        {
            DotNetCorePack(
                module.ToString(),
                settings
            );
        }
        else
        {
            Warning($"Skipping NuGet package for {name}");
        }
    }
});

///////////////////////////////////////////////////////////////////////////////
// Credentials
///////////////////////////////////////////////////////////////////////////////
Task("Credentials")
.WithCriteria(() => isAppVeyor)
.Does(() =>
{
    Information(Figlet("Credentials"));
    
    nugetReleaseToken = EnvironmentVariable("NUGET_TOKEN");
    if (string.IsNullOrEmpty(nugetReleaseToken))
    {
        throw new Exception("Environment variable 'NUGET_TOKEN' is not set");
    }
    appVeyorFeedToken = EnvironmentVariable("APPVEYOR_TOKEN");
    if (string.IsNullOrEmpty(appVeyorFeedToken))
    {
        throw new Exception("Environment variable 'APPVEYOR_TOKEN' is not set");
    }
});

///////////////////////////////////////////////////////////////////////////////
// NuGet Feed
///////////////////////////////////////////////////////////////////////////////
Task("NuGetFeed")
.WithCriteria(() => isAppVeyor && BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
.IsDependentOn("NuGet")
.IsDependentOn("Credentials")
.Does(() => 
{
    Information(Figlet("NuGet Feed"));
    
    var nugets = GetFiles($"{buildArtifacts.Path}/*.nupkg");
    foreach(var package in nugets)
    {
        // pushes symbols package too (see https://docs.microsoft.com/en-us/nuget/create-packages/symbol-packages-snupkg)
        NuGetPush(
            package,
            new NuGetPushSettings {
                Source = nuGetSource,
                ApiKey = nugetReleaseToken
            }
        );
    }
});

///////////////////////////////////////////////////////////////////////////////
// Default
///////////////////////////////////////////////////////////////////////////////
Task("Default")
.IsDependentOn("Credentials")
.IsDependentOn("Version")
.IsDependentOn("Clean")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.IsDependentOn("UnitTests")
.IsDependentOn("NuGet")
.IsDependentOn("NuGetFeed");

RunTarget(target);
