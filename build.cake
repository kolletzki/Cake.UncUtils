#tool nuget:?package=NUnit.ConsoleRunner

//////////////////////////////////////////////////
// Arguments                                    //
//////////////////////////////////////////////////
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

//////////////////////////////////////////////////
// Config                                       //
//////////////////////////////////////////////////
var slnFile = "./Cake.UncUtils.sln";
var nuspecFile = "./Cake.UncUtils.nuspec";
var testDll = "./Cake.UncUtils.Test/bin/" + configuration + "/Cake.UncUtils.Test.dll";

//////////////////////////////////////////////////
// Tasks                                        //
//////////////////////////////////////////////////

//Clean whole solution
Task("Clean-Sln")
	.Does(() => 
{
	CleanDirectories("./**/bin");
	CleanDirectories("./**/obj");
});

//Restore NuGet packages for solution
Task("Restore-NuGet")
	.Does(() => 
{
	NuGetRestore(slnFile);
});

//Build solution
Task("Build-Sln")
	.IsDependentOn("Clean-Sln")
	.IsDependentOn("Restore-NuGet")
	.Does(() => 
{
	DotNetBuild(slnFile, settings => settings.Configuration = configuration);
});

//Run unit tests
Task("Run-Tests")
	.IsDependentOn("Build-Sln")
	.Does(() => 
{
	NUnit3(testDll);
});

//Build NuGet package
Task("Build-NuGet")
	.IsDependentOn("Run-Tests")
	.Does(() => 
{
	CreateDirectory("./nupkg");

	NuGetPack(nuspecFile, new NuGetPackSettings
	{
		Verbosity = NuGetVerbosity.Detailed,
		OutputDirectory = "./nupkg"
	});
});

//Publish the last built NuGet package-
Task("Publish-NuGet")
	.IsDependentOn("Build-NuGet")
	.Does(() => 
{
	var package = GetFiles("./nupkg/*.nupkg")
		.OrderBy(f => new System.IO.FileInfo(f.FullPath).LastWriteTimeUtc)
		.LastOrDefault();

	NuGetPush(package, new NuGetPushSettings
	{
		Source = "https://www.nuget.org/api/v2/package",
		Verbosity = NuGetVerbosity.Detailed
	});
});

RunTarget(target);