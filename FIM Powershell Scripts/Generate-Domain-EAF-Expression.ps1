#--------------------------------------------------------------------------------------------------------
 Set-Variable -Name ForestDn -Value "DC=dhl,DC=Com"
 Set-Variable -Name DnsRoot  -Value "prg-dc.dhl.com"
#--------------------------------------------------------------------------------------------------------
 Clear-Host
 $objSearcher = New-Object System.DirectoryServices.DirectorySearcher
 $objSearcher.SearchRoot = "LDAP://CN=Partitions,CN=Configuration,$ForestDn" 
 $objSearcher.Filter     = "(&(objectclass=Crossref)(dnsRoot=*)(netBIOSName=*))"
 $dataList = @()
 
 $objSearcher.FindAll() | ForEach{
	$Domain = New-Object DirectoryServices.DirectoryEntry "LDAP://$($_.Properties.ncname)"
    If($Domain.objectGuid -eq $null) {Throw "Partition not found"}
	$DomainSid = New-Object System.Security.Principal.SecurityIdentifier($Domain.objectSid[0], 0)
	
	$newRecord = new-object psobject
    $newRecord | add-member noteproperty "Path"           $($_.Path)
    $newRecord | add-member noteproperty "NetBIOSName"    $($_.Properties.netbiosname)
    $newRecord | add-member noteproperty "SID"            $DomainSid.ToString()
	
	$dataList += $newRecord
 }

 If($dataList.length -eq 0) {Throw "L:No domain partitions found!"}

 $CustomExpression = ""
 $dataList | ForEach {
    $CustomExpression += 
	   "IIF(Eq(Left(ConvertSidToString(objectSid),$($_.SID.Length)),""$($_.SID)""),""$($_.NetBIOSName)"","
 }
 $CustomExpression += """Unknown"""
 $dataList | ForEach {
    $CustomExpression += ")"
 }

 Write-Host "Domain partitions for forest"
 Write-Host "============================"
 Write-Host "Forest  : $ForestDn"
 Write-Host "DNS Root: $DnsRoot" 
 $dataList | Format-List
 Write-Host "Custom Expression:"
 Write-Host $CustomExpression
 Write-Host ""
 $CustomExpression | clip
#--------------------------------------------------------------------------------------------------------
 Trap 
 { 
    $exMessage = $_.Exception.Message
    If($exMessage.StartsWith("L:"))
    {write-host "`n" $exMessage.substring(2) "`n" -foregroundcolor white -backgroundcolor darkblue}
    Else 
	{write-host "`nError: " $exMessage "`n" -foregroundcolor white -backgroundcolor darkred}
    Exit 1
 }
#--------------------------------------------------------------------------------------------------------