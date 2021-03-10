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
    $files = (& git diff $prevSHA $commitSHA --name-only $rootDir.FullName "$($rootDir.FullName)")
    if ($files.Count -gt 0) 
    {
        Write-Host $rootDir.FullName
        Write-Host "Tests runing $($rootDir.Name)"
        dotnet test $rootDir.FullName
        if (-Not($?)) 
        {
            throw "Tests failed!"
        }
    }
}