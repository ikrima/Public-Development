pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        powershell(script: 'Set-Item WSMan:\\localhost\\Client\\TrustedHosts * -Force; Write-Host $env:KL_JNKNS_ADMINUSER Write-Host $env:KL_JNKNS_ADMINPWD $Username = "ikrima-ryzen\\kl_admin" $Password = "kl_admin" | ConvertTo-SecureString -AsPlainText -Force $UserCredential = New-Object System.Management.Automation.PSCredential -ArgumentList $Username,$Password $Session = New-PSSession -ComputerName ikrima-ryzen -Credential $UserCredential Invoke-Command -Session $Session -ScriptBlock {   	Set-ExecutionPolicy Bypass -Scope Process -Force; 	$chocoInstDir = if ($env:ChocolateyInstall) {$env:ChocolateyInstall} else { \'C:\\ProgramData\\chocolatey\' }; 	Import-Module "$chocoInstDir\\helpers\\chocolateyInstaller.psm1"; 	Update-SessionEnvironment;      pushd "C:\\knl_v\\VBBDepot\\Devops\\BuildAutomation\\BuildAutomation\\Devops" ;     & .\\bldOps.py fullbuild --dosync ;     popd; } Remove-PSSession $Session', encoding: 'utf-8')
      }
    }
    stage('Archive') {
      agent any
      steps {
        bat 'echo "Archiving"'
      }
    }
  }
}