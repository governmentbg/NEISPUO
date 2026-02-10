param(
    [Parameter(Mandatory=$true)]
    [string]$PublishLocation,

    [Parameter(Mandatory=$true)]
    [string]$IISApp,

    [Parameter(Mandatory=$false)]
    [string]$ComputerName,

    [Parameter(Mandatory=$false)]
    [string]$UserName,

    [Parameter(Mandatory=$false)]
    [string]$Password,

    [Parameter(Mandatory=$false)]
    [string]$AuthType
)

########### improve error handling ###########
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$PSDefaultParameterValues['*:ErrorAction']='Stop'
##############################################

function Get-MSWebDeployInstallPath(){
    return (Get-ChildItem "HKLM:\SOFTWARE\Microsoft\IIS Extensions\MSDeploy" | Select -last 1).GetValue("InstallPath")
}

$msdeploypath = Get-MSWebDeployInstallPath
$msdeploy = Join-Path $msdeploypath "msdeploy.exe"

$remote = ""
if ($ComputerName -and $UserName -and $Password -and $AuthType)
{
    $remote = ",computerName=""$ComputerName"",userName=""$UserName"",password=""$Password"",authtype=""$AuthType"""
}

$msdeployArgs = @(
    "-verb:sync",
    "-source:iisApp=""$PublishLocation""",
    "-dest:iisApp=""$IISApp""$remote",
    "-disableLink:AppPoolExtension",
    "-disableLink:ContentExtension",
    "-disableLink:CertificateExtension",
    "-allowUntrusted",
    "-enableRule:AppOffline",
    "-retryAttempts:20"
)

& $msdeploy $msdeployArgs
if (!$?) { throw 'native call failed' }
