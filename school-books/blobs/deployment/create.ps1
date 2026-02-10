param(
    [Parameter(Mandatory=$true)]
    [string]$SiteName,

    [Parameter(Mandatory=$true)]
    [Tuple[string,string][]]$Bindings,

    [Parameter(Mandatory=$true)]
    [string]$PeriodicRestartAt
)

########### improve error handling ###########
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"
$PSDefaultParameterValues['*:ErrorAction']='Stop'
##############################################

$appcmd = "$env:SystemRoot\system32\inetsrv\appcmd"
$sitepath = "$env:SystemDrive\inetpub\$SiteName\"

if (Test-Path $sitepath -PathType Container) {
    Write-Error "Site directory exists."
    exit 1
}

New-Item -ItemType Directory -Force -Path $sitepath

& $appcmd add site /name:$SiteName /bindings:"$($Bindings[0].item1)/$($Bindings[0].item2)" /physicalPath:$sitepath                                                    ; if (!$?) { throw 'native call failed' }
# there is a different way of adding the first binding and the rest, hence the for (1...) loop
For ($i=1; $i -lt $Bindings.Length; $i++) {
  & $appcmd set site /site.name:$SiteName /+"bindings.[protocol='$($Bindings[$i].item1)',bindingInformation='$($Bindings[$i].item2)']"                                    ; if (!$?) { throw 'native call failed' }
}

& $appcmd add apppool /name:$SiteName /managedRuntimeVersion:"v4.0" /managedPipelineMode:"Integrated"                                                                   ; if (!$?) { throw 'native call failed' }
& $appcmd set apppool $SiteName /startMode:"AlwaysRunning" /processModel.idleTimeout:"0.00:00:00" /recycling.periodicRestart.time:"00:00:00"                            ; if (!$?) { throw 'native call failed' }
& $appcmd set config -section:system.applicationHost/applicationPools /+"[name='$SiteName'].recycling.periodicRestart.schedule.[value='$PeriodicRestartAt']" /commit:apphost      ; if (!$?) { throw 'native call failed' }

& $appcmd set app "$SiteName/" /applicationPool:$SiteName /preloadEnabled:true                                                                                          ; if (!$?) { throw 'native call failed' }

# log x-authorization (bearer token) header in IIS logs
& $appcmd set config  -section:system.applicationHost/sites /+"[name='$SiteName'].logFile.customFields.[logFieldName='x-authorization',sourceName='Authorization',sourceType='RequestHeader']" /commit:apphost      ; if (!$?) { throw 'native call failed' }

# log x-forwarded-for header in IIS logs
& $appcmd set config  -section:system.applicationHost/sites /+"[name='$SiteName'].logFile.customFields.[logFieldName='x-forwarded-for',sourceName='X-Forwarded-For',sourceType='RequestHeader']" /commit:apphost    ; if (!$?) { throw 'native call failed' }
