param(
    [Parameter(Mandatory=$true)]
    [string]$InputFile
)

$OutputFile = [System.IO.Path]::ChangeExtension($InputFile, ".typ")
$TempFile = "$OutputFile.tmp"

# Vérification des fichiers
if (!(Test-Path $InputFile)) {
    Write-Host "Erreur : fichier Markdown introuvable : $InputFile"
    exit 1
}

if (!(Test-Path "./Form/begin.typ")) {
    Write-Host "Erreur : fichier begin.typ introuvable"
    exit 1
}

if (!(Test-Path "./Form/end.typ")) {
    Write-Host "Erreur : fichier end.typ introuvable"
    exit 1
}

# Conversion Markdown -> Typst avec Docker + Pandoc
docker run --rm `
    -v "${PWD}:/data" `
    -w /data `
    pandoc/core `
    "$InputFile" `
    -t typst `
    --lua-filter=filter/figure-kind.lua `
    -o "$TempFile"

# Vérification conversion
if (!(Test-Path $TempFile)) {
    Write-Host "Erreur : la conversion Pandoc a échoué"
    exit 1
}

# Ajout de begin.typ au début et end.typ à la fin
$Header = Get-Content "./Form/begin.typ" -Raw
$Content = Get-Content $TempFile -Raw
$Footer = Get-Content "./Form/end.typ" -Raw

Set-Content $OutputFile ($Header + "`r`n" + $Content + "`r`n" + $Footer)

# Nettoyage
Remove-Item $TempFile

Write-Host "Fichier généré : $OutputFile"