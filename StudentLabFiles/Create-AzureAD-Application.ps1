$authResult = Connect-AzureAD

$tenantId = $authResult.TenantId.ToString()
$tenantDomain = $authResult.TenantDomain

$userAccountId = $authResult.Account.Id
$user = Get-AzureADUser -ObjectId $userAccountId

$appDisplayName = "App-Owns-Data Sample App"
$replyUrl = "https://localhost:5001/signin-oidc"

# create app secret
$newGuid = New-Guid
$appSecret = ([System.Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes(($newGuid))))+"="
$startDate = Get-Date	
$passwordCredential = New-Object -TypeName Microsoft.Open.AzureAD.Model.PasswordCredential
$passwordCredential.StartDate = $startDate
$passwordCredential.EndDate = $startDate.AddYears(1)
$passwordCredential.KeyId = $newGuid
$passwordCredential.Value = $appSecret 

# create Azure AD Application
$aadApplication = New-AzureADApplication `
                        -DisplayName $appDisplayName `
                        -PublicClient $false `
                        -AvailableToOtherTenants $false `
                        -ReplyUrls @($replyUrl) `
                        -Homepage $replyUrl `
                        -PasswordCredentials $passwordCredential

# create applicaiton's service principal 
$appId = $aadApplication.AppId
$appObjectId = $aadApplication.ObjectId
$serviceServicePrincipal = New-AzureADServicePrincipal -AppId $appId
$serviceServicePrincipalObjectId = $serviceServicePrincipal.ObjectId

Write-Host "appObjectId: $appObjectId"

# assign current user as owner
Add-AzureADApplicationOwner -ObjectId $aadApplication.ObjectId -RefObjectId $user.ObjectId

# add the service princiapl to Azure AD security group named 'Power BI Apps'
$adSecurityGroupName = "Power BI Apps"
$adSecurityGroup = Get-AzureADGroup -Filter "DisplayName eq '$adSecurityGroupName'"
if($adSecurityGroup) {
    # Add service principal of the new app as member of Power BI Apps group
    Add-AzureADGroupMember -ObjectId $($adSecurityGroup.ObjectId) `
                           -RefObjectId $($serviceServicePrincipalObjectId)

    # Display members of the Power BI Apps group
    Write-Host "Members of Azure AD group named $adSecurityGroupName"
    $members = Get-AzureADGroupMember -ObjectId $($adSecurityGroup.ObjectId) 
    $members | Format-Table ObjectType, ObjectId, DisplayName
}

$outputFile = "$PSScriptRoot\AppOwnsDataSampleApp.txt"
Write-Host "Writing info to $outputFile"
Out-File -FilePath $outputFile -Append -InputObject "{"
Out-File -FilePath $outputFile -Append -InputObject "  ""AzureAd"": {"
Out-File -FilePath $outputFile -Append -InputObject "    ""Instance"": ""https://login.microsoftonline.com/"","
Out-File -FilePath $outputFile -Append -InputObject "    ""Domain"": ""$tenantDomain"","
Out-File -FilePath $outputFile -Append -InputObject "    ""TenantId"": ""$tenantId"","
Out-File -FilePath $outputFile -Append -InputObject "    ""ClientId"": ""$appId"","
Out-File -FilePath $outputFile -Append -InputObject "    ""ClientSecret"": ""$appSecret"","
Out-File -FilePath $outputFile -Append -InputObject "    ""CallbackPath"": ""/signin-oidc"","
Out-File -FilePath $outputFile -Append -InputObject "    ""SignedOutCallbackPath"": ""/signout-callback-oidc"""
Out-File -FilePath $outputFile -Append -InputObject "  },"
Out-File -FilePath $outputFile -Append -InputObject "  ""PowerBi"": {"
Out-File -FilePath $outputFile -Append -InputObject "    ""ServiceRootUrl"": ""https://api.powerbi.com/"""
Out-File -FilePath $outputFile -Append -InputObject "  },"
Out-File -FilePath $outputFile -Append -InputObject "  ""Logging"": {"
Out-File -FilePath $outputFile -Append -InputObject "    ""LogLevel"": {"
Out-File -FilePath $outputFile -Append -InputObject "      ""Default"": ""Information"","
Out-File -FilePath $outputFile -Append -InputObject "      ""Microsoft"": ""Warning"","
Out-File -FilePath $outputFile -Append -InputObject "      ""Microsoft.Hosting.Lifetime"": ""Information"""
Out-File -FilePath $outputFile -Append -InputObject "    }"
Out-File -FilePath $outputFile -Append -InputObject "  },"
Out-File -FilePath $outputFile -Append -InputObject "  ""AllowedHosts"": ""*"""
Out-File -FilePath $outputFile -Append -InputObject "}"

Notepad $outputFile