#!/bin/bash

declare -a RUNTIMES=("win-x64" "linux-x64" "osx-x64" "linux-arm" "osx-arm64")
CONFIGURATION="Release"

for RID in "${RUNTIMES[@]}"
do
    echo "Publishing for $RID..."
    dotnet publish  GCodeUtil/GCodeUtil.csproj -c $CONFIGURATION \
      -r $RID \
      --self-contained true \
      /p:PublishSingleFile=true \
      /p:IncludeNativeLibrariesForSelfExtract=true \
      /p:EnableCompressionInSingleFile=true \
      /p:DebugType=None \
      /p:PublishTrimmed=true  \
      -o ./publish/$RID
      zip -r ./publish/$RID ./publish/$RID/.
      rm -rf ./publish/$RID
done
