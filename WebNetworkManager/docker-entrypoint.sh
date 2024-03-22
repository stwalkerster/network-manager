#!/usr/bin/env sh
set -x
set -e 

if [ -d /run/secrets ]; then
  for f in /run/secrets/*; do
    . "$f"
  done
fi

dotnet WebNetworkManager.dll
