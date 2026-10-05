#!/usr/bin/env bash
# Replaces data/vehicles-snapshot.json with LIVE records from fueleconomy.gov. Needs curl + jq.
set -euo pipefail
YEAR="${1:-2025}"
BASE="https://www.fueleconomy.gov/ws/rest"
OUT="$(dirname "$0")/../data/vehicles-snapshot.json"
TARGETS=("Toyota|Prius" "Toyota|Camry" "Toyota|RAV4 Hybrid AWD" "Honda|Civic 4Dr" "Honda|CR-V AWD"
         "Mazda|CX-5 4WD" "Ford|F150 Pickup 2WD" "Kia|Niro FE" "Tesla|Model 3 RWD" "Hyundai|Ioniq 5 RWD")
tmp="$(mktemp)"; echo "[]" > "$tmp"
for t in "${TARGETS[@]}"; do
  make="${t%%|*}"; model="${t##*|}"
  id=$(curl -sfG -H "Accept: application/json" "$BASE/vehicle/menu/options" \
        --data-urlencode "year=$YEAR" --data-urlencode "make=$make" --data-urlencode "model=$model" \
        | jq -r '.menuItem | if type=="array" then .[0].value else .value end' || true)
  if [[ -z "$id" || "$id" == "null" ]]; then echo "skip $make $model"; continue; fi
  curl -sf -H "Accept: application/json" "$BASE/vehicle/$id" | jq --slurpfile a "$tmp" '$a[0] + [.]' > "$tmp.new" && mv "$tmp.new" "$tmp"
  echo "OK  $YEAR $make $model -> $id"
done
jq --arg d "$(date -Iseconds)" '{_note: ("Live snapshot from fueleconomy.gov on " + $d), vehicle: .}' "$tmp" > "$OUT"
echo "Saved $(jq '.vehicle|length' "$OUT") vehicles."
