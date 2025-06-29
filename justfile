set working-directory := "Source"
set dotenv-load := true
set windows-shell := ["cmd", "/c"]

# Show the help for this justfile
@help:
    just --list

# Start IDE using the project environment
[windows]
develop:
    #!cmd /c
    @for /F "delims=" %%i in ('"dir /b | findstr ".*\.sln""') do @(start "" "%%i")

# Start IDE using the project environment
[linux]
[macos]
develop:
    open *.sln?

# Build release artifacts
build:
    dotnet build -c Release

# Rebuild release artifacts
rebuild:
    dotnet build --no-incremental -c Release

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
