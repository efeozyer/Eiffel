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
    foreach ($subDir in Get-ChildItem -Path "$($rootDir.FullName)\src\**\") 
    {
        $files = (& git diff $prevSHA $commitSHA --name-only $subDir.FullName "$($subDir.FullName)")

        if ($files.Count -gt 0) 
        {
            $csprojFile = "$($subDir.FullName)\$($subDir.Name).csproj"

            Write-Host $csprojFile
            
            Write-Host "Build started $($subDir.FullName)\$($subDir.Name)"
            
            & dotnet pack $csprojFile --output .\\packages --no-restore --no-build
            
            if (-Not($?)) 
            {
                throw "Package failed!"
            }
        }
    }
}