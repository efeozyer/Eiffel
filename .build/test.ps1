param (
    [Parameter(Mandatory = $true)]
    [string]
    $commitSHA,
    [Parameter(Mandatory = $true)]
    [string]
    $mainSHA
)

foreach ($rootDir in Get-ChildItem -Directory -Path . -Filter 'Eiffel.*')
{
    foreach ($subDir in Get-ChildItem -Path "$($rootDir.Name)\src\**\") 
    {
        $files = (& git diff $mainSHA $commitSHA --name-only $subDir.FullName "$($rootDir.Name).*")
        if ($files.Count -gt 0) 
        {
            Write-Host "Tests runing $($subDir.Name)"
            dotnet test $rootDir.FullName
            if (-Not($?)) 
            {
                throw "Tests failed!"
            }
        }
    }
}