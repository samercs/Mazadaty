$o = @{}
$o.DeployUtc = Get-Date -format u;
$o.ShortHash = git.exe log -1 --pretty=format:%h;
$o.FullHash = git.exe log -1 --pretty=format:%H;
$o.Subject = git.exe log -1 --pretty=format:%s;
$o.Name = git.exe log -1 --pretty=format:%cn;
$o.Email = git.exe log -1 --pretty=format:%ce;

$o | ConvertTo-Json | Out-File "D:\home\site\wwwroot\version.json";
