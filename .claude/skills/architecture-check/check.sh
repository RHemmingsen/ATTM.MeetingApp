#!/usr/bin/env bash
# Mechanical checks for the Clean Architecture + CQRS conventions in CLAUDE.md.
# Usage: check.sh [solution-root]   (defaults to the current directory)
# Exit code 1 if any rule fails. Judgement-based rules are left to the reviewer (see SKILL.md).
set -uo pipefail

ROOT="${1:-.}"
cd "$ROOT" || exit 2

FAILS=0
PASSES=0

proj_dir() { find . -name "*.$1.csproj" -not -path '*/bin/*' -not -path '*/obj/*' -printf '%h\n' | head -1; }
cs_files() { [ -n "$1" ] && find "$1" -name '*.cs' -not -path '*/bin/*' -not -path '*/obj/*' -not -path '*/Migrations/*'; }

DOMAIN=$(proj_dir Domain)
APP=$(proj_dir Application)
PERSIST=$(proj_dir Persistence)
API=$(proj_dir API)
TESTS=$(proj_dir UnitTests)

if [ -z "$DOMAIN" ] || [ -z "$APP" ] || [ -z "$API" ]; then
  echo "Could not find *.Domain, *.Application and *.API projects under $(pwd)."
  exit 2
fi

# rule <id> <description> <violations (newline-separated, empty = pass)>
rule() {
  if [ -z "$3" ]; then
    PASSES=$((PASSES + 1)); printf 'PASS  %-4s %s\n' "$1" "$2"
  else
    FAILS=$((FAILS + 1)); printf 'FAIL  %-4s %s\n' "$1" "$2"
    printf '%s\n' "$3" | sed 's/^/        /'
  fi
}

grep_cs() { # grep_cs <dir> <regex> -> file:line:text
  local files; files=$(cs_files "$1"); [ -z "$files" ] && return 0
  echo "$files" | xargs grep -HnE "$2" 2>/dev/null | grep -vE "^[^:]+:[0-9]+:\s*//" || true
}

