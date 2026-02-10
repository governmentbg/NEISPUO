########### improve error handling ###########
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$PSDefaultParameterValues['*:ErrorAction']='Stop'
##############################################

$bindings = @(
  [Tuple]::Create("https","*:443:blobs.mon.bg")
)

.\create.ps1 "prod-blobs" $bindings "02:00:00"
