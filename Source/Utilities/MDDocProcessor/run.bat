@echo off
setlocal EnableDelayedExpansion
set SCRIPT_HOME=%~dp0.
call dotnet run --project "%SCRIPT_HOME%\Gapotchenko.FX.Utilities.MDDocProcessor" -c Release --no-launch-profile -- %*
if not %ERRORLEVEL% EQU 0 (
    exit /b %ERRORLEVEL%)
