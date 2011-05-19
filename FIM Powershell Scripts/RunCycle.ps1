#Run cycle for ILM v2 workshop

$logFile = [System.IO.Path]::ChangeExtension($myinvocation.mycommand.definition, '.log')

# First, some functions to help us with the WMI stuff

function GetMA
{
    param([string]$maName)
    $filter = "Name='" + $maName + "'"
    Get-WmiObject -Class MIIS_ManagementAgent -Namespace root/MicrosoftIdentityIntegrationServer -Filter $filter
}

function CheckStatus
{
    param([System.Management.ManagementObject]$managementAgent)

    $result = $managementAgent.RunStatus().ReturnValue.ToString()
    if ( $result -eq 'success' )
    {
        Write-Host $result -ForegroundColor Green
    }
    else
    {
        Write-Host $result -ForegroundColor Cyan
    }
}

function LogManagementAgentRun
{
    param([System.Management.ManagementObject]$managementAgent)

    [xml]$runDetails = $managementAgent.RunDetails().ReturnValue

    ('     MA: {0}' -f $runDetails.'run-history'.'run-details'.'ma-name') >> $logFile
    ('Profile: {0}' -f $runDetails.'run-history'.'run-details'.'run-profile-name') >> $logFile
    (' Run by: {0}' -f $runDetails.'run-history'.'run-details'.'security-id') >> $logFile

    $runDetails.'run-history'.'run-details'.'step-details' | 
    Sort-Object {$_.'step-number' } | Foreach-Object {
	('   Step: {0} Type: {1}' -f $_.'step-number',$_.'step-description'.'step-type'.type) >> $logFile
	('  Start: {0}' -f $_.'start-date') >> $logFile
	('    End: {0}' -f $_.'end-date') >> $logFile
	(' Result: {0}' -f $_.'step-result') >> $logFile
    }
    '' >> $logFile

}

function PendingImports
{
    param([System.Management.ManagementObject]$managementAgent)

    [int]$add = $managementAgent.NumImportAdd().ReturnValue
    [int]$mod = $managementAgent.NumImportUpdate().ReturnValue
    [int]$del = $managementAgent.NumImportDelete().ReturnValue
    
    if ( ($add + $mod + $del) -gt 0 )
    {
        $true
    }
    else
    {
        $false
    }
}

function PendingExports
{
    param([System.Management.ManagementObject]$managementAgent)

    [int]$add = $managementAgent.NumExportAdd().ReturnValue
    [int]$mod = $managementAgent.NumExportUpdate().ReturnValue
    [int]$del = $managementAgent.NumExportDelete().ReturnValue
    
    if ( ($add + $mod + $del) -gt 0 )
    {
        $true
    }
    else
    {
        $false
    }
}

# Now make sure that we are running this on a machine with ILM installed
#
$ilmRegKey = Get-ItemProperty HKLM:\SYSTEM\CurrentControlSet\Services\miiserver\Parameters -ErrorAction silentlycontinue
if ( -not $? )
{
    Write-Host "ILM does not seem to be installed on this system."
    Exit
}

$ilmPath = $ilmRegKey.Path
if ([System.String]::IsNullOrEmpty($ilmPath))
{
    Write-Host "ILM does not seem to be installed correctly on this system."
    Exit
}

# switch to the directory where the script is located
pushd (Split-Path -parent $myinvocation.mycommand.definition)

# and go to it.
#===============================================================
$displayWidth = 40

$displayString = 'Clearing run history'
$displayString = $displayString.PadRight($displayWidth)
Write-Host $displayString -NoNewLine -ForegroundColor Yellow

$svr = Get-WmiObject -Class miis_server -Namespace root/MicrosoftIdentityIntegrationServer
$result = $svr.ClearRuns($('{0:yyyy-MM-dd HH:mm:ss.fff}' -f (Get-Date).ToUniversalTime()))
if ( $result.ReturnValue -eq 'success' )
{
    Write-Host $result.ReturnValue -ForegroundColor Green
}
else
{
    Write-Host $result.ReturnValue -ForegroundColor Cyan
}

# Get the MA's
$ad = GetMA("AD")
$ilm = GetMA("ILM2")

# Do a full import ...
$profileName = 'Full Import'
$displayString = $( $ilm.Name + " [$profileName]" )
$displayString = $displayString.PadRight($displayWidth)
Write-Host $displayString -NoNewLine -ForegroundColor Yellow
$ilm.Execute($profileName) > $null
CheckStatus($ilm)
LogManagementAgentRun($ilm)

# Run a full sync
$profileName = 'Full Sync'
$displayString = $( $ilm.Name + " [$profileName]" )
$displayString = $displayString.PadRight($displayWidth)
Write-Host $displayString -NoNewLine -ForegroundColor Yellow
$ilm.Execute($profileName) > $null
CheckStatus($ilm)
LogManagementAgentRun($ilm)

$profileName = 'Export and import'
$displayString = $( $ad.Name + " [$profileName]" )
$displayString = $displayString.PadRight($displayWidth)
Write-Host $displayString -NoNewLine -ForegroundColor Yellow
$ad.Execute($profileName) > $null
CheckStatus($ad)
LogManagementAgentRun($ad)

$profileName = 'Export'
if (PendingExports($ilm))
{
    $displayString = $( $ilm.Name + " [$profileName]" )
    $displayString = $displayString.PadRight($displayWidth)
    Write-Host $displayString -NoNewLine -ForegroundColor Yellow
    $ilm.Execute($profileName) > $null
    CheckStatus($ilm)
    LogManagementAgentRun($ilm)
}


# go back whence we came
popd
