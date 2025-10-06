@echo off
echo === Building Config Management Applications ===
echo.

echo Building complete solution...
dotnet build ConfigManagement.sln

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Build failed! Please check the error messages above.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo Build completed successfully!
echo.
echo To run the applications:
echo   Console app: cd ConsoleApp ^&^& dotnet run
echo   Windows Forms app: cd WindowsFormsApp ^&^& dotnet run
echo.
pause