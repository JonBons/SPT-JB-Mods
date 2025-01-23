@echo off
REM PostBuild Script to copy build output to an absolute path

REM Define the target directory (replace with your desired path)
set TARGET_DIR=S:\Games\Fika\BepInEx\plugins

REM Copy all output files from the build directory to the target directory
xcopy "%~dp0bin\%1\netstandard2.0\JBTrackIR.dll" "%TARGET_DIR%" /E /H /C /I /Y
xcopy "%~dp0bin\%1\netstandard2.0\TrackIRUnity.dll" "%TARGET_DIR%" /E /H /C /I /Y

REM Exit with success code
exit /b 0
