#!/bin/bash
set -e

# All packages resolve from the public NuGet feed. If you keep a custom
# nuget.config alongside the sources, it will be used for restore.
RESTORE_ARGS=""
if [ -f "nuget.config" ]; then
    echo "Found nuget.config, using it for restore..."
    RESTORE_ARGS="--configfile nuget.config"
fi

echo "Restoring NuGet packages..."
dotnet restore src/Rayls.Custody.HSM.API/Rayls.Custody.HSM.API.csproj $RESTORE_ARGS

echo "Starting API with hot-reload..."
exec dotnet watch \
    --project src/Rayls.Custody.HSM.API/Rayls.Custody.HSM.API.csproj \
    run \
    --no-launch-profile \
    --urls "http://+:5000"
