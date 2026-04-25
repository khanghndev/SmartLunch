@echo off
echo === Copy SQL scripts into Docker container ===

docker cp ./Scripts/. smart-lunch-mysql:/scripts

echo === Running SQL script inside container ===

docker exec -i smart-lunch-mysql bash -c "cd /scripts && mysql -u root -pDevMySQLRoot@SmartLunch2026 < run_all.sql"

echo === DONE ===
pause