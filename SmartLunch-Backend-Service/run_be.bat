@echo off
title SmartLunch Backend Runner

echo === STARTING SERVICES ===

call SmartLunch-Backend-Service-Infrastructure\Data\run_sql.bat

if %ERRORLEVEL% neq 0 (
    echo SQL script failed!
    pause
    exit /b %ERRORLEVEL%
)

echo === START BACKEND (.NET) ===

cd /d SmartLunch-Backend-Service-API
dotnet run

pause