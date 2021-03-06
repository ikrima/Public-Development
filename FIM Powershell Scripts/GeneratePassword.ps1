function pwdgen ($pwd=$null) {
$pwdlength = [int]8
$bytes = [byte[]][byte]1
$pwd = [string]""
$rng = New-Object System.Security.Cryptography.RNGCryptoServiceProvider
$numeric = 3
$alpha = 5
$special = 2

do {
    $rng.getbytes($bytes)
    $rnd = $bytes[0] -as [int]
    $int = ($rnd % 90) + 33
    if (($int -ge 48 -and $int -le 57) -and ($numeric -gt 0))
    {
        $numeric = $numeric - 1
        $chr = $int -as [char]
        $pwd = $pwd + $chr
    }
    
    if ((($int -ge 65 -and $int -le 90) -or ($int -ge 97 -and $int -le 122)) -and ($alpha -gt 0))
    {
        $alpha = $alpha - 1
        $chr = $int -as [char]
        $pwd = $pwd + $chr
    }
    
    if (($int -ge 33 -and $int -le 38) -and ($special -gt 0))
    {
        $special = $special - 1
        $chr = $int -as [char]
        $pwd = $pwd + $chr
    }
} 
while ($alpha -gt 0 -or $numeric -gt 0 -or $special -gt 0)

write-host $pwd
}
pwdgen