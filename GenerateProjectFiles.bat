@echo off

rem ## This is the entry to generate the whole project.
echo Generating Project Files...
if not exist "%~dp0Build\BatchFiles\GenerateProjectFiles.bat" goto ERROR_BatchFileLocation
call "%~dp0Build\BatchFiles\GenerateProjectFiles.bat"
pause
exit /B %ERRORLEVEL%


:ERROR_BatchFileLocation
echo PersonaEngine ERROR: the batch files cannot be found.
pause