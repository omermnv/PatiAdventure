param([string]$Unity = 'C:/Program Files/Unity/Hub/Editor/6000.3.26f1/Editor/Unity.exe',[switch]$Preview)
$ErrorActionPreference='Stop'
$projectRoot=Split-Path $PSScriptRoot -Parent
$validationRoot=Join-Path $projectRoot ('.codex-validation/UnityValidation-'+(Get-Date -Format 'yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Path $validationRoot | Out-Null
foreach($folder in @('Assets','Packages','ProjectSettings')) { Copy-Item -LiteralPath (Join-Path $projectRoot $folder) -Destination $validationRoot -Recurse }
# Give the copy a separate save namespace so automated play tests cannot touch user saves.
$settingsPath=Join-Path $validationRoot 'ProjectSettings/ProjectSettings.asset'
$settings=Get-Content -LiteralPath $settingsPath -Raw
$settings=$settings -replace 'companyName:.*','companyName: PatiValidation' -replace 'productName:.*','productName: PatiValidation'
[IO.File]::WriteAllText($settingsPath,$settings)
New-Item -ItemType Directory -Force (Join-Path $validationRoot 'ValidationResults') | Out-Null
Set-Content -LiteralPath (Join-Path $projectRoot '.codex-validation/LatestUnityValidation.txt') -Value $validationRoot
$arguments=@('-batchmode','-nographics','-quit','-projectPath',('"'+$validationRoot+'"'),'-executeMethod','PatiAdventure.Editor.ProjectValidation.Run','-logFile',('"'+$validationRoot+'/ValidationResults/Unity.log"'))
$process=Start-Process -FilePath $Unity -ArgumentList $arguments -WindowStyle Hidden -PassThru
Write-Output ('Validation project: '+$validationRoot)
Write-Output ('Unity process ID: '+$process.Id)
if($Preview)
{
    # Bootstrap imports without a graphics device, then render in the initialized copy.
    $process.WaitForExit()
    if($process.ExitCode -ne 0){throw ('Initial scene validation failed: '+$process.ExitCode)}
    & (Join-Path $PSScriptRoot 'Run-UnityValidation.ps1') -Mode PolishedPreview -Unity $Unity
}
