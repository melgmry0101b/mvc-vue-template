#!/bin/bash

# This file is a consolidation point for build, run, and deploy scripts.

if ! command -v pwsh &> /dev/null
then
    echo "PowerShell Core 'pwsh' cannot be found"
    exit
fi

pwsh ./scripts/build/main.ps1 "$@"
