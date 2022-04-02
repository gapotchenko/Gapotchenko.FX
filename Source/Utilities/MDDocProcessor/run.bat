@echo off
call dotnet run --project Gapotchenko.FX.Utilities.MDDocProcessor -c Release --no-launch-profile %*
if not %ERRORLEVEL% EQU 0 (
    exit /b %ERRORLEVEL%)
