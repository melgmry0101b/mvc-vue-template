@echo off

REM This file is a consolidation point for build, run, and deploy scripts.

WHERE /q pwsh
IF ERRORLEVEL 1 (
    ECHO PowerShell Core 'pwsh' cannot be found
    EXIT /B
)

pwsh .\scripts\build\main.ps1 %*
