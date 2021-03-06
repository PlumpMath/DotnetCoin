#/usr/bin/env bash

#exit if any command fails
set -e

dotnet clean
dotnet restore

dotnet build

dotnet test DotcoinTests/DotcoinTests.csproj
