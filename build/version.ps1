param (
    [Parameter(Mandatory = $true)]
    [string]
    $commitSHA,
    [Parameter(Mandatory = $true)]
    [string]
    $mainSHA,
    [Parameter(Mandatory = $true)]
    [boolean]
    $isRelease
)

foreach ($rootDir in Get-ChildItem -Directory -Path . -Filter 'Eiffel.*')
{
    foreach ($subDir in Get-ChildItem -Path "$($rootDir.Name)\src\**\") 
    {
        $files = (& git diff $mainSHA $commitSHA --name-only $subDir.FullName "$($rootDir.Name).*")
        if ($files.Count -gt 0) 
        {
            $csprojFile = "$($subDir.FullName)\$($subDir.Name).csproj"
            $csprojXml = [Xml] (Get-Content $csprojFile)
            $currentVersion = [version] $csprojXml.Project.PropertyGroup.Version
            if ([string]::IsNullOrWhiteSpace($csprojXml.Project.PropertyGroup.Version)) 
            {
                if ($isRelease) 
                {
                    $newVersion = [version]::new($currentVersion.Major + 1, $currentVersion.Minor + 1, $currentVersion.Build)
                }
                Else 
                {
                    $newVersion = [version]::new($currentVersion.Major + 1, $currentVersion.Minor, $currentVersion.Build + 1)
                }
                $versionNode = $csprojXml.CreateElement("Version")
                $versionNode.InnerText = $newVersion
                $csprojXml.SelectSingleNode("//Project//PropertyGroup").AppendChild($versionNode) | Out-Null
            }
            ElseIf ($isRelease) 
            {
                $newVersion = [version]::new($currentVersion.Major, $currentVersion.Minor + 1, $currentVersion.Build)
                $csprojXml.Project.PropertyGroup.Version = [string]$newVersion
            } 
            Else 
            {
                $newVersion = [version]::new($currentVersion.Major, $currentVersion.Minor, $currentVersion.Build + 1)
                $csprojXml.Project.PropertyGroup.Version = [string]$newVersion
            }
            
            $csprojXml.Save($csprojFile)
            if (-Not($?)) 
            {
                throw "Version upgrade failed!"
            }
        }
    }
}

& git config user.name efeozyer
& git config user.email efeozyer@yandex.com
& git add .
& git commit -m "Package version upgraded"
& git push

if (-Not($?)) 
{
    throw "Package version upgrade failed!"
}