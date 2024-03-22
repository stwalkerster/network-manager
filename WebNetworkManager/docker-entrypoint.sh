#!/usr/bin/env sh
set -e 

if [ -d /run/secrets ]; then
  for f in /run/secrets/*; do
    . "$f"
  done
fi

dotnet WebNetworkManager.dll
