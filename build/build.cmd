setlocal enableextensions enabledelayedexpansion

pushd ..\src\lschanged

dotnet publish -c Release ^
               -o ..\..\releases\win-x64 ^
               --self-contained ^
               --runtime win-x64 ^
               -p:PublishTrimmed=true ^
               -p:PublishSingleFile=true

dotnet publish -c Release ^
               -o ..\..\releases\linux-x64 ^
               --self-contained ^
               --runtime linux-x64 ^
               -p:PublishTrimmed=true ^
               -p:PublishSingleFile=true

popd             

endlocal & exit /b 0