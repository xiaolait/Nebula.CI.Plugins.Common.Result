#! /bin/bash

rm -r ./bin

dotnet publish -o ./bin/Release
docker build -t nebula/ci/plugins/common/result .