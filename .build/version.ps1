param (
    [Parameter(Mandatory = $true)]
    [string]
    $commitSHA,
    [Parameter(Mandatory = $true)]
    [string]
    $prevSHA
)

& git config user.name efeozyer
& git config user.email efeozyer@yandex.com

foreach ($rootDir in Get-ChildItem -Directory -Path . -Filter 'Eiffel.*')
{
    foreach ($subDir in Get-ChildItem -Path "$($rootDir.Name)\src\**\") 
    {
        $files = (& git diff $prevSHA $commitSHA --name-only $subDir.FullName "$($subDir.FullName)")
        if ($files.Count -gt 0) 
        {
            $csprojFile = "$($subDir.FullName)\$($subDir.Name).csproj"
            $csprojXml = [Xml] (Get-Content $csprojFile)
            $currentVersion = [version] $csprojXml.Project.PropertyGroup.Version
            if ([string]::IsNullOrWhiteSpace($csprojXml.Project.PropertyGroup.Version)) 
            {
                $newVersion = [version]::new(1, 0, 0, 0);
                $versionNode = $csprojXml.CreateElement("Version")
                $versionNode.InnerText = $newVersion
                $csprojXml.SelectSingleNode("//Project//PropertyGroup").AppendChild($versionNode) | Out-Null
            }
            else 
            {
                $newVersion = [version]::new($currentVersion.Major, $currentVersion.Minor, $currentVersion.Build + 1, 0)
                $csprojXml.Project.PropertyGroup.Version = [string]$newVersion
            }
            
            $csprojXml.Save($csprojFile)
            if (-Not($?)) 
            {
                throw "Package version upgrade failed!"
            }
            
            & git add .
            
            & git commit -m "Package version upgraded"

            & git tag -a  "$($subDir.Name).v$($newVersion)" -m "New release for $($subDir.Name)"

            & git push --tags
            
            if (-Not($?)) 
            {
                throw "Package version upgrade failed!"
            }
        }
    }
}

