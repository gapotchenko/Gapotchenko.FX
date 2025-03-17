set windows-shell := ["cmd", "/c"]
set working-directory := "Source"

@help:
  just --list

build:
  dotnet build -c Release

clean:
  dotnet clean -c Debug
  dotnet clean -c Release
  
test:
  dotnet test -c Debug
  dotnet test -c Release

pack:
  dotnet clean -c Release
  dotnet pack -c Release
