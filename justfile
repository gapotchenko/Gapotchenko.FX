# Gapotchenko.FX
#
# Copyright © Gapotchenko and Contributors
#
# File introduced by: Oleksiy Gapotchenko
# Year of introduction: 2025

set working-directory := "Source"

# ---------------------------------------------------------------------------
# Shells & Interpreters
# ---------------------------------------------------------------------------

set windows-shell := ["gnu-tk", "-i", "-c"]
set script-interpreter := ["gnu-tk", "-i", "-l", "/bin/sh", "-eu"]

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

set dotenv-load := true

# ---------------------------------------------------------------------------
# Recipes
# ---------------------------------------------------------------------------

# Show the help for this justfile
@help:
    just --list

# Start IDE using the project environment
[group("development")]
[windows]
develop:
    start "" *.slnx

# Start IDE using the project environment
[group("development")]
[unix]
develop:
    open *.slnx

# Install development prerequisites
[group("development")]
prerequisites:
    go install mvdan.cc/sh/v3/cmd/shfmt@latest
    gnu-tk -i -x Build/Prerequisites.sh

# Format source code
[group("development")]
[script]
format:
    echo 'Formatting **/*.sh...'
    fd -e sh -x shfmt -i 4 -l -w
    echo 'Formatting **/justfile...'
    fd --glob justfile -x just --unstable --fmt --justfile
    (cd Mastering; cat Exclusion.dic | tr '[:upper:]' '[:lower:]' | sort -u | sponge Exclusion.dic)

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

# Deploy published artifacts to a point of delivery
[group("publication")]
deploy:
    dotnet nuget push '**\bin\Release\nuget\*.nupkg' --source "$GP_OSS_NUGET_SOURCE"

# Make a release by testing, publishing and deploying the project artifacts
[group("publication")]
release: test publish deploy
