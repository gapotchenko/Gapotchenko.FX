@echo off
call run generate-toc ..\..
if not %ERRORLEVEL% EQU 0 (
    exit /b %ERRORLEVEL%)
