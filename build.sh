#!/bin/bash

echo "=== Building Config Management Applications ==="

# Check if we're on Windows (where Windows Forms can be built)
if [[ "$OSTYPE" == "msys" || "$OSTYPE" == "win32" || "$OSTYPE" == "cygwin" ]]; then
    echo "Building both Console and Windows Forms applications..."
    dotnet build ConfigManagement.sln
else
    echo "Building Console application (Windows Forms requires Windows)..."
    dotnet build ConsoleApp/ConfigManagementConsole.csproj
    
    echo ""
    echo "Note: Windows Forms application (WindowsFormsApp/) can only be built and run on Windows."
    echo "To build the complete solution on Windows, run: dotnet build ConfigManagement.sln"
fi

echo ""
echo "Build completed!"