refs() { grep -oE 'ProjectReference Include="[^"]+"' "$1"/*.csproj | sed -E 's/.*[\\/]([^\\/]+)\.csproj"/\1/'; }

# --- Dependency rule ---------------------------------------------------------
rule D1 "Domain references no projects or packages" \
  "$(grep -nE '<(ProjectReference|PackageReference)' "$DOMAIN"/*.csproj)"

rule D2 "Application references only Domain" \
  "$(refs "$APP" | grep -vE '\.Domain$')"

rule D3 "Application has no EF Core / ASP.NET Core / Azure packages" \
  "$(grep -nE 'PackageReference Include="(Microsoft\.EntityFrameworkCore|Microsoft\.AspNetCore|Azure\.)' "$APP"/*.csproj)"

rule D4 "Application code has no EF Core / ASP.NET Core / Azure usings" \
  "$(grep_cs "$APP" '^using (Microsoft\.EntityFrameworkCore|Microsoft\.AspNetCore|Azure)')"

rule D5 "Domain code has no framework usings (EF, MediatR, serialization, ASP.NET)" \
  "$(grep_cs "$DOMAIN" '^using (Microsoft\.|MediatR|AutoMapper|FluentValidation|System\.Text\.Json|Newtonsoft)')"

rule D6 "Domain has no framework attributes" \
  "$(grep_cs "$DOMAIN" '\[(JsonConverter|JsonPropertyName|Table|Column|Key|Required|MaxLength)')"

[ -n "$PERSIST" ] && rule D7 "Persistence does not reference API" \
  "$(refs "$PERSIST" | grep -E '\.API$')"

[ -n "$TESTS" ] && rule D8 "UnitTests do not reference Persistence or API" \
  "$(refs "$TESTS" | grep -E '\.(Persistence|API)$')"

# --- Application -------------------------------------------------------------
rule A1 "Handlers do not construct validators (use ValidationBehavior)" \
  "$(grep_cs "$APP" 'new [A-Za-z]+Validator\(')"

rule A2 "Handlers depend on I<Entity>Repository, not IAsyncRepository<T>" \
  "$(grep_cs "$APP/Features" 'IAsyncRepository<')"

rule A3 "Assembly scanning uses typeof(...).Assembly, not AppDomain" \
  "$(grep_cs "$APP" 'AppDomain\.CurrentDomain\.GetAssemblies')"

rule A4 "A ValidationBehavior pipeline behavior exists" \
  "$(grep_cs "$APP" 'IPipelineBehavior<' | grep -q . || echo "no IPipelineBehavior implementation found in $APP")"

rule A5 "Contracts take CancellationToken on async methods" \
  "$(grep_cs "$APP/Contracts" 'Task(<[^;]*>)? +[A-Za-z]+Async\([^)]*\);' | grep -v CancellationToken)"

rule A6 "Feature files sit in Features/<Entities>/(Commands|Queries)/<UseCase>/" \
  "$(cs_files "$APP/Features" | grep -vE '/Features/[^/]+/(Commands|Queries)/[^/]+/[^/]+\.cs$')"

rule A7 "Handler/validator/request names match their folder" \
  "$(cs_files "$APP/Features" | while read -r f; do
       folder=$(basename "$(dirname "$f")"); file=$(basename "$f" .cs)
       case "$file" in *VM) continue ;; esac
       [[ "$file" == "$folder"* ]] || echo "$f (folder $folder)"
     done)"

rule A8 "Namespaces match folders" \
  "$(for f in $(cs_files "$APP") $(cs_files "$DOMAIN") $(cs_files "$PERSIST") $(cs_files "$API"); do
       ns=$(grep -m1 -oE '^namespace [A-Za-z0-9_.]+' "$f" | cut -d' ' -f2); [ -z "$ns" ] && continue
       expected=$(dirname "${f#./}" | tr '/' '.')
       [ "$ns" = "$expected" ] || echo "$f: namespace $ns, expected $expected"
     done)"

rule A9 "File name matches the type it declares" \
  "$(for f in $(cs_files "$APP") $(cs_files "$DOMAIN") $(cs_files "$PERSIST") $(cs_files "$API"); do
       t=$(grep -m1 -oE '(class|record|interface|enum|struct) [A-Za-z0-9_]+' "$f" | awk '{print $2}')
       [ -z "$t" ] && continue; [ "$t" = "Program" ] && continue
       [ "$t" = "$(basename "$f" .cs)" ] || echo "$f declares $t"
     done)"

# --- Persistence -------------------------------------------------------------
if [ -n "$PERSIST" ]; then
  rule P1 "No inline entity configuration in OnModelCreating (use IEntityTypeConfiguration)" \
    "$(grep -lE ': *DbContext\b' $(cs_files "$PERSIST") 2>/dev/null | xargs -r grep -HnE 'modelBuilder\.Entity<')"

  rule P2 "No Guid.NewGuid() in persistence code (seed data needs fixed ids)" \
    "$(grep_cs "$PERSIST" 'Guid\.NewGuid\(\)')"

  rule P3 "PendingModelChangesWarning is not suppressed" \
    "$(grep_cs "$PERSIST" 'PendingModelChangesWarning')"

  rule P4 "Persistence does not fetch secrets itself (Key Vault belongs in Program.cs configuration)" \
    "$(grep_cs "$PERSIST" '(SecretClient|DefaultAzureCredential)')"
fi

# --- API ---------------------------------------------------------------------
rule C1 "Controllers inherit ControllerBase" \
  "$(grep_cs "$API/Controllers" 'class [A-Za-z]+Controller.*: *Controller\b')"

rule C2 "Controllers do not use repositories, DbContext or AutoMapper" \
  "$(grep_cs "$API/Controllers" '(Repository|DbContext|IMapper)')"

rule C3 "Controllers have no try/catch (middleware handles errors)" \
  "$(grep_cs "$API/Controllers" '^\s*(try|catch)\b')"

rule C4 "Exception middleware references a 500 status (verify it is the default case)" \
  "$(grep_cs "$API" 'Status500InternalServerError|InternalServerError' | grep -q . || echo "no 500 mapping found in $API")"

# --- Hygiene -----------------------------------------------------------------
rule H1 "No empty <Folder Include> entries in project files" \
  "$(grep -nE '<Folder Include' $(find . -name '*.csproj' -not -path '*/bin/*' -not -path '*/obj/*'))"

echo
echo "$PASSES passed, $FAILS failed"
[ "$FAILS" -eq 0 ]
