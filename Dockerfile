#
# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

# Copy the entire solution into the container
COPY . .

# Restore dependencies from public NuGet feed
RUN dotnet restore --verbosity normal

# Publish all projects under src to the /app folder.
#
# NO `-r <rid>` ON PURPOSE. `-r linux-x64` used to be pinned here, and it made
# the apphost (/app/Rayls.Custody.HSM.API) an x86-64 ELF no matter which
# platform the image was built for. On the arm64 Graviton nodes that fails at
# startup with:
#     exec /app/Rayls.Custody.HSM.API: exec format error
# Without `-r`, the publish is RID-agnostic and the apphost is emitted for the
# SDK's own architecture — arm64 when built on the ARM64 runner
# (_build-image.yml defaults to ubuntu-24.04-arm / platforms: linux/arm64),
# x64 when built on an x64 machine. It also makes the `dotnet restore` above
# actually count, instead of being redone RID-specific by publish.
RUN find src -name '*.csproj' -exec dotnet publish {} --configuration Release -f net8.0 --output /app \;

# Stage 2: Create final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Create a non-root user and group for running the app.
# UID/GID are pinned to 1001 to match rayls-app-chart's securityContext
# (runAsUser: 1001, fsGroup: 1001), so the chart's UID and the owner of /app are
# the same user. Without --uid, `adduser --system` picks an arbitrary system UID
# (usually 999-ish) and the two disagree.
RUN addgroup --system --gid 1001 appgroup \
    && adduser --system --uid 1001 --ingroup appgroup appuser

# Set working directory for the runtime image
WORKDIR /app

# Copy published output from the build stage
COPY --from=build /app /app

# Set ownership to non-root user and make executables runnable
RUN chown -R appuser:appgroup /app \
    && chmod +x /app/Rayls.Custody.HSM.API

# Switch to the non-root user — NUMERIC on purpose.
# The image config records this literally, so `USER appuser` makes it the string
# "appuser". With `runAsNonRoot: true` and no explicit runAsUser, the kubelet
# cannot prove a named user isn't root and refuses to start the container:
#   "image has non-numeric user (appuser), cannot verify user is non-root"
# `USER 1001` is verifiable and starts either way.
USER 1001

# Disable .NET Tracing diagnostics in production
ENV COMPlus_EnableDiagnostics=0

# mcr.microsoft.com/dotnet/aspnet:8.0 defaults to ASPNETCORE_HTTP_PORTS=8080 —
# override it so the app actually binds the port declared below.
ENV ASPNETCORE_HTTP_PORTS=5000

# Start the application directly
ENTRYPOINT ["/app/Rayls.Custody.HSM.API"]

# Expose the application port
EXPOSE 5000