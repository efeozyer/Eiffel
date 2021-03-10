param (
    [Parameter(Mandatory = $true)]
    [string]
    $commitSHA,
    [Parameter(Mandatory = $true)]
    [string]
    $prevSHA
)

foreach ($rootDir in Get-ChildItem -Directory -Path . -Filter 'Eiffel.*')
{
    foreach ($subDir in Get-ChildItem -Path "$($rootDir.Name)\src\**\") 
    {
        $files = (& git diff $prevSHA $commitSHA --name-only $subDir.FullName "$($subDir.FullName)")
        if ($files.Count -gt 0) 
        {
            $csprojFile = "$($subDir.FullName)\$($subDir.Name).csproj"
            Write-Host "Restore started $($subDir.FullName)\$($subDir.Name)"
            & dotnet restore $csprojFile
            if (-Not($?)) 
            {
                throw "Restore failed!"
            }
        }
    }
}