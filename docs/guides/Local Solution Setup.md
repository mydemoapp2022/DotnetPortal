# Local Solution Setup Guide
 
## Prerequisites
 
Before working with this solution, ensure the following are already installed and configured:
 
- Visual Studio Professional 2026
- .NET 10 SDK
 
## Initial Setup Steps
 
1. Clone or pull the solution repository to your local machine.
 
2. From the solution root directory, run the following command **before attempting to build**:
 
   > dotnet tool restore
 
## Important Notes
 
- This solution relies on .NET tools that are integrated into the build process.
- If the tools are not restored first, the build will fail.
- Errors caused by missing tools can prevent the project from building entirely.
 
Always run the tool restore step after a fresh clone or when tools are updated.