@echo off
setlocal EnableExtensions EnableDelayedExpansion
cd /d "%~dp0"

set "CONTAINER=smart-lunch-mysql"
set "MYSQL_USER=root"
set "MYSQL_PWD=DevMySQLRoot@SmartLunch2026"
set "SCRIPTS_DIR=Scripts"

REM Mode: update (default) | reset
set "MODE=update"
if /I "%~1"=="reset" set "MODE=reset"
if /I "%~1"=="full" set "MODE=reset"
if /I "%~1"=="update" set "MODE=update"

echo ======================================================
if /I "%MODE%"=="reset" (
    echo  SmartLunch Database - FULL RESET
    echo  run_all.sql ^(init + seed + run_update.sql^)
    echo  WARNING: Drops SmartLunch database and re-seeds!
) else (
    echo  SmartLunch Database - INCREMENTAL UPDATE
    echo  run_update.sql only ^(keeps data^)
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

set "SQL_FILE=run_update.sql"
if /I "%MODE%"=="reset" (
    set "SQL_FILE=run_all.sql"
) else (
    echo [3/4] Verifying database SmartLunch exists...
    docker exec "%CONTAINER%" mysql -u "%MYSQL_USER%" -p"%MYSQL_PWD%" -N -e "USE SmartLunch;" 2>nul
    if %errorlevel% neq 0 (
        echo.
        echo [WARN] Database SmartLunch does not exist yet.
        echo Running FULL RESET instead ^(run_all.sql includes run_update.sql^)...
        echo.
        set "SQL_FILE=run_all.sql"
        set "MODE=reset"
    )
)

echo [4/4] Running !SQL_FILE! ...
echo.

docker exec "%CONTAINER%" bash -c "cd /scripts && mysql --default-character-set=utf8mb4 -u \"%MYSQL_USER%\" -p\"%MYSQL_PWD%\" < \"!SQL_FILE!\""
set "EXITCODE=%errorlevel%"

if %EXITCODE% neq 0 (
    echo.
    echo [ERROR] SQL execution failed ^(exit %EXITCODE%^).
    echo Tips:
    echo   run_sql.bat update  - maintenance only ^(run_update.sql^)
    echo   run_sql.bat reset   - drop DB + full setup ^(run_all.sql^)
    echo.
    pause
    exit /b %EXITCODE%
)

echo.
echo ======================================================
if /I "!SQL_FILE!"=="run_all.sql" (
    echo  DONE - Full setup completed ^(includes run_update.sql^).
) else (
    echo  DONE - Incremental update applied ^(run_update.sql^).
)
echo ======================================================
echo.
echo Usage:
echo   run_sql.bat          - run_update.sql ^(default^)
echo   run_sql.bat update   - same as default
echo   run_sql.bat reset    - run_all.sql ^(init + seed + run_update^)
echo.

pause
exit /b 0
