#addin "nuget:?package=Cake.Git&version=0.16.1"
#addin "nuget:?package=Cake.Powershell&version=0.4.2"
#addin "nuget:?package=semver&version=2.0.4"

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");
var buildNugetPackages = Argument<bool>("buildNugetPackages", false);
var publishNugetPackages = Argument<bool>("publishNugetPackages", false);
var nugetApiKey = Argument<string>("nugetApiKey", null);

Setup(ctx =>
{
    Information("======================");
    Information("Build variables values");
    Information("======================");

    Information($"{"Task target", -32}: {target}");
    Information($"{"Build configuration", -32}: {configuration}");
    Information($"{"Build nuget packages", -32}: {buildNugetPackages}");
    Information($"{"Publish nuget packages", -32}: {publishNugetPackages}");
});

Task("Clean")
    .Does(() => {
        CleanDirectories("./**/obj/**");
        CleanDirectories("./**/bin/**");
    });

Task("Default")
    .Does(() =>
    {
        Information("No target (-target {target}) supplied. The following targets are valid");
        Information("\tClean");
        Information("\tBuild");
    });

RunTarget(target);