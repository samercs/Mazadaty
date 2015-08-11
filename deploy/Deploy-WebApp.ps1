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
    $rootDirectory = Resolve-Path ..\.\

    $solutionFile = Get-ChildItem -Path $rootDirectory | where {$_.extension -eq ".sln"} | Select-Object -First 1
        
    Remove-Module Invoke-MsBuild -ErrorAction SilentlyContinue
    Import-Module ($currDirectory.Path + '\Invoke-MsBuild.psm1') -Scope Local -Verbose:$false

    $params = "/property:Configuration={0};PublishProfile={1};UserName={2};Password={3};DeployOnBuild=true;WarningLevel=1;AllowUntrustedCertificate=true /verbosity:m" -f $configuration, $profile, $username, $password

    $buildSucceeded = Invoke-MsBuild -Path $solutionFile.FullName -ShowBuildWindowAndPromptForInputBeforeClosing -MsBuildParameters $params

    if ($buildSucceeded)
    { 
        Write-Host "Build completed successfully." 

        #$webClient = new-object System.Net.WebClient
        #$webClient.Headers.Add("user-agent", "PowerShell Script")
        #$webClient.DownloadString($url + "/ok?" + (Get-Date).Ticks);
    }
    else
    { 
        Write-Host "Build failed. Check the build log file for errors." 
    }
}

Deploy-WebApp -Profile $profile -Configuration "Release" -Username $username -Password $password;


