## Developing with Power BI Embedding using .NET 5

In this lab, you will create a new .NET 5 project for an MVC web
application and then you will go through the steps required to implement
Power BI embedding. You will use the new Microsoft authentication
library named *Microsoft.Identity.Web* to provide an interactive login
experience for users and to acquire app-only access tokens which you
will need to call the Power BI Service API. After that, you will write
the server-side C\# code and the client-side JavaScript code to embed a
simple Power BI report on a custom Web page. In the later exercises of
this lab, you will add project-level support for Node.js, TypeScript and
webpack so that you can migrate the client-side code from JavaScript to
TypeScript so your code receives the benefits of strong typing,
IntelliSense and compile-time type checks.

These lab exercises require that you have a Microsoft 365 tenant in
which you have Power BI administrator permissions and the ability to
create new Azure AD groups and applications. If you need a new Azure AD
tenant for a development environment, you can follow the steps in
[Create Power BI Development
Environment.pdf](https://github.com/PowerBiDevCamp/Camp-Sessions/raw/master/Create%20Power%20BI%20Development%20Environment.pdf).

To complete this lab, your developer PC must be configured to allow the
execution of PowerShell scripts with Windows PowerShell 5. Your
developer workstation must also have the following software and
developer tools installed.

1\) **PowerShell cmdlet library for AzureAD** –
\[[download](https://docs.microsoft.com/en-us/powershell/azure/active-directory/install-adv2?view=azureadps-2.0)\]  
2) **.NET 5 SDK** –
\[[download](https://dotnet.microsoft.com/download/dotnet/5.0)\]  
3) **Visual Studio Code** –
\[[download](https://code.visualstudio.com/Download)\]  
4) **Node.js** – \[[download](https://nodejs.org/en/download/)\]  
5) **Visual Studio 2019** (optional) –
\[[download](https://visualstudio.microsoft.com/downloads/)\]

Please refer to this [setup
document](https://github.com/PowerBiDevCamp/Camp-Sessions/raw/master/Create%20Power%20BI%20Development%20Environment.pdf)
if you need more detail on how to configure your developer workstation
to work on this tutorial.

### Exercise 1: Create a New Azure AD Application

In this exercise, you will begin by copying the student files into a
local folder on your student workstation. After that, you will use the
.NET 5 CLI to create a new .NET 5 project for an MVC web application
with support for authentication.

1.  Download the student lab files to a local folder on your developer
    workstation.

2.  Create a new top-level folder on your workstation named **DevCamp**
    at a location such as **c:\\DevCamp**.

3.  Download the ZIP archive with the student lab files from GitHub by
    clicking the following link.

<https://github.com/PowerBiDevCamp/DOTNET5-AppOwnsData-Tutorial/archive/master.zip>

4.  From inside the ZIP file, extract the **Solutions** folder and
    **StudentLabFiles** folder into a local folder such as
    **c:\\DevCamp\\**.

<img src="images\media\image1.png" style="width:3.43531in;height:0.99656in" />

5.  The **DevCamp** folder should now contain a Solutions folder and
    **StudentLabFiles** folder as shown in the following screenshot.

<img src="images\media\image2.png" style="width:4.82127in;height:0.80355in" />

6.  The **Solutions** folder contains child folders with the completed
    .NET 5 project solution for each of the exercises.

<img src="images\media\image3.png" style="width:2.31348in;height:0.95285in" />

7.  The **StudentLabFiles** folder contains files that you will need as
    you move through the exercises in this lab.

<img src="images\media\image4.png" style="width:5.5579in;height:1.91776in" />

You will be required to write quite a bit code as you complete the
exercises in this lab. Many of the files in the **StudentLabFiles**
folder are provided to you as a convenience in case you'd like to
copy-and-paste the code required during the various exercise of this
lab.

8.  Execute the PowerShell script named **Create-AzureAD-Group.ps1** to
    create a new Azure AD group named **Power BI Apps**.

9.  Using Windows Explorer, look in the **StudentLabFiles** folder and
    locate the script named **Create-AzureAD-Group.ps1.**

10. Open **Create-AzureAD-Group.ps1** in the Windows PowerShell ISE.

11. The script begins by calling **Connect-AzureAD** to establish a
    connection with Azure AD.

$authResult = Connect-AzureAD

12. After that, the script creates a new Azure AD security group in your
    Azure AD tenant named **Power BI Apps**.

$adSecurityGroupName = "Power BI Apps"

$adSecurityGroup = New-AzureADGroup \`

-DisplayName $adSecurityGroupName \`

-SecurityEnabled $true \`

-MailEnabled $false \`

-MailNickName notSet

$adSecurityGroup \| Format-Table DisplayName, ObjectId

13. Execute **Create-AzureAD-Group.ps1**.using the Windows PowerShell
    ISE or the Windows PowerShell command line.

14. When prompted for credentials, log in with an Azure AD user account
    in the same tenant where you are using Power BI.

<img src="images\media\image5.png" style="width:5.10768in;height:3.05279in" />

15. Use the Azure portal to verify the new Azure AD group named Power BI
    Apps has been created.

    1.  Navigate to the **All groups** blade of the Azure AD portal
        using the following URL.

<https://portal.azure.com/#blade/Microsoft_AAD_IAM/GroupsManagementMenuBlade/AllGroups>

2.  Verify that you can see the new Azure AD security group named
    **Power BI Apps**.

<img src="images\media\image6.png" style="width:4.81579in;height:1.15445in" />

1.  Enable the **Allow service principals to use Power BI APIs** setting
    and configure it with the **Power BI Apps** security group.

    1.  Navigate to the Power BI portal at <https://app.powerbi.com>.

    2.  Drop down the **Settings** menu and select the navigation
        command for the **Admin portal**.

<img src="images\media\image7.png" style="width:6.04386in;height:1.11106in" />

3.  In the Power BI Admin portal, click the **Tenant settings** link on
    the left.

<img src="images\media\image8.png" style="width:2.54874in;height:1.63706in" />

4.  Move down in the **Developer settings** section and expand the
    **Allow service principals to use Power BI APIs** section.

<img src="images\media\image9.png" style="width:3.64697in;height:2.21601in" />

5.  Note that the **Allow service principals to use Power BI APIs**
    setting is initially set to **Disabled**.

<img src="images\media\image10.png" style="width:2.83882in;height:1.31524in" />

6.  Change the setting to **Enabled** and place your cursor inside the
    **Apply to: Specific security groups** textbox.

<img src="images\media\image11.png" style="width:3.38187in;height:1.34759in" />

7.  Type in **Power BI Apps** to resolve the Azure AD group and then
    click **Apply** to save your configuration changes.

<img src="images\media\image12.png" style="width:3.00984in;height:0.87233in" />

8.  You will see a notification indicating it may take up to 15 minutes
    until your tenant recognizes your configuration changes.

<img src="images\media\image13.png" style="width:3.91776in;height:0.70182in" />

The previous step is required because you must configure the Power BI
Service instance in your Azure AD tenant to support API access by a
service principal which is used in app-owns-data embedding. By default,
any code running as a service principal is not allowed to call the Power
BI Service API. In the next step you will create a new Azure AD
application and add its service principal to the Azure AD group named
**Power BI Apps.** This is turn will make it possible for your
application to call the Power BI Service API with an app-only identity.

16. Execute the PowerShell script named
    **Create-AzureAD-Application.ps1**

17. Using Windows Explorer, look in the **StudentLabFiles** folder and
    locate the script named **Create-AzureAD-Application.ps1.**

18. Open **Create-AzureAD-Application.ps1** in a text editor such as the
    Windows PowerShell ISE or Notepad.

19. The script begins by calling **Connect-AzureAD** to establish a
    connection with Azure AD.

$authResult = Connect-AzureAD

20. The script contains two variables to set the application display
    name and a reply URL of **https://localhost:5001/signin-oidc**.

$appDisplayName = "App-Owns-Data Sample App"

$replyUrl = "https://localhost:5001/signin-oidc"

When you register a reply URL with **localhost** with a port number such
as **5001**, Azure AD will allow you to perform testing with reply URLs
that use localhost and any other port number. For example, you can use a
reply URL of **https://localhost:44300/signin-oidc**.

21. The script also contains the code below which creates a new
    **PasswordCredential** object for an app secret.

\# create app secret

$newGuid = New-Guid

$appSecret =
(\[System.Convert\]::ToBase64String(\[System.Text.Encoding\]::UTF8.GetBytes(($newGuid))))+"="

$startDate = Get-Date

$passwordCredential = New-Object -TypeName
Microsoft.Open.AzureAD.Model.PasswordCredential

$passwordCredential.StartDate = $startDate

$passwordCredential.EndDate = $startDate.AddYears(1)

$passwordCredential.KeyId = $newGuid

$passwordCredential.Value = $appSecret

22. Down below, you can see the call to the **New-AzureADApplication**
    cmdlet which creates a new Azure AD application.

\# create Azure AD Application

$aadApplication = New-AzureADApplication \`

-DisplayName $appDisplayName \`

-PublicClient $false \`

-AvailableToOtherTenants $false \`

-ReplyUrls @($replyUrl) \`

-Homepage $replyUrl \`

-PasswordCredentials $passwordCredential

23. Moving down, you should see code which determines the Object ID for
    the service principal of the new application..

\# create applicaiton's service principal

$appId = $aadApplication.AppId

$appObjectId = $aadApplication.ObjectId

$serviceServicePrincipal = New-AzureADServicePrincipal -AppId $appId

$serviceServicePrincipalObjectId = $serviceServicePrincipal.ObjectId

24. Below that code there is code which add the service principal to the
    Azure AD security group named **Power BI Apps**.

\# add the service princiapl to Azure AD security group named 'Power BI
Apps'

$adSecurityGroupName = "Power BI Apps"

$adSecurityGroup = Get-AzureADGroup -Filter "DisplayName eq
'$adSecurityGroupName'"

if($adSecurityGroup) {

\# Add service principal of the new app as member of Power BI Apps group

Add-AzureADGroupMember -ObjectId $($adSecurityGroup.ObjectId) \`

-RefObjectId $($serviceServicePrincipalObjectId)

\# Display members of the Power BI Apps group

Write-Host "Members of Azure AD group named $adSecurityGroupName"

$members = Get-AzureADGroupMember -ObjectId $($adSecurityGroup.ObjectId)

$members \| Format-Table ObjectType, ObjectId, DisplayName

}

25. Execute **Create-Azure-ADApplication.ps1**.using the Windows
    PowerShell ISE or the Windows PowerShell command line.

Note you must execute this script using Windows PowerShell 5 and not
PowerShell 7. This restriction is due to incompatibilities between
PowerShell7 and the PowerShell module named **AzureAD**.

26. When prompted for credentials, log in with an Azure AD user account
    in the same tenant where you are using Power BI.

27. When the PowerShell script runs successfully, it will create and
    open a text file named **AppOwnsDataSampleApp.txt**.

<img src="images\media\image14.png" style="width:2.9066in;height:1.46162in" />

You should leave the text file **AppOwnsDataSampleApp.txt** open for
now. This file contains JSON configuration data that you will copy and
paste into the **appsettings.json**.file of the new .NET 5 project you
will create the next exercise.

### Exercise 2: Create a .NET 5 Project for a Secure Web Application

In this exercise, you will use the .NET CLI to create a new .NET 5
project for an MVC web application with support for authentication using
the new Microsoft authentication library named Microsoft.Identity.Web..

1.  Create a new .NET 5 project for an ASP.NET MVC web application.

<!-- -->

28. Using Windows Explorer, create a child folder inside the
    **C:\\DevCamp** folder named **AppOwnsData**.

<img src="images\media\image15.png" style="width:3.28592in;height:0.93489in" />

29. Launch Visual Studio Code.

30. Use the **Open Folder…** command in Visual Studio Code to open the
    **AppOwnsData** folder.

<img src="images\media\image16.png" style="width:3.50074in;height:0.86874in" />

1.  Once you have open the **AppOwnsData** folder, close the **Welcome**
    page.

<img src="images\media\image17.png" style="width:5.08505in;height:0.65461in" />

31. Use the Terminal console to verify the current version of .NET

32. Use the **Terminal &gt; New Terminal** command or the
    \[**Ctrl+Shift+\`**\] keyboard shortcut to open the Terminal
    console.

<img src="images\media\image18.png" style="width:5.49123in;height:0.8669in" />

1.  You should now see a Terminal console with a cursor where you can
    type and execute command-line instructions.

<img src="images\media\image19.png" style="width:5.1788in;height:1.65401in" />

33. Type the following **dotnet** command-line instruction into the
    console and press **Enter** to execute it.

dotnet --version

1.  When you run the command, the **dotnet** CLI should respond by
    display the .NET version number.

<img src="images\media\image20.png" style="width:3.20724in;height:0.81053in" />

Make sure you .NET version number is **5.0.100** at a minimum. If you
are working with .NET CORE version 3.1 or early, the project templates
for creating new web applications are much different and these lab
instructions will not work as expected. If you do not have .NET 5
installed, you must install the [.NET 5
SDK](https://dotnet.microsoft.com/download/dotnet/5.0) before you can
move past this point.

34. Run .NET CLI commands to register a self-signed certificate to
    enabled the .NET debugger to use SSL with https://localhost.

35. In the Terminal console, type and execute the following command to
    generate a new .NET console application.

dotnet dev-certs https --trust

36. Once you have run this command, you should now be able to run
    application in the debugger at **https://localhost:5001**.

37. Run .NET CLI commands to create a new ASP.NET MVC project.

38. In the Terminal console, type and execute the following command to
    generate a new .NET console application.

dotnet new mvc --auth SingleOrg

The **--auth SingleOrg** parameter instructs the .NET 5 CLI to generate
the new web application with extra code with authentication support
using Microsoft's new authentication library named
Microsoft.Identity.Web.

39. After running the **dotnet new** command, you should see that quite
    a few new folders and files have been added to the project.

<img src="images\media\image21.png" style="width:4.39642in;height:1.81352in" />

40. Open the **AppOwnsData.csproj** file and locate the reference to
    **Microsoft.Idebtity.Web** and **Microsoft.Idebtity.Web.UI**.

<img src="images\media\image22.png" style="width:5.94479in;height:1.81934in" />

41. Close the **AppOwnsData.csproj** without saving any changes.

Key point.Microsoft.Identity.Web has been added to project along with
scaffolding code to authenticate user. All you need to do

1.  Copy the JSON in **AppOwnsDataSampleApp.txt** into the
    **appsettings.json** file in your project.

    1.  Return to the **AppOwnsData** project in Visual Studio Code and
        open the **appsettings.json** file.

    2.  The **appsettings.json** file should initially appear like the
        screenshot below.

<img src="images\media\image23.png" style="width:3.70774in;height:1.61962in" />

42. Delete the contents of **appsettings.json** and replace it by
    copying and pasting the contents of **AppOwnsDataSampleApp.txt**

<img src="images\media\image24.png" style="width:5.76378in;height:1.9853in" />

Note the **PowerBi:ServiceRootUrl** parameter has been added as a custom
configuration value to track the base URL to the Power BI Service API.
When programming against the Power BI Service in the Microsoft public
cloud, the URL is <https://api.powerbi.com/>. However, the base URL for
the Power BI Service API will be different in other clouds such as the
government cloud. Therefore, this value will be stored as a project
configuration value so it is easy to change if required. [More
info](https://docs.microsoft.com/en-us/power-bi/developer/embedded/embed-sample-for-customers-national-clouds)

43. Configure Visual Studio Code with the Omnisharp extensions needed
    for C\# development with .NET 5.

44. Click on the button at the bottom of the left navigation menu to
    display the **EXTENSION** pane.

45. You should be able to see what extensions are currently installed.

46. You should also be able to search to find new extensions you'd like
    to install.

<img src="images\media\image25.png" style="width:3.72571in;height:2.39635in" />

47. Find and install the **C\#** extension from Microsoft if it is not
    installed. This extension is also known as the Omnisharp extension

<img src="images\media\image26.png" style="width:6.10187in;height:1.529in" />

48. Find and install the **Debugger for Chrome** extension from
    Microsoft if it is not already installed.

49. You should be able to confirm that the **C\#** extension and the
    **Debugger for Chrome** extensions are now installed.

<img src="images\media\image27.png" style="width:3.32583in;height:1.11373in" />

It is OK if you have other Visual Studio Code extensions installed as
well. It's just important that you install these two extensions in
addition to whatever other extensions you may have installed.

50. Generate the project assets required for building your project and
    running it in the .NET debugger.

51. Open the Visual Studio Code Command Palette by using the
    **Ctrl**+**Shift**+**P** keyboard combination.

52. Type .NET into the Command Palette search box.

53. Select the command titled **.NET: Generate Assets for Build and
    Debug**.

<img src="images\media\image28.png" style="width:6.08443in;height:0.85504in" />

54. When the command runs successfully, in generates the files
    **launch.json** and **tasks.json** a new folder named **.vscode**.

<img src="images\media\image29.png" style="width:6.57017in;height:1.79253in" />

It's not uncommon for the configuration of the C\# extension (i.e.
Omnisharp) in Visual Studio Code to cause errors when running the
command to generate the assets for build and debug. If this is the case,
it can be tricky to fix this problem. If the previous step to generate
build and debug assets did not successfully create the **launch.json**
file and the **tasks.json** file in the **.vscode** folder, you can copy
these two essential files from **Troubleshooting/.vscode** folder
located inside the **StudentFiles** folder.

55. Run and test the **AppOwnsData** web application using Visual Studio
    Code and the .NET 5 debugger.

56. Start the .NET 5 debugger by selecting **Run &gt; Start Debugging**
    or by pressing the **{F5}** keyboard shortcut.

<img src="images\media\image30.png" style="width:6.91776in;height:0.88407in" />

The AppOwnsData web application is currently configured authenticate the
user before the user is able to view the home page. Therefore, you will
be prompted to log in as soon as you launch the application in the .NET
debugger.

57. When prompted to Sign in, log in using your organizational account.

<img src="images\media\image31.png" style="width:1.29345in;height:0.89858in" />

58. Once you have signed in, you will be prompted by the **Permissions
    requested** dialog to grant consent to the application.

59. Click the **Accept** button to continue.

<img src="images\media\image32.png" style="width:1.51057in;height:1.80774in" />

60. You should now see the home page for the **AppOwnsData** web
    application which should match the following screenshot.

<img src="images\media\image33.png" style="width:6.20175in;height:1.67017in" />

61. You're done testing. Close the browser, return to Visual Studio Code
    and stop the debug session using the debug toolbar.

<img src="images\media\image34.png" style="width:2.4908in;height:0.66926in" />

You now got to the point where the **AppOwnsData** web application is up
and running and it can successfully authenticate the user. Over the next
few steps you will add some custom HTML and CSS styles to make the
application a bit more stylish. You will also configure the home page to
allow for anonymous access in order to create a better login experience
for the application's users.

62. Copy and paste a set of pre-written CSS styles into the
    **AppOwnsData** project's **site.css** file.

63. Expand the **wwwroot** folder and then expand the **css** folder
    inside to examine the contents of the **wwwroot/css** folder.

64. Open the CSS file named **site.css** and delete any existing content
    inside.

<img src="images\media\image35.png" style="width:1.88751in;height:2.05059in" />

65. Using the Windows Explorer, look inside the **StudentLabFiles**
    folder and locate the file named **Exercise 2 - site.css.txt**.

66. Open **Exercise 2 - site.css.txt** up in a text editor and copy all
    of its contents into the Windows clipboard.

67. Return to Visual Studio Code and paste the contents of the Windows
    clipboard into **sites.css**.

<img src="images\media\image36.png" style="width:5.93951in;height:1.83631in" />

68. Save your changes and close **site.css**.

69. Copy a custom **favicon.ico** file to the **wwwroot** folder.

70. Using the Windows Explorer, look inside the **StudentLabFiles**
    folder and locate the file named **favicon.ico**.

71. Copy the **favicon.ico** file into the **wwwroot** folder of your
    project.

<img src="images\media\image37.png" style="width:2.29687in;height:1.3125in" />

Any file you add the **wwwroot** folder will appear at the root folder
of the website created by the **AppOwnsData** project. By adding the
**favicon.ico** file, this web application will now display a custom
**favicon.ico** in the browser page tab.

72. Modify the partial razor view file named **\_LoginPartial.cshtml**
    to display the user display name instead of the user principal name.

73. Expand the **Views &gt; Shared** folder and locate the partial view
    named **\_LoginPartial.cshtml**.

74. Open **\_LoginPartial.cshtml** in an editor window.

75. In the existing code, locate the code **Hello @User.Identity.Name!**
    which displays a message with the user principal name.

<img src="images\media\image38.png" style="width:3.15059in;height:0.93905in" />

76. Replace **Hello @User.Identity.Name!** with **Hello
    @User.FindFirst("name").Value** as shown in the following
    screenshot.

<img src="images\media\image39.png" style="width:4.37917in;height:0.39811in" />

With this update, the application will display the user's display name
in the welcome message instead of using the user principal name. In
other words, the user greeting will now display a message like **Hello
Betty White** instead of **Hello BettyW@Contoso.com!**.

77. Save your changes and close **\_LoginPartial.cshtml**.

78. Modify **Index.cshtml** to display different HTML output depending
    on whether the user has logged in or not.

79. Expand the **Views &gt; Home** folder and locate the view file named
    **Index.cshtml**.

80. Open **Index.cshtml** in an editor window.

81. Delete the contents of **Index.cshtml** and replace it with the code
    shown in the following code listing.

@using System.Security.Principal

@if (User.Identity.IsAuthenticated) {

&lt;div class="jumbotron"&gt;

&lt;h2&gt;Welcome @User.FindFirst("name").Value&lt;/h2&gt;

&lt;p&gt;You have now logged into this application.&lt;/p&gt;

&lt;/div&gt;

}

else {

&lt;div class="jumbotron"&gt;

&lt;h2&gt;Welcome to the App-Owns-Data Tutorial&lt;/h2&gt;

&lt;p&gt;Click the &lt;strong&gt;sign in&lt;/strong&gt; link in the
upper right to get started&lt;/p&gt;

&lt;/div&gt;

}

82. Once you have copied the code from above, save your changes and
    close **Index.cshtml**.

<img src="images\media\image40.png" style="width:4.37917in;height:1.49721in" />

When you create a new .NET 5 project which supports authentication, the
underlying project template creates a home page that requires
authentication. To support a more natural login experience for the user,
it often makes sense to configure your application so that an anonymous
user can access the home page. In the next step you will modify the
**Home** controller in order to make the home page accessible to
anonymous users.

83. Modify the **Index** action method in **HomeController.cs** to
    support anonymous access.

84. Inside the **Controllers** folder, locate **HomeControllers.cs** and
    open this file in an editor window.

85. Locate the **Index** method inside the **HomeController** class.

<img src="images\media\image41.png" style="width:4.2763in;height:2.19345in" />

86. Add the **\[AllowAnonymous\]** attribute to the **Index** method as
    shown in the following code listing.

\[AllowAnonymous\]

public IActionResult Index()

{

return View();

}

87. Save your changes and close **HomeController.cs**.

It's time again to test this web application in the .NET debugger so you
can see the effects of your changes. In the next step, you will start
the debugger so you can start up the web application and test the user
authentication experience in the browser.

88. Test the **AppOwnsData** project by running it in the .NET debugging
    environment.

89. Start the .NET debugger by selecting **Run &gt; Start Debugging** or
    by pressing the **{F5}** keyboard shortcut.

90. Once the debugging session has initialized, the browser should
    display the home page using anonymous access.

91. Click the **Sign in** link to test the user experience when
    authenticating with Azure AD.

<img src="images\media\image42.png" style="width:3.88294in;height:1.16488in" />

92. Once you've signed in, you should be able to see the user display
    name in the welcome message in the upper right corner.

<img src="images\media\image43.png" style="width:3.9592in;height:1.12202in" />

If the web page does not appear with a yellow background as shown in the
screenshot above, it's possible your browser has cached the original
version of the **site.css** file. If this is the case, you must clear
the browser cache so it loads the latest version of **site.css**.

93. Test the user experience for logging out.

94. Click the **Sign out** link to begin the logout experience.

<img src="images\media\image44.png" style="width:5.00774in;height:0.77261in" />

95. After logging out, you'll be directed to the
    **Microsoft.Identity.Web** logout page at
    **/MicrosoftIdentity/Account/SignedOut**.

<img src="images\media\image45.png" style="width:5.07437in;height:0.97917in" />

96. You're done testing. Close the browser, return to Visual Studio Code
    and stop the debug session using the debug toolbar.

In the next step, you will add a new controller action and view named
**Embed**. However, instead of creating a new controller action and
view, you will simply the rename the controller action and view named
**Privacy** that were automatically added by the project template.

97. Create a new controller action named **Embed**.

98. Locate the **HomeController.cs** file in the **Controllers** folder
    and open it in an editor window.

99. Look inside the **HomeController** class and locate the method named
    **Privacy**.

\[AllowAnonymous\]

public IActionResult Index() {

return View();

}

public IActionResult Privacy() {

return View();

}

100. Rename of the **Privacy** method to **Embed**. No changes to the
     method body are required.

\[AllowAnonymous\]

public IActionResult Index() {

return View();

}

public IActionResult Embed() {

return View();

}

Note that, unlike the **Index** method, the **Embed** method does not
have the **AllowAnonymous** attribute. That means only authenticated
users will be able to navigate to this page. One really nice aspect of
the ASP.NET MVC architecture is that it will automatically trigger an
interactive login whenever an anonymous user attempts to navigate to a
secured page such as **Embed**.

101. Create a new MVC view for the **Home** controller named
     **Embed.cshtml**.

102. Look inside the **Views &gt; Home** folder and locate the razor
     view file named **Privacy.cshtml**.

<img src="images\media\image46.png" style="width:2.47599in;height:1.12202in" />

103. Rename the **Privacy.cshtml** razor file to **Embed.cshtml**..

<img src="images\media\image47.png" style="width:1.86488in;height:1.00417in" />

104. Open **Embed.cshtml** in a code editor.

105. Delete the existing contents of **Embed.cshtml** and replace it
     with the follow line of HTML code.

&lt;h2&gt;TODO: Embed Report Here&lt;/h2&gt;

106. Save your changes and close **Embed.cshtml**.

In a standard .NET 5 web application that uses MVC, there is a shared
page layout defined in a file named **\_Layouts.cshtml** which is
located in the **Views &gt; Shared** folder. In the next step you will
modify the shared layout in the **\_Layouts.cshtml** file so that you
can add a link to the **Embed** page into the top navigation menu.

107. Modify the shared layout in **\_Layout.cshtml** to include a link
     to the **Embed** page.

108. Inside the **Views &gt; Shared** folder, locate
     **\_Layouts.cshtml** and open this shared view file in an editor
     window.

109. Using Windows Explorer, look inside the **StudentLabFiles** folder
     and locate the file named **\_Layout.cshtml**.

110. Open **Exercise 2 - \_Layout.cshtml.txt** in the
     **StudentLabFiles** folder copy its contents to the Windows
     clipboard.

111. Return to Visual Studio Code and paste the contents of the Windows
     clipboard into the **\_Layouts.cshtml** file.

<img src="images\media\image48.png" style="width:4.99016in;height:1.87917in" />

112. Save your changes and close **\_Layouts.cshtml**

113. Run the web application in the Visual Studio Code debugger to test
     the new **Embed** page.

114. Start the Visual Studio Code debugger by selecting **Run &gt; Start
     Debugging** or by pressing the **{F5}** keyboard shortcut.

115. The **AppOwnsData** web application should display the home page as
     shown to an anonymous user.

If you are automatically signed in by the application, then you should
sign out and then click **Home**.

116. Click on the **Embed** link in the top nav menu to navigate to the
     **Embed** page.

<img src="images\media\image49.png" style="width:5.6376in;height:1.06488in" />

When you attempt to navigate to the **Embed** page as an anonymous user,
you'll be automatically prompted to log in.

117. Log in using your user name and password.

<img src="images\media\image50.png" style="width:2.02869in;height:1.57895in" />

118. Once you have logged in, you should be automatically redirected to
     the **Embed** page.

<img src="images\media\image51.png" style="width:6.58336in;height:1.04057in" />

119. You're done testing. Close the browser, return to Visual Studio
     Code and stop the debug session using the debug toolbar.

The next step is an *<u>optional step</u>* for those campers that prefer
developing with Visual Studio 2019 instead of Visual Studio Code.  
If you are happy developing with Visual Studio Code and are not
interested in developing .NET 5 projects using Visual Studio 2019, you
can skip the next step and move ahead to *Exercise 3: Call the Power BI
Service API*.

120. Open and test the **AppOwnsData** project using Visual Studio 2019.

121. Launch Visual Studio 2019 – You can use any edition including the
     Enterprise edition, Pro edition or Community edition.

122. From the **File** menu, select the **Open &gt; Project/Solution…**
     command.

<img src="images\media\image52.png" style="width:6.30685in;height:1.08443in" />

123. In the **Open Project/Solution** dialog, select the
     **AppOwnsData.csproj** file in the **AppOwnsData** folder and click
     **Open**.

124. The **AppOwnsData** project should now be open in Visual Studio
     2019 as shown in the following screenshot.

<img src="images\media\image53.png" style="width:1.85714in;height:2.16667in" />

There is one big difference between developing with Visual Studio Code
and Visual Studio 2019. Visual Studio Code only requires project files
(\*.csproj). However, Visual Studio 2019 requires that you work with
both project files and solution files (\*.sln). In the next step you
will save a new project file for the **AppOwnsData** solution to make it
easier to develop this project with Visual Studio 2019.

125. In the **Solution Explorer** on the right, select the top node for
     the **AppOwnsData** project.

126. From the **File** menu, select the **Save All** menu command.

<img src="images\media\image54.png" style="width:4.80774in;height:1.38478in" />

127. Save the solution file **AppOwnsData.sln** in the **AppOwnsData**
     project folder

<img src="images\media\image55.png" style="width:2.14286in;height:1.30833in" />

128. Run the AppOwnsData application in the Visual Studio debugger.

129. Press {F5} to launch the Visual Studio debugger.

130. If you are prompted with a dialog prompting you to trust an SSL
     certificate, click **Yes** to continue.

<img src="images\media\image56.png" style="width:1.56488in;height:0.65011in" />

131. If you are prompted with a **Security Warning** dialog, click
     **Yes** to trust the certificate.

<img src="images\media\image57.png" style="width:1.27757in;height:1.22202in" />

132. sssss

<img src="images\media\image58.png" style="width:4.53273in;height:1.108in" />

133. Test the application by signing in, signing out and clicking the
     **Embed** link.

134. Once you have tested the application, close the browser, return to
     Visual Studio and terminate the debugging session.

Remember that the **AppOwnsData.sln** file is only used by Visual Studio
2019 and it not used at all in Visual Studio Code.

The following lab exercises will go back to developing with Visual
Studio Code. However, you can use whichever IDE you like better. Also,
you can and open and test all the lab solutions using either Visual
Studio Code or Visual Studio 2019.

### Exercise 3: Call the Power BI Service API

In this exercise, you will begin your work by creating a new Power BI
workspace and importing a PBIX file to that you have a report for
testing purposes. After that, you will add support to the
**AppOwnsData** web application to acquire app-only access tokens from
Azure AD and to call the Power BI Service API. By the end of this
exercise, your code will be able to call to the Power BI Service API to
retrieve data about a report required for embedding.

1.  Using the browser, log into the Power BI Service in the Microsoft
    365 tenant serving as your Power BI development environment.

<!-- -->

135. Navigate the Power BI portal at <https://app.powerbi.com> and if
     prompted, log in using your credentials.

<!-- -->

2.  Create a new app workspace named **Dev Camp Lab**.

    1.  Click the **Workspace** flyout menu in the left navigation.

<img src="images\media\image59.png" style="width:1.02202in;height:1.35419in" />

2.  Click the **Create app workspace** button to display the **Create an
    > app workspace** dialog.

<img src="images\media\image60.png" style="width:1.63287in;height:0.97917in" />

3.  In the **Create an app workspace** pane, enter a workspace name such
    > as **Dev Camp Lab**.

4.  Click the **Save** button to create the new app workspace.

<img src="images\media\image61.png" style="width:2.27193in;height:2.45614in" />

5.  When you click **Save**, the Power BI service should create the new
    > app workspace and then switch your current Power BI session to be
    > running within the context of the new **Dev Camp Lab** workspace.

<img src="images\media\image62.png" style="width:3.92067in;height:1.96488in" />

Now that you have created the new app workspace, the next step is to
upload a PBIX project file created with Power BI Desktop. You are free
to use your own PBIX file as long as the PBIX file does not have
row-level security (RLS) enabled. If you don't have your own PBIX file,
you can download the sample PBIX file named [**COVID-19
US.pbix**](https://github.com/PowerBiDevCamp/pbix-samples/raw/main/COVID-19%20US.pbix)
and use that instead.

136. Upload a PBIX file to create a new report and dataset.

137. Click **Add content** to navigate to the **Get Data** page.

138. Click the **Get** button in the **Files** section.

<img src="images\media\image63.png" style="width:4.89345in;height:2.01175in" />

139. Click on **Local File** in order to select a PBIX file that you
     have on your local hard drive.

<img src="images\media\image64.png" style="width:3.7193in;height:0.95049in" />

140. Select the PBIX file and click the **Open** button to upload it to
     the Power BI Service.

<img src="images\media\image65.png" style="width:2.5525in;height:1.13799in" />

141. The Power BI Service should have created a report and a dataset
     from the PBIX file you uploaded.

142. If the Power BI Service created a dashboard as well, delete this
     dashboard as you will not need it.

<img src="images\media\image66.png" style="width:3.42983in;height:1.60251in" />

143. Open the report to see what it looks like when displayed in the
     Power BI Service.

144. Click on the report to open it.

<img src="images\media\image67.png" style="width:5.34325in;height:1.79345in" />

145. You should now be able to see the report.

<img src="images\media\image68.png" style="width:5.45059in;height:1.703in" />

In the next step, you will find and record the GUID-based IDs for the
report and its hosting workspace. You will then use these IDs later in
this exercises when you first write the code to embed a report in the
**AppOwnsData** web application.

146. Get the Workspace ID and the Report ID from the report URL.

147. Locate and copy the app workspace ID from the report URL by copying
     the GUID that comes after **/groups/**.

<img src="images\media\image69.png" style="width:4.8626in;height:1.13461in" />

148. Open up a new text file in a program such as Notepad and paste in
     the value of the workspace ID.

149. Locate and copy the report ID from the URL by copying the GUID that
     comes after **/reports/**.

<img src="images\media\image70.png" style="width:4.98246in;height:1.17875in" />

150. Copy the report ID into the text file Notepad.

<img src="images\media\image71.png" style="width:3.27917in;height:0.69929in" />

Leave the text file open for now. In a step later in this exercise, you
will copy and paste these IDs into your C\# code.

151. Add the Azure AD service principal for the **AppOwnsData**
     application to the **Dev Camp Lab** workspace as an administrator.

152. Click the **Dev Camp Lab** workspace in the left navigation to
     display the workspace summary page.

153. Click the **Access** link to open the **Access** pane where you can
     configure who has access to workspace resources.

<img src="images\media\image72.png" style="width:3.80774in;height:1.63189in" />

154. In the search box with the caption of **Enter email address**, type
     **App-Owns-Data** to find the Azure AD application.

<img src="images\media\image73.png" style="width:5.20175in;height:1.32934in" />

155. Select the Azure AD application named **App-Owns-Data Sample App**.

<img src="images\media\image74.png" style="width:2.39345in;height:1.10467in" />

156. Select **Admin** in the dropdown menu to specify the level of
     access and then click the **Add** button.

<img src="images\media\image75.png" style="width:2.01612in;height:0.99345in" />

157. You should now be able to confirm that the **App-Owns-Data Sample
     App** has been configured as a workspace admin.

<img src="images\media\image76.png" style="width:1.66488in;height:1.44355in" />

158. Close the **Access** pane.

Now you will move from working with Power BI in the browser back to the
AppOwnsData project in Visual Studio Code.

159. Add the NuGet package for the **Power BI .NET SDK**.

     1.  Return to the instance of Visual Studio Code where you are
         working on the **AppOwnsData** project.

     2.  Move back to the Terminal so you can execute another dotnet CLI
         command.

     3.  Type and execute the following **dotnet add package** command
         to add the NuGet package for the **Power BI .NET SDK**.

dotnet add package Microsoft.PowerBi.Api

<img src="images\media\image77.png" style="width:5.19298in;height:1.2694in" />

160. Open the **AppOwnsData.csproj** file. You should now see this file
     contains a package reference to **Microsoft.PowerBi.Api**.

<img src="images\media\image78.png" style="width:4.54028in;height:1.37719in" />

161. Close the the **AppOwnsData.csproj** file without saving any
     changes.

Your project now includes the NuGet package for the Power BI .NET SDK so
you can begin to program against the classes from this package in the
**Microsoft.PowerBI.Api** namespace.

162. Create a new C\# class named **PowerBiServiceApi** in which you
     will add code for calling the Power BI Service API.

163. Return to the **AppOwnsData** project in Visual Studio Code.

164. Create a new top-level folder in the **AppOwnsData** project named
     **Services**.

165. Inside the **Services** folder, create a new C\# source file named
     **PowerBiServiceApi.cs**.

<img src="images\media\image79.png" style="width:3.50255in;height:1.93631in" />

166. Copy and paste the following code into **PowerBiServiceApi.cs** to
     provide a starting point.

using System;

using System.Linq;

using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Microsoft.Identity.Web;

using Microsoft.Rest;

using Microsoft.PowerBI.Api;

using Microsoft.PowerBI.Api.Models

using Newtonsoft.Json;

namespace AppOwnsData.Services {

public class EmbeddedReportViewModel {

//TODO: implement this class

}

public class PowerBiServiceApi {

//TODO: implement this class

}

}

167. Implement the **EmbeddedReportViewModel** class using the following
     code.

public class EmbeddedReportViewModel {

public string Id;

public string Name;

public string EmbedUrl;

public string Token;

}

The **EmbeddedReportViewModel** class is designed as a view model which
defines the structure the embedding data required for a Power BI report.
You will use this class later in this lab to pass embedding data for a
report from an MVC controller to an MVC view.

168. Begin your implementation by adding two private fields named
     **tokenAcquisition** and **urlPowerBiServiceApiRoot**.

public class PowerBiServiceApi {

private ITokenAcquisition tokenAcquisition { get; }

private string urlPowerBiServiceApiRoot { get; }

}

169. Add the following constructor to initialize the two private fields
     named **tokenAcquisition** and **urlPowerBiServiceApiRoot**.

public class PowerBiServiceApi {

private ITokenAcquisition tokenAcquisition { get; }

private string urlPowerBiServiceApiRoot { get; }

public PowerBiServiceApi(IConfiguration configuration, ITokenAcquisition
tokenAcquisition) {

this.urlPowerBiServiceApiRoot =
configuration\["PowerBi:ServiceRootUrl"\];

this.tokenAcquisition = tokenAcquisition;

}

}

This code uses the .NET dependency injection (DI) pattern. When your
class needs to use a service, you can simply add a constructor parameter
based on the type for that service and the .NET runtime takes care of
passing the service instance at run time. In this case, the constructor
is injecting an instance of the .NET configuration service using the
**IConfiguration** parameter which is used to retrieve the
**PowerBi:ServiceRootUrl** configuration value from
**appsettings.json**. The **ITokenAcquisition** parameter which is named
**tokenAcquisition** holds a reference to the Microsoft authentication
service provided by the **Microsoft.Identity.Web** library and will be
used to acquire access tokens from Azure AD.

170. Place your cursor at the bottom of the **PowerBiServiceApi** class
     and add another new line so you can add more members.

171. At the bottom off the **PowerBiServiceApi** class, add the
     following static read-only field named **RequiredScopes**.

public const string powerbiApiDefaultScope =
"https://analysis.windows.net/powerbi/api/.default";

The **powerbiApiDefaultScope** constant is a string with the default
permissions scope for the Power BI Service API. Your application will
pass this permission scope when it calls to Azure AD to acquire an
app-only access token.

172. Move down in the **PowerBiServiceApi** class below the
     **RequiredScopes** field and add the **GetAccessToken** method.

public string GetAccessToken() {

return
this.tokenAcquisition.GetAccessTokenForAppAsync(powerbiApiDefaultScope).Result;

}

173. Move down below the **GetAccessToken** method and add the
     **GetPowerBiClient** method.

public PowerBIClient GetPowerBiClient() {

var tokenCredentials = new TokenCredentials(GetAccessToken(), "Bearer");

return new PowerBIClient(new Uri(urlPowerBiServiceApiRoot),
tokenCredentials);

}

174. Move down below the **GetPowerBiClient** method and add the
     **GetReport** method.

public async Task&lt;EmbeddedReportViewModel&gt; GetReport(Guid
WorkspaceId, Guid ReportId) {

PowerBIClient pbiClient = GetPowerBiClient();

// call to Power BI Service API to get embedding data

var report = await pbiClient.Reports.GetReportInGroupAsync(WorkspaceId,
ReportId);

// generate read-only embed token for the report

var datasetId = report.DatasetId;

var tokenRequest = new GenerateTokenRequest(TokenAccessLevel.View,
datasetId);

var embedTokenResponse =

await pbiClient.Reports.GenerateTokenAsync(WorkspaceId, ReportId,
tokenRequest);

var embedToken = embedTokenResponse.Token;

// return report embedding data to caller

return new EmbeddedReportViewModel {

Id = report.Id.ToString(),

EmbedUrl = report.EmbedUrl,

Name = report.Name,

Token = embedToken

};

}

175. Save and close **PowerBIServiceApi.cs**.

Note that **Exercise 3 - PowerBiServiceApi.cs.txt** in the
**StudentLabFiles** folder contains the final code for
**PowerBiServiceApi.cs**.

176. Modify the code in **Startup.cs** to properly register the services
     required for user authentication and access token acquisition.

177. Open the **Startup.cs** file in an editor window.

178. Underneath the existing **using** statements, add the following
     **using** statement;

using AppOwnsData.Services;

179. Look inside the **ConfigureServices** method and locate the
     following line of code.

public void ConfigureServices(IServiceCollection services) {

services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)

.AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));

180. Remove the code that calls **AddAuthentication** and
     **AddMicrosoftIdentityWebApp** on the services object.

181. Add the following code to the top of the **ConfigureServices**
     method.

public void ConfigureServices(IServiceCollection services) {

services

.AddMicrosoftIdentityWebAppAuthentication(Configuration)

.EnableTokenAcquisitionToCallDownstreamApi()

.AddInMemoryTokenCaches();

182. Move below the call to **AddInMemoryTokenCaches** and add the
     following code.

services.AddScoped(typeof(PowerBiServiceApi));

183. At this point, the **ConfigureService** method in **Startup.cs**
     should match the following code listing.

public void ConfigureServices(IServiceCollection services) {

services

.AddMicrosoftIdentityWebAppAuthentication(Configuration)

.EnableTokenAcquisitionToCallDownstreamApi()

.AddInMemoryTokenCaches();

services.AddScoped(typeof(PowerBiServiceApi));

services.AddControllersWithViews(options =&gt; {

var policy = new AuthorizationPolicyBuilder()

.RequireAuthenticatedUser()

.Build();

options.Filters.Add(new AuthorizeFilter(policy));

});

services.AddRazorPages()

.AddMicrosoftIdentityUI();

}

The code in **ConfigureServices** accomplishes several important things.
The call to **AddMicrosoftWebAppCallsWebApi** configures the Microsoft
authentication library to acquire access tokens. Next, the call to
**AddInMemoryTokenCaches** configures a token cache that the Microsoft
authentication library will use to cache app-only access tokens behind
the scenes. Finally, the call to
**services.AddScoped(typeof(PowerBiServiceApi))** configures the
**PowerBiServiceApi** class as a service class that can be added to
other classes using the dependency injection (DI) pattern.

184. Modify the **HomeController** class to program against the
     **PowerBiServiceApi** class.

185. Inside the **Controllers** folder, locate **HomeController.cs** and
     open it in an editor window.

186. Underneath the existing **using** statements, add a **using**
     statement to import the **AppOwnsData.Services** namespace.

using AppOwnsData.Services;

187. At the top of the **HomeController** class locate the **\_logger**
     field and the constructor as shown in the following code listing.

\[Authorize\]

public class HomeController : Controller {

private readonly ILogger&lt;HomeController&gt; \_logger;

public HomeController(ILogger&lt;HomeController&gt; logger) {

\_logger = logger;

}

188. Remove the **\_logger** field and the existing constructor and
     replace them with the following code.

\[Authorize\]

public class HomeController : Controller {

private PowerBiServiceApi powerBiServiceApi;

public HomeController(PowerBiServiceApi powerBiServiceApi) {

this.powerBiServiceApi = powerBiServiceApi;

}

This is another example of using dependency injection. Since you
registered the **PowerBiServiceApi** class as a service by calling
**services.AddScoped** in the **ConfigureServices** method, you can
simply add a **PowerBiServiceApi** parameter to the constructor and the
.NET 5 runtime will take care of creating a **PowerBiServiceApi**
instance and passing it to the constructor.

189. Locate the **Embed** method implementation in the
     **HomeController** class and replace it with the following code.

public async Task&lt;IActionResult&gt; Embed() {

// replace these two GUIDs with the workspace ID and report ID you
recorded earlier

Guid workspaceId = new Guid("11111111-1111-1111-1111-111111111111");

Guid reportId = new Guid("22222222-2222-2222-2222-222222222222");

var viewModel = await powerBiServiceApi.GetReport(workspaceId,
reportId);

return View(viewModel);

}

190. Modify the HTML and razor code in the view file named
     **Embed.cshtml**.

191. Locate the **Embed.cshtml** razor file inside the **Views &gt;
     Home** folder and open this file in an editor window.

192. Delete the contents of **Embed.cshtml** and replace it with the
     following code which creates a table to display report data.

@model AppOwnsData.Services.EmbeddedReportViewModel;

&lt;style&gt;

table td {

min-width: 120px;

word-break: break-all;

overflow-wrap: break-word;

font-size: 0.8em;

}

&lt;/style&gt;

&lt;h3&gt;Report View Model Data&lt;/h3&gt;

&lt;table class="table table-bordered table-striped table-sm" &gt;

&lt;tr&gt;&lt;td&gt;Report
Name&lt;/td&gt;&lt;td&gt;@Model.Name&lt;/td&gt;&lt;/tr&gt;

&lt;tr&gt;&lt;td&gt;Report
ID&lt;/td&gt;&lt;td&gt;@Model.Id&lt;/td&gt;&lt;/tr&gt;

&lt;tr&gt;&lt;td&gt;Embed
Url&lt;/td&gt;&lt;td&gt;@Model.EmbedUrl&lt;/td&gt;&lt;/tr&gt;

&lt;tr&gt;&lt;td&gt;Embed
Token&lt;/td&gt;&lt;td&gt;@Model.Token&lt;/td&gt;&lt;/tr&gt;

&lt;/table&gt;

193. The code in **Embed.cshtml** should now match the following
     screenshot..

<img src="images\media\image80.png" style="width:4.73684in;height:2.05263in" />

194. Save your changes and close **Embed.cshtml**.

195. Run the web application in the Visual Studio Code debugger to test
     the new **Embed** page.

196. Start the Visual Studio Code debugger by selecting **Run &gt; Start
     Debugging** or by pressing the **{F5}** keyboard shortcut.

197. The **AppOwnsData** web application should display the home page as
     shown to an anonymous user.

198. Click on the **Embed** link in the top nav menu to navigate to the
     **Embed** page.

<img src="images\media\image81.png" style="width:5.17544in;height:0.92008in" />

199. If you are prompted to enter your credentials, enter your user name
     and password and log in.

200. Once you navigate to the **Embed** page, it should display a table
     containing the embedding data for your Power BI report.

<img src="images\media\image82.png" style="width:5.76316in;height:2.80474in" />

201. You're done testing. Close the browser, return to Visual Studio
     Code and stop the debug session using the debug toolbar.

You are now half way in your development efforts to embed a Power BI
report. You have written the server-side code to call the Power BI
Service API and retrieve the data required to embed a report. In the
next exercise, you will complete the Power BI embedding implementation
by adding client-side JavaScript code which programs against the Power
BI JavaScript API.

### Exercise 4: Embed a Report using powerbi.js

In this exercise, you will modify the view named **Embed.cshtml** to
embed a Power BI report on a web page. Your work will involve adding a
new a JavaScript file named **embed.js** in which you will write the
minimal client-side code required to embed a report.

1.  Modify the razor view file named **Embed.cshtml**.

<!-- -->

202. Inside the **Views &gt; Home** folder, locate and open
     **Embed.cshtml** in an editor window.

203. Replace the contents of **Embed.cshtml** with the following code.

@model AppOwnsData.Services.EmbeddedReportViewModel;

&lt;div id="embed-container" style="height:800px;"&gt;&lt;/div&gt;

@section Scripts {

}

Note that the div element with the ID of **embed-container** will be
used as the embed container.

Over the next few steps, you will add three **script** tags into the
**Scrips** section. The benefit of adding script tags into the
**Scripts** section is that they will load after the JavaScript
libraries such as jquery which are loaded from the shared view
**\_Layout.cshtml**.

204. Place your cursor inside the **Scripts** section and paste in the
     following **script** tag to import **powerbi.min.js** from a CDN.

&lt;script
src="https://cdn.jsdelivr.net/npm/powerbi-client@2.13.3/dist/powerbi.min.js"&gt;&lt;/script&gt;

**powerbi.min.js** is the JavaScript file that loads the client-side
library named the **Power BI JavaScript API**.

205. Underneath the **script** tag for the Power BI JavaScript API, add
     a second **script** tag using the following code.

&lt;script&gt;

var viewModel = {

reportId: "@Model.Id",

embedUrl: "@Model.EmbedUrl",

token: "@Model.Token"

};

&lt;/script&gt;

This **script** tag is creates a JavaScript object named **viewModel**
which is accessible to the JavaScript code you'll write later in this
lab.

206. Underneath the other two **script** tags, add a third **script**
     tag to load a custom JavaScript file named **embed.js**.

&lt;script src="\~/js/embed.js"&gt;&lt;/script&gt;

Note that the JavaScript file named **embed.js** does not exist yet. You
will create the **embed.js** file in the next step.

207. When you are done, the contents you have in **Embed.cshtml** should
     match the following code listing.

@model AppOwnsData.Services.EmbeddedReportViewModel;

&lt;div id="embed-container" style="height:800px;"&gt;&lt;/div&gt;

@section Scripts {

&lt;script
src="https://cdn.jsdelivr.net/npm/powerbi-client@2.13.3/dist/powerbi.min.js"&gt;&lt;/script&gt;

&lt;script&gt;

var viewModel = {

reportId: "@Model.Id",

embedUrl: "@Model.EmbedUrl",

token: "@Model.Token"

};

&lt;/script&gt;

&lt;script src="\~/js/embed.js"&gt;&lt;/script&gt;

}

208. Save your changes and close **Embed.cshtml**.

The final step is to add a new JavaScript file named **embed.js** with
the code required to embed a report.

209. Add a new JavaScript file named **embed.js**.

210. Locate the top-level folder named **wwwroot** and expand it.

211. Locate the **js** folder inside the **wwwroot** folder and expand
     that.

212. Currently, there should be one file inside the **wwwroot &gt; js**
     folder named **site.js**.

<img src="images\media\image83.png" style="width:2.27728in;height:1.08583in" />

213. Add a new JavaScript file named **embed.js**.

<img src="images\media\image84.png" style="width:1.87719in;height:1.42337in" />

214. Add the JavaScript code to **embed.js** to embed a report.

215. Open **embed.js** in an editor window.

216. Delete whatever content exists inside **embed.js**.

217. Paste the following code into **embed.js** to provide a starting
     point.

$(function(){

// 1 - get DOM object for div that is report container

// 2 - get report embedding data from view model

// 3 - embed report using the Power BI JavaScript API.

// 4 - add logic to resize embed container on window resize event

});

You will now copy and paste four sections of JavaScript code into
**embed.js** to complete the implementation. Note that you can copy and
paste all the code at once by copying the contents of **Exercise 4 -
embed.js.txt** in the **StudentLabFiles** folder.

218. Add the following JavaScript code to create a variable named
     **reportContainer** which holds a reference to **embed-container**.

// 1 - get DOM object for div that is report container

var reportContainer = document.getElementById("embed-container");

219. Add code to create 3 variables named **reportId**, **embedUrl** and
     **token** which are initialized from the global **viewModel**
     object.

// 2 - get report embedding data from view model

var reportId = window.viewModel.reportId;

var embedUrl = window.viewModel.embedUrl;

var token = window.viewModel.token

Now this JavaScript code has retrieved the three essential pieces of
data from **window.viewModel** to embed a Power BI report.

220. Add the following code to embed a report by calling the
     **powerbi.embed** function provided by the Power BI JavaScript API.

// 3 - embed report using the Power BI JavaScript API.

var models = window\['powerbi-client'\].models;

var config = {

type: 'report',

id: reportId,

embedUrl: embedUrl,

accessToken: token,

permissions: models.Permissions.All,

tokenType: models.TokenType.Embed,

viewMode: models.ViewMode.View,

settings: {

panes: {

filters: { expanded: false, visible: true },

pageNavigation: { visible: false }

}

}

};

// Embed the report and display it within the div container.

var report = powerbi.embed(reportContainer, config);

Note that the variable named **models** is initialized using a call to
**window\['powerbi-client'\].models**. The **models** variable is used
to set configuration values such as **models.Permissions.All**,
**models.TokenType.Aad** and **models.ViewMode.View**.

A key point is that you need to create a configuration object in order
to call the **powerbi.embed** function. You can learn a great deal more
about creating the configuration object for Power BI embedding in [this
wiki](https://github.com/Microsoft/PowerBI-JavaScript/wiki) for the
Power BI JavaScript API.

221. Add the following JavaScript code to resize the **embed-container**
     div element whenever the window resize event fires.

// 4 - add logic to resize embed container on window resize event

var heightBuffer = 12;

var newHeight = $(window).height() - ($("header").height() +
heightBuffer);

$("\#embed-container").height(newHeight);

$(window).resize(function () {

var newHeight = $(window).height() - ($("header").height() +
heightBuffer);

$("\#embed-container").height(newHeight);

});

222. Your code in **embed.js** should match the following screenshot.

<img src="images\media\image85.png" style="width:3.24233in;height:2.77603in" />

Remember you can copy and paste all the code at once by using the text
in **Exercise 4 - embed.js.txt** in the **StudentLabFiles** folder.

223. Save your changes and close **embed.js**.

224. Run the web application in the Visual Studio Code debugger to test
     your work on the **Embed** page.

225. Start the Visual Studio Code debugger by selecting **Run &gt; Start
     Debugging** or by pressing the **{F5}** keyboard shortcut.

226. The **AppOwnsData** web application should display the home page as
     shown to an anonymous user.

227. Click on the **Embed** link in the top nav menu to navigate to the
     **Embed** page and login when prompted.

228. You should now be able to navigate to the **Embed** page and see
     the Power BI report displayed on the page.

<img src="images\media\image86.png" style="width:4.87917in;height:3.01469in" />

229. Try resizing the browser window. The embedded report should
     continually adapt to the size of the window.

<img src="images\media\image87.png" style="width:4.73684in;height:3.43352in" />

230. You're done testing. Close the browser, return to Visual Studio
     Code and stop the debug session using the debug toolbar.

You have now reached an important milestone. You can now tell all your
peers that you have embedded a Power BI report. However, there is more
that you can do to improve the developer experience for writing
client-side code against the Power BI JavaScript API. In the next
exercise, you will add support to your project so that you can program
client-side code using TypeScript instead of JavaScript. By moving to
TypeScript you can benefit from strongly-typed programming, compile-time
type checking and much better IntelliSense.

### Exercise 5: Add TypeScript Support to a .NET 5 Project

In this exercise, you will add support for developing your client-side
code with TypeScript instead of JavaScript. It is assumed that you have
already installed [Node.js](https://nodejs.org/en/download/) so that the
Node Package Manager command-line tool (**npm)** is available at the
commend line. You will begin by adding several Node.js configuration
files to the root folder of the **AppOwnsData** project. After that you
will restore a set of Node.js packages and use the webpack utility to
compile TypeScript code into an output file named **embed.js**.

1.  Copy three essential node.js development configuration files into
    the root folder of the **AppOwnsData** project.

    1.  Locate these three files in the **StudentLabFiles** folder.

        1.  **package.json** – the standard project file for all Node.js
            projects.

        2.  **tsconfig.json** – a configuration file used by the
            TypeScript compiler (TSC).

        3.  **webpack.config.js** – a configuration file used by the
            webpack utility.

    2.  Copy **package.json**, **tsconfig.json** and
        **webpack.config.js** into the root folder of the
        **AppOwnsData** project.

<img src="images\media\image88.png" style="width:2.08344in;height:1.92202in" />

Visual Studio Code makes it difficult to add existing files to a project
folder. You can use the Windows Explorer to copy these three files from
the **StudentLabFiles** folder to the **AppOwnsData** project folder.

2.  Restore the Node.js packages which are referenced in
    **package.json**.

    1.  Open **package.json** and review the Node.js packages referenced
        in **devDependencies** section.

<img src="images\media\image89.png" style="width:4.48822in;height:2.49345in" />

231. Open the Visual Studio Code terminal by clicking the **View &gt;
     Terminal** menu command or by using **Ctrl+\`** keyboard shortcut.

232. Run the **npm install** command to restore the list of Node.js
     packages.

<img src="images\media\image90.png" style="width:2.56812in;height:0.4574in" />

233. When you run the **npm install** command, **npm** will download all
     the Node.js packages into the **node\_modules** folder.

<img src="images\media\image91.png" style="width:6.48263in;height:2.16488in" />

234. Take a quick look at the **tsconfig.json** file.

235. Open the **tsconfig.json** file in an editor window and examine the
     TypeScript compiler settings inside.

236. When you are done, close **tsconfig.json** without saving any
     changes.

237. Take a quick look at the **webpack.config.js** file.

238. Open the **webpack.config.js** file in an editor window and examine
     its content.

const path = require('path');

module.exports = {

entry: './Scripts/embed.ts',

output: {

filename: 'embed.js',

path: path.resolve(\_\_dirname, 'wwwroot/js'),

},

resolve: {

extensions: \['.js', '.ts'\]

},

module: {

rules: \[

{ test: /\\.(ts)$/, loader: 'awesome-typescript-loader' }

\],

},

mode: "development",

devtool: 'source-map'

};

Note the **entry** property of **model.exports** object is set to
**./Scripts/embed.ts**. The **path** and **filename** of the **output**
object combine to a file path of **wwwroot/js/embed.js**. When the
webpack utility runs, it will look for a file named **embed.ts** in the
**Scripts** folder as its main entry point for the TypeScript compiler
(tsc.exe) and produce an output file in named **embed.js** in the
**wwwroot/js** folder.

239. When you are done, close **webpack.config.js** without saving any
     changes.

240. Add a new TypeScript source file named **embed.ts**.

241. In the **AppOwnsData** project folder, create a new top-level
     folder named **Scripts**.

242. Create a new file inside the **Scripts** folder named **embed.ts**.

<img src="images\media\image92.png" style="width:2.11656in;height:0.92472in" />

243. In Windows Explorer, locate the **Exercise 5 - embed.ts.txt** file
     in the **StudentLabFiles** folder.

244. Open **Exercise 5 - embed.ts.txt** in a text editor such as Notepad
     and copy all its contents to the Windows clipboard.

245. Return to Visual Studio Code and paste the contents of the Windows
     clipboard into **Embed.ts.**

<img src="images\media\image93.png" style="width:4.36488in;height:1.60201in" />

246. Save your changes and close **embed.ts**.

247. Use the webpack utility to compile **embed.ts** into **embed.js**.

248. Locate the original **embed.js** file in the **wwwroot/js** folder
     and delete it.

249. Open the Visual Studio Code terminal by clicking the **View &gt;
     Terminal** menu command or by using **Ctrl+\`** keyboard shortcut.

250. Run the **npm run build** command to run the webpack utility.

251. When you run **npm run build**, webpack should automatically
     generate a new version of **embed.js** in the **wwwroot/js**
     folder.

<img src="images\media\image94.png" style="width:3.00774in;height:1.80687in" />

252. Open the new version of **embed.js**. You should see it is a source
     file generated by the webpack utility.

<img src="images\media\image95.png" style="width:3.97263in;height:1.03594in" />

253. Close **embed.js** without saving any changes.

254. Update **AppOwnsData.csproj** to add the **npm run build** command
     as part of the MSBuild build process.

255. Open the project file named **AppOwnsData.csproj** in an editor
     window.

<img src="images\media\image96.png" style="width:4.30774in;height:1.34839in" />

256. Add a new **Target** element named **PostBuild** to run the **npm
     run build** command as shown in the following code listing.

&lt;Project Sdk="Microsoft.NET.Sdk.Web"&gt;

&lt;PropertyGroup&gt;

&lt;TargetFramework&gt;net5.0&lt;/TargetFramework&gt;

&lt;UserSecretsId&gt;aspnet-AppOwnsData-74548193-D24B-4C99-9C55-AF543E13A630&lt;/UserSecretsId&gt;

&lt;/PropertyGroup&gt;

&lt;ItemGroup&gt;

&lt;PackageReference
Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="5.0.1"
NoWarn="NU1605" /&gt;

&lt;PackageReference
Include="Microsoft.AspNetCore.Authentication.OpenIdConnect"
Version="5.0.1" NoWarn="NU1605" /&gt;

&lt;PackageReference Include="Microsoft.Identity.Web" Version="1.1.0"
/&gt;

&lt;PackageReference Include="Microsoft.Identity.Web.UI" Version="1.1.0"
/&gt;

&lt;PackageReference Include="Microsoft.PowerBi.Api" Version="3.20.1"
/&gt;

&lt;/ItemGroup&gt;

&lt;Target Name="PostBuild" AfterTargets="PostBuildEvent"&gt;

&lt;Exec Command="npm run build" /&gt;

&lt;/Target&gt;

&lt;/Project&gt;

257. Save your changes and close **AppOwnsData.csproj**.

258. Return to the terminal and run the **dotnet build** command.

<img src="images\media\image97.png" style="width:1.88714in;height:0.37452in" />

259. When you run the **dotnet build** command, the output window should
     show you that the webpack command is running.

<img src="images\media\image98.png" style="width:1.76488in;height:1.72419in" />

Now whenever you start a debug session with the **{F5}** key, the
TypeScript in **embed.ts** will be automatically compiled into
**embed.js**.

260. Run the web application in the Visual Studio Code debugger to test
     your work on the **Embed** page.

261. Start the Visual Studio Code debugger by selecting **Run &gt; Start
     Debugging** or by pressing the **{F5}** keyboard shortcut.

262. Click on the **Embed** link in the top nav menu to navigate to the
     **Embed** page and login when prompted to view the report.

<img src="images\media\image99.png" style="width:5.02614in;height:1.21801in" />

263. You're done testing. Close the browser, return to Visual Studio
     Code and stop the debug session using the debug toolbar.

When you test the **AppOwnsData** web application, it should behave just
as it did when you tested it in Exercise 4. The difference is that now
the client-side behavior is now implemented with TypeScript instead of
JavaScript.

### Exercise 6: Create a View Model for App Workspaces

Up to this point, you have implemented the **AppOwnsData** project to
embed a single report by hard-coding the IDs of that report and its
hosting workspace. In this exercise, you will remove the hard-coded IDs
and extend the **Embed** page of the **AppOwnsData** project to
dynamically discover what workspaces and reports are available to the
current user.

1.  Extend the **PowerBiServiceApi** class with a new method named
    **GetEmbeddedViewModel**.

    1.  Locate the **PowerBiServiceApi.cs** in the **Service** folder
        and open it in an editor window.

    2.  Add the following method named **GetEmbeddedViewModel** to the
        end of **PowerBiServiceApi** class.

public async Task&lt;string&gt; GetEmbeddedViewModel(string
appWorkspaceId) {

PowerBIClient pbiClient = GetPowerBiClient();

Object viewModel;

Guid workspaceId = new Guid(appWorkspaceId);

var workspaces = (await pbiClient.Groups.GetGroupsAsync()).Value;

var currentWorkspace = workspaces.First((workspace) =&gt; workspace.Id
== workspaceId);

var datasets = (await
pbiClient.Datasets.GetDatasetsInGroupAsync(workspaceId)).Value;

var reports = (await
pbiClient.Reports.GetReportsInGroupAsync(workspaceId)).Value;

IList&lt;GenerateTokenRequestV2Dataset&gt; datasetRequests = new
List&lt;GenerateTokenRequestV2Dataset&gt;();

foreach (var dataset in datasets) {

datasetRequests.Add(new GenerateTokenRequestV2Dataset(dataset.Id));

};

IList&lt;GenerateTokenRequestV2Report&gt; reportRequests = new
List&lt;GenerateTokenRequestV2Report&gt;();

foreach (var report in reports) {

reportRequests.Add(new GenerateTokenRequestV2Report(report.Id,
allowEdit: true));

};

IList&lt;GenerateTokenRequestV2TargetWorkspace&gt; workspaceRequests =

new GenerateTokenRequestV2TargetWorkspace\[\] {

new GenerateTokenRequestV2TargetWorkspace(workspaceId)

};

GenerateTokenRequestV2 tokenRequest =

new GenerateTokenRequestV2(datasets: datasetRequests,

reports: reportRequests,

targetWorkspaces: workspaceRequests);

// call to Power BI Service API and pass GenerateTokenRequest object to
generate embed token

string embedToken =
pbiClient.EmbedToken.GenerateToken(tokenRequest).Token;

viewModel = new

{

workspaces = workspaces,

currentWorkspace = currentWorkspace.Name,

datasets = datasets,

reports = reports,

token = embedToken

};

return JsonConvert.SerializeObject(viewModel);

}

The **GetEmbeddedViewModel** method accepts an **appWorkspaceId**
parameter and returns a string value with JSON-formatted data. If the
**appWorkspaceId** parameter contains a GUID, the
**GetEmbeddedViewModel** method returns a view model for the app
workspace associated with that GUID.

You can copy and paste this method from the **Exercise 6 -
PowerBiServiceApi.cs.txt** file in the **StudentLabFiles** folder.

3.  Enhance your conceptual understanding of the data involved by
    examining the JSON returned by **GetEmbeddedViewModel**.

<img src="images\media\image100.png" style="width:5.4386in;height:1.95638in" />

4.  Underneath the **GetEmbeddedViewModel** method, add another method
    named

public async Task&lt;Group&gt; GetFirstWorkspace() {

PowerBIClient pbiClient = this.GetPowerBiClient();

var workspaces = (await pbiClient.Groups.GetGroupsAsync()).Value;

if (workspaces.Count &gt; 0) {

return workspaces.First();

}

Else {

return null;

}

}

5.  Save your work and close **PowerBiServiceApi.cs**.

<!-- -->

2.  Modify **Embed** method in **HomeController** to call the
    **GetEmbeddedViewModel** method.

    1.  Locate the **HomeController.cs** file and open it in an editor
        window.

    2.  Locate the **Embed** method which should currently match this
        **Embed** method implementation.

    3.  Delete the **Embed** method implementation and replace it the
        following code.

public async Task&lt;IActionResult&gt; Embed(string workspaceId) {

try {

Guid guidTest = new Guid(workspaceId);

var viewModel = await
this.powerBiServiceApi.GetEmbeddedViewModel(workspaceId);

return View(viewModel as object);

}

Catch {

var firstWorkspace = await this.powerBiServiceApi.GetFirstWorkspace();

if (firstWorkspace == null) {

return RedirectToPage("/Error");

}

Else {

return RedirectToPage("/Embed", null, new { workspaceId =
firstWorkspace.Id });

}

}

}

264. Save your work and close **HomeController.cs**.

There are a few things to note about the new implementation of the
**Embed** controller action method. First, the method now takes a string
parameter named **workspaceId**. When this controller method is passed a
workspace ID in the **workspaceId** query string parameter, it passes
that workspace ID along to the **PowerBiServiceApi** class when it calls
the **GetEmbeddedViewModel** method.

The second thing to note about this example if that the string-based
**viewModel** variable is cast to a type of **object** in the **return**
statement using the syntax **View(viewModel as object)**. This is a
required workaround because passing a string parameter to **View()**
would fail because the string value would be treated as a view name
instead of a view model being passed to the underlying view.

265. Replace the code in **Embed.cshtml** with a better implementation.

266. Locate **Embed.cshtml** file in the **Views &gt; Home** folder,
     open it in an editor window and delete all the content inside.

267. In Windows Explorer, locate the **Exercise 6 - Embed.cshtml.txt**
     file in the **StudentLabFiles** folder.

268. Open **Exercise 6 - Embed.cshtml.txt** in a text editor such as
     Notepad and copy all its contents to the Windows clipboard.

269. Return to Visual Studio Code and paste the contents of to the
     Windows clipboard into **Embed.cshtml.**

<img src="images\media\image101.png" style="width:4.37293in;height:1.42654in" />

270. Save your changes and close **Embed.cshtml.**

271. Replace the code in **Embed.ts** with a better implementation.

272. Locate **Embed.ts** file in the **Scripts** folder, open it in an
     editor window and delete all the content inside.

273. In Windows Explorer, locate the **Exercise 6 - Embed.ts.txt** file
     in the **StudentLabFiles** folder.

274. Open **Exercise 6 - Embed.ts.txt** in a text editor such as Notepad
     and copy all its contents to the Windows clipboard.

275. Return to Visual Studio Code and paste the content of **Exercise
     6 - Embed.ts.txt** into **Embed.ts.**

<img src="images\media\image102.png" style="width:4.88338in;height:1.57566in" />

276. Save your changes and close **Embed.ts.**

Revieing the client-side code in **Embed.ts** has been left as an
exercise for the reader, After you run and test the **AppOwnsData**
application at the end of this exercise and you have experienced the
user interface from the user perspective, you might want to examine the
code in **Embed.ts** to see how this application implements the
functionality to navigate between workspace and to discover the reports
and datasets in each workspace.

277. Run the web application in the .NET debugger to test your work on
     the **Embed** page.

278. Start the Visual Studio Code debugger by selecting **Run &gt; Start
     Debugging** or by pressing the **{F5}** keyboard shortcut.

279. Click on the **Embed** link in the top nav menu to navigate to the
     **Embed** page and login when prompted.

280. The **Embed** page should appear much differently than before as
     shown in the following screenshot.

<img src="images\media\image103.png" style="width:6.40022in;height:1.35116in" />

Note there is a dropdown list for the **Current Workspace** that you can
use to navigate across workspaces.

281. Return to the Power BI in the browser and add the App-Owns-Data
     Sample App as an admin on a few other workspaces.

282. Return back to the **AppOwnsData** app and refresh the page.

283. Use the **Current Workspace** dropdown menu to navigate to the
     workspace you created earlier in this lab.

<img src="images\media\image104.png" style="width:3.94595in;height:1.29825in" />

284. Click on a report in the **Open Report** section.

<img src="images\media\image105.png" style="width:4.85965in;height:1.68446in" />

285. The report should open in read-only mode.

<img src="images\media\image106.png" style="width:5.87573in;height:2.27741in" />

286. Click the **Toggle Edit Mode** button to move the report into edit
     mode.

<img src="images\media\image107.png" style="width:5.95657in;height:1.65461in" />

287. Note that when the report goes into edit mode, there isn't much
     space to work on the report while editing.

<img src="images\media\image108.png" style="width:4.20724in;height:1.95364in" />

288. Click the **Full Screen** button to enter full screen mode

<img src="images\media\image109.png" style="width:3.64823in;height:0.94408in" />

You can invoke the **File &gt; Save** command in a report that is in
edit mode to save your changes.

289. Press the **Esc** key in the keyboard to exit full screen mode.

290. Click on a second report in the **Open Report** section to navigate
     between reports.

<img src="images\media\image110.png" style="width:5.47039in;height:2.1203in" />

291. Create a new report by clicking on a dataset name in the **New
     Report** section.

<img src="images\media\image111.png" style="width:4.83882in;height:1.72047in" />

292. Add a simple visual of any type to the new report.

<img src="images\media\image112.png" style="width:5.79825in;height:2.0875in" />

293. Save the new report using the **File &gt; Save as** menu command.

<img src="images\media\image113.png" style="width:3.49123in;height:1.52351in" />

294. Give your new report a name.

<img src="images\media\image114.png" style="width:2.78838in;height:0.78107in" />

295. After you click save, the new report should show up in the Open
     Report section and be displayed in read-only mode.

<img src="images\media\image115.png" style="width:5.11404in;height:1.59103in" />

296. When you're done testing, close the browser, return to Visual
     Studio Code and stop the debug session.

Congratulations. You have now completed this lab and have gained
cutting-edge experience developing with Power BI embedding
