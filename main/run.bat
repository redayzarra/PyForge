@echo off
:loop
dotnet run
if %ERRORLEVEL% == 2 goto loop