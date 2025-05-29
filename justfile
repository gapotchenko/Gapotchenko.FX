set working-directory := "Source"
set dotenv-load := true
set windows-shell := ["cmd", "/c"]

# Show the help for this justfile
@help:
    just --list

# Start IDE using the project environment
[windows]
develop:
    start Gapotchenko.FX.sln

# Build release artifacts
build:
    dotnet build -c Release

# Clean all build artifacts
clean:
    dotnet clean -c Debug
    dotnet clean -c Release

# Run all tests
test:
    dotnet test -c Debug
    dotnet test -c Release

# Produce publishable artifacts
publish:
    dotnet clean -c Release
    dotnet pack -c Release

# Make a release by testing and producing publishable project artifacts
release: test publish
