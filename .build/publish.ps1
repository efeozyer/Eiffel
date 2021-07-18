param (
    [Parameter(Mandatory = $true)]
    [string]
    $apiKey,
    
    [Parameter(Mandatory = $true)]
    [string]
    $source
)

foreach ($item in Get-ChildItem -File -Path .\\packages -Filter '*.nupkg')
{ 
    & dotnet nuget push $item.FullName -k "$apiKey" -s "$source" --skip-duplicate
 
    if (-Not($?)) 
    {
         throw "Publish failed!"
    }
}