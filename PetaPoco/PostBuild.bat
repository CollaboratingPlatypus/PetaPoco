cd ..\..
pwd


if %1 == Release NuGet.exe pack "PetaPoco.nuspec" -o "..\Output"
if %1 == Release NuGet.exe pack "PetaPoco.Core.nuspec" -o "..\Output"

..\csj\csj.exe -o:PetaPoco.cs Database.cs -r *.cs -x:Properties\*.cs