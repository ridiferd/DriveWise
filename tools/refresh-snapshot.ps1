# Replaces data/vehicles-snapshot.json with LIVE records from fueleconomy.gov.
# Usage (PowerShell 7):  ./tools/refresh-snapshot.ps1 -Year 2025
param([int]$Year = 2025)

$base = "https://www.fueleconomy.gov/ws/rest"
$h = @{ Accept = "application/json" }
$targets = @(
  @{ make = "Toyota";    model = "Prius" },
  @{ make = "Toyota";    model = "Camry" },
  @{ make = "Toyota";    model = "RAV4 Hybrid AWD" },
  @{ make = "Honda";     model = "Civic 4Dr" },
  @{ make = "Honda";     model = "CR-V AWD" },
  @{ make = "Mazda";     model = "CX-5 4WD" },
  @{ make = "Ford";      model = "F150 Pickup 2WD" },
  @{ make = "Kia";       model = "Niro FE" },
  @{ make = "Tesla";     model = "Model 3 RWD" },
  @{ make = "Hyundai";   model = "Ioniq 5 RWD" }
)

$vehicles = @()
foreach ($t in $targets) {
  $url = "$base/vehicle/menu/options?year=$Year&make=$([uri]::EscapeDataString($t.make))&model=$([uri]::EscapeDataString($t.model))"
  try {
    $opts = (Invoke-RestMethod -Uri $url -Headers $h).menuItem
    $first = @($opts)[0]
    if ($null -eq $first) { Write-Warning "No options for $($t.make) $($t.model)"; continue }
    $v = Invoke-RestMethod -Uri "$base/vehicle/$($first.value)" -Headers $h
    $vehicles += $v
    Write-Host "OK  $Year $($t.make) $($t.model) -> id $($first.value)"
  } catch { Write-Warning "Skip $($t.make) $($t.model): $_" }
}

$out = [ordered]@{ _note = "Live snapshot from fueleconomy.gov on $(Get-Date -Format s)"; vehicle = $vehicles }
$out | ConvertTo-Json -Depth 5 | Set-Content -Path (Join-Path $PSScriptRoot "../data/vehicles-snapshot.json") -Encoding utf8
Write-Host "Saved $($vehicles.Count) vehicles."
