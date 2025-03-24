set windows-shell := ["cmd", "/c"]
set working-directory := "Source"

# Show the help for this justfile
@help:
  just --list

build:
  dotnet build -c Release

clean:
  dotnet clean -c Debug
  dotnet clean -c Release

# Run all tests  
test:
  dotnet test -c Debug
  dotnet test -c Release

# Pack NuGet packages
pack:
  dotnet clean -c Release
  dotnet pack -c Release
