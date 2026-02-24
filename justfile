# Gapotchenko.FX
#
# Copyright © Gapotchenko and Contributors
#
# File introduced by: Oleksiy Gapotchenko
# Year of introduction: 2025

set dotenv-load := true
set working-directory := "Source"
set windows-shell := ["gnu-tk", "-i", "-c"]
set script-interpreter := ["gnu-tk", "-i", "-l", "/bin/sh", "-eu"]

# -----------------------------------------------------------------------------

# Show the help for this justfile
@help:
    just --list

# -----------------------------------------------------------------------------
# Development
# -----------------------------------------------------------------------------

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
[script]
prerequisites:
    go install mvdan.cc/sh/v3/cmd/shfmt@latest
    if [ -n "${GNU_TK_MSYS2_REPOSITORY_PREFIX-}" ]; then
        pacman -S --needed --noconfirm "${GNU_TK_MSYS2_REPOSITORY_PREFIX}fd"
        pacman -S --needed --noconfirm moreutils
    fi

# Format source code
[group("development")]
[script]
format:
    echo 'Formatting **/*.sh...'
    fd -e sh -x shfmt -l -w
    echo 'Formatting **/justfile...'
    fd --glob justfile -x just --unstable --fmt --justfile
    (cd Mastering; cat Exclusion.dic | tr '[:upper:]' '[:lower:]' | sort -u | sponge Exclusion.dic)

# -----------------------------------------------------------------------------
# Build
# -----------------------------------------------------------------------------

# Build release artifacts
[group("build")]
build:
    dotnet build -c Release

# Rebuild release artifacts
[group("build")]
rebuild:
    dotnet build --no-incremental -c Release

# Clean all build artifacts
[group("build")]
clean:
    dotnet clean -c Debug
    dotnet clean -c Release

# Run all tests
[group("build")]
test configuration="Release":
    dotnet test -c "{{configuration}}"

# Produce publishable artifacts
[group("build")]
publish:
    dotnet clean -c Release
    dotnet pack -c Release

# -----------------------------------------------------------------------------
# Publication
# -----------------------------------------------------------------------------

# Deploy published artifacts to a point of delivery
[group("publication")]
deploy:
    dotnet nuget push '**\bin\Release\nuget\*.nupkg' --source "$GP_OSS_NUGET_SOURCE"

# Make a release by testing, publishing and deploying the project artifacts
[group("publication")]
release: test publish deploy
