@echo off
setlocal EnableDelayedExpansion
set SCRIPT_HOME=%~dp0.
"%SCRIPT_HOME%\run" generate-toc ..\..
