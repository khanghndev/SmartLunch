@echo off
setlocal
cd /d "%~dp0"

echo ======================================================
echo  SmartLunch Database - Full Reset ^& Update
echo ======================================================
echo.

echo [1/3] Clearing old scripts in container...
docker exec smart-lunch-mysql rm -rf /scripts
docker exec smart-lunch-mysql mkdir -p /scripts
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Could not connect to Docker container "smart-lunch-mysql".
    echo Make sure Docker Desktop is running and the container is started.
    echo.
    pause
    exit /b 1
)

echo [2/3] Copying fresh SQL scripts to container...
docker cp Scripts/. smart-lunch-mysql:/scripts
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Failed to copy scripts to container.
    echo Make sure the "Scripts" folder exists next to this .bat file.
    echo.
    pause
    exit /b 1
)

echo [3/3] Running Master Script (run_all.sql)...
echo This may take a few seconds...
docker exec -i smart-lunch-mysql bash -c "cd /scripts && mysql -u root -pDevMySQLRoot@SmartLunch2026 < run_all.sql"
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] SQL execution failed! Check the output above for details.
) else (
    echo.
    echo ======================================================
    echo  DONE! Database has been fully reset and updated.
    echo ======================================================
)

echo.
pause