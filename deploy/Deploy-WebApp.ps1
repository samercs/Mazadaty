$profile = "Mzayad";
$username = "`$Mzayad";
$password = "LnaeiuhQS43rGuFYCcLpleAih1xLxvqh9dWJBChvkud9jNd4GLiSkdnn2AcN";
$url = "https://mzayad.orangejetpack.com";

function Deploy-WebApp
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [String]
        $Profile,

        [Parameter(Mandatory = $true)]
        [String]
        $Configuration,

        [Parameter(Mandatory = $true)]
        [String]
        $Username,

        [Parameter(Mandatory = $true)]
        [String]
        $Password 
    )
    
    $currDirectory = Resolve-Path .\ 
    $webProjectDirectory = Get-ChildItem -Path (Resolve-Path ..\.\src) | ? { $_.Name.EndsWith(".Web") };
    $webProjectFile = Get-ChildItem -Path $webProjectDirectory.FullName | ? {$_.Extension -eq ".csproj"} | Select-Object -First 1

    Remove-Module Invoke-MsBuild -ErrorAction SilentlyContinue; 
    Import-Module ($currDirectory.Path + '\Invoke-MsBuild.psm1') -Scope Local -Verbose:$false
    
    $params = "/property:Configuration=Release;PublishProfile={0};UserName={1};Password={2};DeployOnBuild=true;WarningLevel=1;AllowUntrustedCertificate=true;VisualStudioVersion=14.0 /verbosity:m" -f $profile, $username, $password
    $buildSucceeded = Invoke-MsBuild -Path $webProjectFile.FullName -ShowBuildWindowAndPromptForInputBeforeClosing -MsBuildParameters $params

    if ($buildSucceeded)
    { 
        $url = $url + "/ok?" + (Get-Date).Ticks;
        
        Write-Host "Build completed successfully, warming $url..." 

        $webClient = new-object System.Net.WebClient
        $webClient.Headers.Add("user-agent", "PowerShell Script")
        $webClient.DownloadString($url);

        Write-Host "Deploy completed.";
    }
    else
    { 
        Write-Host "Build failed. Check the build log file for errors." 
    }
}

Deploy-WebApp -Profile $profile -Configuration "Release" -Username $username -Password $password;


