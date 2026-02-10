#!/bin/bash

rm -r ../build
dotnet build ../src/SB.Blobs.sln /p:TreatWarningsAsErrors=true /warnaserror --configuration Release
dotnet publish ../src/SB.Blobs.csproj --no-build --output ../build --configuration Release
