@echo off
setlocal EnableDelayedExpansion
set SCRIPT_HOME=%~dp0.
call "%SCRIPT_HOME%\run" generate-toc ..\..
if not %ERRORLEVEL% EQU 0 (
    exit /b %ERRORLEVEL%)
