@echo off
setlocal EnableDelayedExpansion
set SCRIPT_HOME=%~dp0.
dotnet run --project "%SCRIPT_HOME%\Gapotchenko.FX.Utilities.MDDocProcessor" -c Release --no-launch-profile -v q -- %*
