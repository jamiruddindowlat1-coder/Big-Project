function Start-CourseSystem {
    $rootPath = "D:\Big Project\CourseManagementSystem"
    $apiPath = "$rootPath\CourseManagementSystem.Api"
    $frontendPath = "$rootPath\my-frontend"

    Write-Host "Starting..." -ForegroundColor Cyan

    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$apiPath'; dotnet run"

    Start-Sleep -Seconds 3

    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$frontendPath'; npm start"

    Start-Sleep -Seconds 2
    Start-Process "http://localhost:5000/swagger"

    Write-Host "Done! Swagger: http://localhost:5000/swagger" -ForegroundColor Green
}

Start-CourseSystem