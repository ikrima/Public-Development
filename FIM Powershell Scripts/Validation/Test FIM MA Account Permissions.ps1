#--------------------------------------------------------------------------------------------------------------------------
 set-variable -name RegKey -value "hklm:\SYSTEM\CurrentControlSet\Services\FIMService" -option constant 
#--------------------------------------------------------------------------------------------------------------------------
 write-host "`nFIM MA Account Quick Test"
 write-host "==========================="
#--------------------------------------------------------------------------------------------------------------------------
 if((test-path $RegKey) -eq $false)
 {throw (new-object ExecutionEngineException "FIM registry key not found")}
 $accountSid = (Get-ItemProperty "$RegKey").SynchronizationAccountSid
 $sid = new-object System.Security.Principal.SecurityIdentifier $accountSid
 $ntAccountFromSid = $sid.Translate([System.Security.Principal.NTAccount])
 $ntAccountFromReg = (Get-ItemProperty "$RegKey").SynchronizationAccount
 if(0 -ne [String]::Compare($ntAccountFromSid,$ntAccountFromReg, $true))
 {throw "Rgistry FIM MA account name and SID don't match!"} 
 write-host " -FIM MA account name: $ntAccountFromSid"
 write-host " -FIM MA account SID : $accountSid"
 write-host "Command completed successfully`n"
#--------------------------------------------------------------------------------------------------------------------------
 trap
 { 
    Write-Host "`nError: $($_.Exception.Message)`n" -foregroundcolor white -backgroundcolor darkred
    Exit
 }
#--------------------------------------------------------------------------------------------------------------------------
