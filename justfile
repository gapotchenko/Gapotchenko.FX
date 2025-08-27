set working-directory := "Source"
set dotenv-load := true
set windows-shell := ["gnu-tk", "-i", "-c"]
set script-interpreter := ["gnu-tk", "-i", "-l", "/bin/sh", "-eu"]
set unstable

# Show the help for this justfile
@help:
    just --list

# Start IDE using the project environment
[group("development")]
[windows]
develop:
    start "" *.sln?

# Start IDE using the project environment
[group("development")]
[unix]
develop:
    open *.sln?

# Format source code
[group("development")]
[script]
format:
    echo 'Formatting **/*.sh...'
    fd -e sh -x shfmt -i 4 -l -w
    echo 'Formatting **/justfile...'
    fd --glob justfile -x just --unstable --fmt --justfile

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
test configuration="Release":
    dotnet test -c "{{configuration}}"

# Produce publishable artifacts
publish:
    dotnet clean -c Release
    dotnet pack -c Release
