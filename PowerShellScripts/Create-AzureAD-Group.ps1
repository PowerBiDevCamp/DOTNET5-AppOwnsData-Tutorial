$authResult = Connect-AzureAD

$adSecurityGroupName = "Power BI Apps"

$adSecurityGroup = 
`New-AzureADGroup `
    -DisplayName $adSecurityGroupName `
    -SecurityEnabled $true `
    -MailEnabled $false `
    -MailNickName notSet

$adSecurityGroup | Format-Table DisplayName, ObjectId