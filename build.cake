#tool nuget:?package=GitReleaseManager
#tool nuget:?package=xunit.runner.console

var target = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

// we define where the build artifacts should be places
// this is relative to the project root folder
var buildArtifacts      = new DirectoryPath("./artifacts/");
var framework     = "netstandard2.0";
var testFramework = "netcoreapp2.1";
var project = new DirectoryPath("./src/Yaapii.JSON/Yaapii.JSON.csproj");

var owner = "icarus-consulting";
var repository = "Yaapii.JSON";

var githubtoken = "";
var codecovToken = "";

var isAppVeyor          = AppVeyor.IsRunningOnAppVeyor;

var version = "11.0.0";


///////////////////////////////////////////////////////////////////////////////
// CLEAN
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
  .Does(() => 
{
  // clean the artifacts folder to prevent old builds be present
  // https://cakebuild.net/dsl/directory-operations/
  CleanDirectories(new DirectoryPath[] { buildArtifacts });
});

///////////////////////////////////////////////////////////////////////////////
// RESTORE
///////////////////////////////////////////////////////////////////////////////
Task("Restore")
  .Does(() =>
{
  // collect all csproj files recusive from the root directory
  // and run a niget restore
	var projects = GetFiles("./**/*.csproj");

	foreach(var project in projects)
	{
	    DotNetCoreRestore(project.FullPath);
  }
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Build")
  .IsDependentOn("Clean") // we can define Task`s which a dependet on other task like this
  .IsDependentOn("Restore")
  .Does(() =>
{	
	//main = netstandard2.0, tests = netcoreapp2.0
	var projects = GetFiles("./src/**/*.csproj");	//main project(s)
	var testProjects = GetFiles("./tests/**/*.csproj"); //test project(s)

	foreach(var project in projects)
	{
		DotNetCoreBuild(project.ToString(), new DotNetCoreBuildSettings() {
		  Framework = framework,
		  Configuration = configuration
		});
	}

	foreach(var project in testProjects)
	{
		DotNetCoreBuild(project.ToString(), new DotNetCoreBuildSettings() {
		  Framework = testFramework,
		  Configuration = configuration
		});
	}
});

///////////////////////////////////////////////////////////////////////////////
// Test
///////////////////////////////////////////////////////////////////////////////
Task("Test")
  .IsDependentOn("Build")
  .Does(() =>
{
    var projectFiles = GetFiles("./tests/**/*.csproj");
	foreach(var file in projectFiles)
    {
		Information("### Discovering Tests in " + file.FullPath);
        DotNetCoreTest(file.FullPath);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Packaging
///////////////////////////////////////////////////////////////////////////////
Task("Pack")
  .IsDependentOn("Version")
  .IsDependentOn("Build")
  .Does(() => 
{
  
	var settings = new DotNetCorePackSettings()
    {
        Configuration = configuration,
        OutputDirectory = buildArtifacts,
	  	VersionSuffix = ""
    };
   
	settings.MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersionPrefix(version);
	settings.ArgumentCustomization = args => args.Append("--include-symbols");

   if (isAppVeyor)
   {

       var tag = BuildSystem.AppVeyor.Environment.Repository.Tag;
       if(!tag.IsTag) 
       {
			settings.VersionSuffix = "build" + AppVeyor.Environment.Build.Number.ToString().PadLeft(5,'0');
       } 
	   else 
	   {     
			settings.MSBuildSettings = new DotNetCoreMSBuildSettings().SetVersionPrefix(tag.Name);
       }
	}
	
	DotNetCorePack(
		project.ToString(),
		settings
    );
});

///////////////////////////////////////////////////////////////////////////////
// Version
///////////////////////////////////////////////////////////////////////////////
Task("Version")
  .WithCriteria(() => isAppVeyor && BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
  .Does(() => 
{
    version = BuildSystem.AppVeyor.Environment.Repository.Tag.Name;
});

///////////////////////////////////////////////////////////////////////////////
// Release
///////////////////////////////////////////////////////////////////////////////
Task("GetAuth")
    .Does(() =>
{
    githubtoken = EnvironmentVariable("GITHUB_TOKEN");
	codecovToken = EnvironmentVariable("CODECOV_TOKEN");
});

Task("Release")
  .WithCriteria(() => isAppVeyor && BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag)
  .IsDependentOn("Version")
  .IsDependentOn("Pack")
  .IsDependentOn("GetAuth")
  .Does(() => {
     GitReleaseManagerCreate(githubtoken, owner, repository, new GitReleaseManagerCreateSettings {
            Milestone         = version,
            Name              = version,
            Prerelease        = false,
            TargetCommitish   = "master"
    });
          
	var nugetFiles = string.Join(",", GetFiles("./artifacts/**/*.nupkg").Select(f => f.FullPath) );
	Information("Nuget artifacts: " + nugetFiles);

	GitReleaseManagerAddAssets(
		githubtoken,
		owner,
		repository,
		version,
		nugetFiles
	);

	GitReleaseManagerPublish(githubToken, owner, repository, version);

});

Task("Default")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("Pack")
  .IsDependentOn("Release")
  .Does(() =>
{ });

RunTarget(target);
