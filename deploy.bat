set solution=Mzayad.sln
set profile=Mzayad
set username=$Mzayad
set password=LnaeiuhQS43rGuFYCcLpleAih1xLxvqh9dWJBChvkud9jNd4GLiSkdnn2AcN
set configuration=Release

%windir%\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe %solution% /property:Configuration=%configuration%;DeployOnBuild=true;PublishProfile=%profile%;WarningLevel=1;UserName=%username%;Password=%password%;AllowUntrustedCertificate=true /verbosity:m