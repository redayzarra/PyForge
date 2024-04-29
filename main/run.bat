@echo off
:loop
dotnet run
if %ERRORLEVEL% == 2 goto loop
if %ERRORLEVEL% == 3 powershell -Command "test-compiler"
