# Gapotchenko.FX
#
# Copyright © Gapotchenko and Contributors
#
# File introduced by: Oleksiy Gapotchenko
# Year of introduction: 2025

set dotenv-load
set working-directory := "Source"

[windows]
set shell := ["gnu-tk", "-i", "-c"]
[windows]
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
[working-directory("..")]
format:
    echo 'Formatting **/*.sh...'
    fd -e sh -x shfmt -l -w
    echo 'Formatting **/justfile...'
    fd --glob justfile -x just --fmt --justfile
    (cd Source/Mastering; just format)

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
    dotnet test -c "{{ configuration }}"

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
