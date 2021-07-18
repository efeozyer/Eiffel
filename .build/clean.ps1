Write-Host "Clean started"

foreach ($package in Get-ChildItem -File -Path .\\packages)
{
    Remove-Item -Path $package.FullName

    if (-Not($?)) 
    {
        throw "Clean failed!"
    }
}