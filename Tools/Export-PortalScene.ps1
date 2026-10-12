param([string]$SceneName='PatiAdventure_Portals_Ready')
$ErrorActionPreference='Stop'
if($SceneName -notmatch '^[A-Za-z0-9_]+$'){throw 'Invalid scene name.'}
$projectRoot=Split-Path $PSScriptRoot -Parent
$marker=Join-Path $projectRoot '.codex-validation/LatestUnityValidation.txt'
if(!(Test-Path -LiteralPath $marker)){$marker=Join-Path $projectRoot 'Temp/LatestUnityValidation.txt'}
$validationRoot=[IO.Path]::GetFullPath((Get-Content -LiteralPath $marker).Trim())
if(!$validationRoot.StartsWith((Join-Path $projectRoot '.codex-validation/UnityValidation-'),[StringComparison]::OrdinalIgnoreCase) -and !$validationRoot.StartsWith((Join-Path $projectRoot 'Temp/UnityValidation-'),[StringComparison]::OrdinalIgnoreCase)){throw 'Unexpected validation project.'}
$relative=(Get-Content -LiteralPath (Join-Path $validationRoot 'ValidationResults/GeneratedPortalScene.txt')).Trim()
$sceneSource=Join-Path $validationRoot $relative
$yaml=[IO.File]::ReadAllText($sceneSource)
$guids=@([regex]::Matches($yaml,'guid: ([0-9a-f]{32})') | ForEach-Object {$_.Groups[1].Value} | Sort-Object -Unique)
$folders=@{}
foreach($meta in Get-ChildItem -LiteralPath (Join-Path $validationRoot 'Assets/PatiAdventure/Generated') -Recurse -Filter '*.meta')
{
    $match=[regex]::Match([IO.File]::ReadAllText($meta.FullName),'guid: ([0-9a-f]{32})')
    if($match.Success -and $guids -contains $match.Groups[1].Value -and $meta.FullName -notlike '*.unity.meta'){$folders[$meta.DirectoryName]=$true}
}
foreach($folder in $folders.Keys)
{
    $name=$SceneName+'_'+(Split-Path $folder -Leaf)
    $target=Join-Path $projectRoot ('Assets/PatiAdventure/Generated/'+$name)
    if(Test-Path -LiteralPath $target){throw ('Refusing to overwrite '+$target)}
    Copy-Item -LiteralPath $folder -Destination $target -Recurse
    if(Test-Path -LiteralPath ($folder+'.meta')){Copy-Item -LiteralPath ($folder+'.meta') -Destination ($target+'.meta')}
}
# Newly added scripts must retain the GUIDs used by the validated scene.
foreach($folder in @('Scripts','Editor','Tests','Resources'))
{
    $source=Join-Path $validationRoot ('Assets/PatiAdventure/'+$folder)
    foreach($meta in Get-ChildItem -LiteralPath $source -Recurse -Filter '*.meta')
    {
        $rel=$meta.FullName.Substring($validationRoot.Length+1)
        $target=Join-Path $projectRoot $rel
        if(!(Test-Path -LiteralPath $target)){Copy-Item -LiteralPath $meta.FullName -Destination $target}
    }
}
$destination=Join-Path $projectRoot ('Assets/PatiAdventure/Generated/'+$SceneName+'.unity')
if(Test-Path -LiteralPath $destination){throw ('Refusing to overwrite '+$destination)}
Copy-Item -LiteralPath $sceneSource -Destination $destination
Copy-Item -LiteralPath ($sceneSource+'.meta') -Destination ($destination+'.meta')
Write-Output ('Exported validated portal scene: '+$destination)
