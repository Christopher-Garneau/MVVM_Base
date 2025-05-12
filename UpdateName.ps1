param(
    [Parameter(Mandatory=$true)]
    [string]$NewWord
)

# Expression régulière pour le contenu des fichiers
$pattern = '(Challenge([\\_-]{1,2}))Base'

# Expression régulière pour les noms de fichiers/dossiers (Windows: \ ou /)
$namePattern = 'Challenge([\\\/_-]{1,2})Base'

# 1. Renommer les dossiers (du plus profond au plus haut)
Get-ChildItem -Recurse -Directory | Sort-Object { $_.FullName.Length } -Descending | ForEach-Object {
    if ($_ -match $namePattern) {
        $newName = [regex]::Replace($_.Name, $namePattern, "Challenge`$1$NewWord")
        $parent = Split-Path $_.FullName -Parent
        $newPath = Join-Path $parent $newName
        Rename-Item -Path $_.FullName -NewName $newName
        Write-Host "Dossier renommé : $($_.FullName) -> $newPath"
    }
}

# 2. Renommer les fichiers
Get-ChildItem -Recurse -File | ForEach-Object {
    if ($_ -match $namePattern) {
        $newName = [regex]::Replace($_.Name, $namePattern, "Challenge`$1$NewWord")
        $parent = Split-Path $_.FullName -Parent
        $newPath = Join-Path $parent $newName
        Rename-Item -Path $_.FullName -NewName $newName
        Write-Host "Fichier renommé : $($_.FullName) -> $newPath"
    }
}

# 3. Remplacer dans le contenu des fichiers
Get-ChildItem -Recurse -File | ForEach-Object {
    $file = $_.FullName
    $content = Get-Content $file -Raw
    $newContent = [regex]::Replace($content, $pattern, "`$1$NewWord")
    if ($newContent -ne $content) {
        Set-Content $file $newContent
        Write-Host "Contenu modifié : $file"
    }
}
