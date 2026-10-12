param([string]$UnityData = 'C:/Program Files/Unity/Hub/Editor/6000.3.26f1/Editor/Data', [switch]$Android)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot -Parent
$outputFolder = Join-Path $projectRoot $(if($Android){'Temp/CodeValidation/Android'}else{'Temp/CodeValidation/Editor'})
New-Item -ItemType Directory -Force -Path $outputFolder | Out-Null
# Modular UnityEditor assemblies are under Managed/UnityEngine. Never also
# reference the monolithic Managed/UnityEditor.dll: that caused CS0433.
$references = @(Get-ChildItem "$UnityData/Managed/UnityEngine" -Filter 'UnityEngine*.dll')
$references += Get-ChildItem "$UnityData/NetStandard/ref/2.1.0" -Filter '*.dll'
$references += Get-ChildItem "$UnityData/NetStandard/compat/2.1.0/shims/netstandard" -Filter '*.dll'
$references += Get-Item "$projectRoot/Library/ScriptAssemblies/Unity.InputSystem.dll"
$references += Get-Item "$projectRoot/Library/ScriptAssemblies/UnityEngine.UI.dll"
$references += Get-Item "$projectRoot/Library/ScriptAssemblies/Unity.RenderPipelines.Universal.Runtime.dll"
$references += Get-Item "$projectRoot/Library/ScriptAssemblies/Unity.RenderPipelines.Core.Runtime.dll"
if (!$Android) { $references += Get-ChildItem "$UnityData/Managed/UnityEngine" -Filter 'UnityEditor*.dll' }
$arguments = @('/nologo', '/target:library', '/nostdlib+', '/langversion:9', ('/out:"' + $outputFolder + '/PatiAdventure.Validation.dll"'))
$arguments += if ($Android) { '/define:UNITY_ANDROID' } else { '/define:UNITY_EDITOR' }
$arguments += $references | Sort-Object FullName -Unique | ForEach-Object { '/reference:"' + $_.FullName + '"' }
$sources = @(Get-ChildItem "$projectRoot/Assets/PatiAdventure/Scripts" -Recurse -Filter '*.cs')
if (!$Android) { $sources += Get-ChildItem "$projectRoot/Assets/PatiAdventure/Editor" -Filter '*.cs' }
foreach ($source in $sources) {
    $meta = $source.FullName + '.meta'
    if (Test-Path -LiteralPath $meta) {
        $guid = [regex]::Match([IO.File]::ReadAllText($meta), '(?m)^guid: (\S+)')
        if (!$guid.Success -or $guid.Groups[1].Value -notmatch '^[0-9a-f]{32}$') {
            throw ('Invalid Unity script GUID: ' + $meta)
        }
    }
}
$arguments += $sources | ForEach-Object { '"' + $_.FullName + '"' }
$responseFile = Join-Path $outputFolder 'Compile.rsp'
[IO.File]::WriteAllLines($responseFile, $arguments)
& "$UnityData/MonoBleedingEdge/bin/mono.exe" "$UnityData/MonoBleedingEdge/lib/mono/4.5/csc.exe" ('@' + $responseFile)
if ($LASTEXITCODE -ne 0) { throw "C# validation failed: $LASTEXITCODE" }
Write-Output 'C# validation passed. This supplements, but does not replace, actual Unity compilation.'
