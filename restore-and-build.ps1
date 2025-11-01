Write-Host "Cleaning bin and obj directories..." -ForegroundColor Yellow
Get-ChildItem -Path . -Include bin,obj -Recurse -Directory | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore

Write-Host "Compiling solution..." -ForegroundColor Yellow
dotnet build --no-restore

Write-Host "Process completed!" -ForegroundColor Green