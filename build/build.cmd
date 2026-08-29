setlocal enableextensions enabledelayedexpansion

pushd ..\src\lschanged

dotnet publish -c Release ^
               -o ..\..\releases\win-x64 ^
               --self-contained ^
               --runtime win-x64 ^
               -p:PublishTrimmed=true ^
               -p:PublishSingleFile=true ^
               -p:InvariantGlobalization=true

dotnet publish -c Release ^
               -o ..\..\releases\win-arm64 ^
               --self-contained ^
               --runtime win-arm64 ^
               -p:PublishTrimmed=true ^
               -p:PublishSingleFile=true ^
               -p:InvariantGlobalization=true

dotnet publish -c Release ^
               -o ..\..\releases\linux-x64 ^
               --self-contained ^
               --runtime linux-x64 ^
               -p:PublishTrimmed=true ^
               -p:PublishSingleFile=true ^
               -p:InvariantGlobalization=true

dotnet publish -c Release ^
               -o ..\..\releases\linux-arm ^
               --self-contained ^
               --runtime linux-arm ^
               -p:PublishTrimmed=true ^
               -p:PublishSingleFile=true ^
               -p:InvariantGlobalization=true

dotnet publish -c Release ^
               -o ..\..\releases\linux-arm64 ^
               --self-contained ^
               --runtime linux-arm64 ^
               -p:PublishTrimmed=true ^
               -p:PublishSingleFile=true ^
               -p:InvariantGlobalization=true

popd             

endlocal & exit /b 0