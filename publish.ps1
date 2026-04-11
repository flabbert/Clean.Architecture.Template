param(
    [Parameter(Mandatory)]
    [string]$ApiKey,

    [string]$Version,

    [ValidateSet('major', 'minor', 'patch')]
    [string]$Bump = 'patch'
)

$ErrorActionPreference = 'Stop'

$csproj = 'Flabbert.Clean.Template.csproj'
[xml]$xml = Get-Content $csproj

if (-not $Version) {
    $current = [version](Select-Xml -Xml $xml -XPath '//PackageVersion').Node.InnerText
    switch ($Bump) {
        'major' { $Version = "$($current.Major + 1).0.0" }
        'minor' { $Version = "$($current.Major).$($current.Minor + 1).0" }
        'patch' {
            $p = if ($current.Build -ge 0) { $current.Build + 1 } else { 1 }
            $Version = "$($current.Major).$($current.Minor).$p"
        }
    }
}

Write-Host "Version: $Version"
(Select-Xml -Xml $xml -XPath '//PackageVersion').Node.InnerText = $Version
$xml.Save((Resolve-Path $csproj))

dotnet pack -c Release /p:PackageVersion=$Version

$nupkg = Get-ChildItem -Path "bin\Release\*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1

if (-not $nupkg) {
    Write-Error "No .nupkg file found in bin\Release\"
    exit 1
}

Write-Host "Pushing $($nupkg.Name)..."
dotnet nuget push $nupkg.FullName --api-key $ApiKey --source https://api.nuget.org/v3/index.json
