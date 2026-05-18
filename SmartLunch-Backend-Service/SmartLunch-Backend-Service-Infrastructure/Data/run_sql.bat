@echo off
setlocal EnableExtensions EnableDelayedExpansion
cd /d "%~dp0"

set "CONTAINER=smart-lunch-mysql"
set "MYSQL_USER=root"
set "MYSQL_PWD=DevMySQLRoot@SmartLunch2026"
set "SCRIPTS_DIR=Scripts"

REM Mode: update (default) | reset | full
set "MODE=update"
if /I "%~1"=="reset" set "MODE=reset"
if /I "%~1"=="full" set "MODE=reset"
if /I "%~1"=="update" set "MODE=update"

echo ======================================================
if /I "%MODE%"=="reset" (
    echo  SmartLunch Database - FULL RESET
    echo  WARNING: Drops SmartLunch database and re-seeds!
) else (
    echo  SmartLunch Database - INCREMENTAL UPDATE
    echo  Keeps existing data, applies maintenance scripts.
)
echo ======================================================
echo.

echo [1/4] Checking Docker container "%CONTAINER%"...
docker inspect -f "{{.State.Running}}" "%CONTAINER%" 2>nul | findstr /I "true" >nul
if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Container "%CONTAINER%" is not running.
    echo Start it first, e.g.: docker start %CONTAINER%
    echo.
    pause
    exit /b 1
)

echo [2/4] Syncing SQL scripts into container...
docker exec "%CONTAINER%" rm -rf /scripts 2>nul
docker exec "%CONTAINER%" mkdir -p /scripts
if %errorlevel% neq 0 (
    echo [ERROR] Could not prepare /scripts in container.
    pause
    exit /b 1
)

docker cp "%SCRIPTS_DIR%\." "%CONTAINER%:/scripts"
if %errorlevel% neq 0 (
    echo [ERROR] Failed to copy "%SCRIPTS_DIR%" to container.
    pause
    exit /b 1
)

if /I not "%MODE%"=="reset" (
    echo [3/4] Verifying database SmartLunch exists...
    docker exec "%CONTAINER%" mysql -u "%MYSQL_USER%" -p"%MYSQL_PWD%" -N -e "USE SmartLunch;" 2>nul
    if %errorlevel% neq 0 (
        echo.
        echo [WARN] Database SmartLunch does not exist yet.
        echo Running FULL RESET instead ^(run_all.sql^)...
        echo.
        set "MODE=reset"
    )
)

if /I "!MODE!"=="reset" (
    set "SQL_FILE=run_all.sql"
    echo [4/4] Running FULL setup: !SQL_FILE! ...
) else (
    set "SQL_FILE=run_update.sql"
    echo [4/4] Running UPDATE: !SQL_FILE! ...
)

echo.

docker exec "%CONTAINER%" bash -c "cd /scripts && mysql --default-character-set=utf8mb4 -u \"%MYSQL_USER%\" -p\"%MYSQL_PWD%\" < \"!SQL_FILE!\""
set "EXITCODE=%errorlevel%"

if %EXITCODE% neq 0 (
    echo.
    echo [ERROR] SQL execution failed ^(exit %EXITCODE%^).
    echo Tips:
    echo   - reset:  run_sql.bat reset
    echo   - update: run_sql.bat update
    echo.
    pause
    exit /b %EXITCODE%
)

echo.
echo ======================================================
if /I "!MODE!"=="reset" (
    echo  DONE - Database reset and fully seeded.
) else (
    echo  DONE - Schema/maintenance update applied.
)
echo ======================================================
echo.
echo Usage:
echo   run_sql.bat          - incremental update ^(default^)
echo   run_sql.bat update   - same as default
echo   run_sql.bat reset    - drop DB and run run_all.sql
echo.

pause
exit /b 0
