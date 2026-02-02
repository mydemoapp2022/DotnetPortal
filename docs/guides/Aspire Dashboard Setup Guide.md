# Aspire Dashboard – Local Development Guide (Windows / PowerShell)

This guide walks through getting the **Aspire Dashboard** running locally so we can view **structured logs, traces, and metrics** during development.

Our system is **already exporting OTLP data to localhost:4317**, so once the dashboard is running it should just work.


## Option 1: The Fast Path (Containers)

If you have access to **Docker or Podman**, you can short-circuit almost everything below.  
The Aspire Dashboard is standalone and easy to run as a container.

Run the following command (single line):

    docker run --rm -it -d -p 18888:18888 -p 4317:18889 --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:latest

Once started, open your browser to:

    http://localhost:18888

If this works for you, you can stop here.


## Option 2: No Containers (Build from Source)

Most of us don’t have container access, so we’ll build the dashboard locally.


## 1. Clone the Aspire Repository

Clone the Aspire repo:

    https://github.com/dotnet/aspire


## 2. Verify the .NET SDK Version

Aspire currently targets **.NET SDK 10**.

You may need to update:

    aspire/global.json

Make sure it references a .NET 10 SDK that you actually have installed.  
The default roll-forward behavior may not match your local setup.


## 3. Build the Solution

Build the solution normally.

Notes:
- There are **hundreds of projects**
- The build will take a while

You only need to do this once.


## 4. Publish the Aspire Dashboard Locally

### Create a publish directory

We want the output in the **root of your user profile**.

In PowerShell:

    mkdir $HOME\aspire-build


### Publish the dashboard

From the Aspire repo, navigate to:

    src\Aspire.Dashboard

Run:

    dotnet publish Aspire.Dashboard.csproj -c Release -r win-x64 -p:PublishSingleFile=true -p:SelfContained=true -o $HOME\aspire-build

After this completes, you should have:

    Aspire.Dashboard.exe

located at:

    $HOME\aspire-build


## 5. Create a PowerShell Launcher Script

In the same directory as **Aspire.Dashboard.exe**, create a file named:

    aspire.ps1

Contents:

    # PowerShell script to run Aspire Dashboard standalone

    # Set environment variables for this session only
    $env:ASPNETCORE_URLS = "http://localhost:18888"
    $env:ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL = "http://localhost:4317"
    $env:ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL = "http://localhost:4318"
    $env:ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS = "true"

    # Run the dashboard executable
    $dashboard = Start-Process -NoNewWindow -FilePath ".\Aspire.Dashboard.exe" -PassThru
    Start-Sleep -Seconds 2
    Start-Process $env:ASPNETCORE_URLS
    $dashboard.WaitForExit()


## 6. Create a CMD Wrapper for Easy Launching

Create a file named:

    aspire-dashboard.cmd

In the same directory, with the following contents:

    @echo off
    setlocal

    powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%USERPROFILE%\aspire-build\aspire.ps1"

Note:  
This file intentionally uses **cmd.exe syntax**, since it is a CMD launcher.


## 7. Create a Shortcut (Optional)

You can now:
1. Right-click **aspire-dashboard.cmd**
2. Select **Create shortcut**
3. Move the shortcut to your desktop or another convenient location

Double-clicking the shortcut will:
- Start the Aspire Dashboard
- Automatically open the browser

## 8. Verify Everything Is Working

- Dashboard UI:  
      http://localhost:18888
- OTLP endpoint already configured:  
      localhost:4317

If your local services are exporting telemetry, logs, traces, and metrics should appear shortly after startup.

 