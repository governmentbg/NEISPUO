########### improve error handling ###########
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$PSDefaultParameterValues['*:ErrorAction']='Stop'
##############################################

$publishLocation = Join-Path (Get-Location) "..\build\" -Resolve

.\deploy.ps1 `
    -PublishLocation $publishLocation `
    -IISApp "test-blobs/"
