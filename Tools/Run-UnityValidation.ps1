param([ValidateSet('Scene','EditMode','PlayMode','Android','Preview','PortalsPreview','PolishedPreview','LivingAndroid','WorldAndroid','TerrainAndroid','MobileAndroid','DaylightAndroid','ExpandedAndroid','StartupRendering','FixedScene')][string]$Mode='Scene',[string]$Unity='C:/Program Files/Unity/Hub/Editor/6000.3.26f1/Editor/Unity.exe',[switch]$Wait,[string]$TestFilter='')
$ErrorActionPreference='Stop'
$projectRoot=Split-Path $PSScriptRoot -Parent
$marker=Join-Path $projectRoot '.codex-validation/LatestUnityValidation.txt'
if(!(Test-Path -LiteralPath $marker)){$marker=Join-Path $projectRoot 'Temp/LatestUnityValidation.txt'}
$validationRoot=(Get-Content -LiteralPath $marker).Trim()
$resolved=[IO.Path]::GetFullPath($validationRoot)
if(!$resolved.StartsWith([IO.Path]::GetFullPath((Join-Path $projectRoot '.codex-validation/UnityValidation-')),[StringComparison]::OrdinalIgnoreCase) -and !$resolved.StartsWith([IO.Path]::GetFullPath((Join-Path $projectRoot 'Temp/UnityValidation-')),[StringComparison]::OrdinalIgnoreCase)) {throw 'Unexpected validation workspace.'}
$lockPath=Join-Path $validationRoot 'Temp/UnityLockfile'
if(Test-Path -LiteralPath $lockPath)
{
    try {$lockProbe=[IO.File]::Open($lockPath,[IO.FileMode]::Open,[IO.FileAccess]::ReadWrite,[IO.FileShare]::None);$lockProbe.Dispose()}
    catch {throw 'Validation copy is still locked by Unity; wait for the current operation to finish.'}
}
# Only an isolated copy is synchronized. This script does not delete any files.
foreach($folder in @('Scripts','Editor','Tests','Resources'))
{
    $source=Join-Path $projectRoot ('Assets/PatiAdventure/'+$folder)
    Copy-Item -LiteralPath $source -Destination (Join-Path $validationRoot 'Assets/PatiAdventure') -Recurse -Force
    if(Test-Path -LiteralPath ($source+'.meta')) {Copy-Item -LiteralPath ($source+'.meta') -Destination (Join-Path $validationRoot ('Assets/PatiAdventure/'+$folder+'.meta')) -Force}
}
$arguments=@('-batchmode','-projectPath',('"'+$validationRoot+'"'),'-logFile',('"'+$validationRoot+'/ValidationResults/'+$Mode+'.log"'))
if($Mode -ne 'Preview' -and $Mode -ne 'PortalsPreview' -and $Mode -ne 'PolishedPreview' -and $Mode -ne 'StartupRendering'){$arguments+='-nographics'}
if($Mode -eq 'Scene') {$arguments+=@('-quit','-executeMethod','PatiAdventure.Editor.ProjectValidation.Run')}
elseif($Mode -eq 'FixedScene') {$arguments+=@('-quit','-executeMethod','PatiAdventure.Editor.ProjectValidation.CreateFixedPortalScene')}
elseif($Mode -eq 'Android') {$arguments+=@('-quit','-executeMethod','PatiAdventure.Editor.ProjectValidation.CompileAndroid')}
elseif($Mode -eq 'Preview') {$arguments+=@('-quit','-executeMethod','PatiAdventure.Editor.ProjectValidation.Capture')}
elseif($Mode -eq 'PortalsPreview') {$arguments+=@('-quit','-executeMethod','PatiAdventure.Editor.ProjectValidation.CapturePortals')}
elseif($Mode -eq 'PolishedPreview') {$arguments+=@('-quit','-executeMethod','PatiAdventure.Editor.ProjectValidation.CapturePolishedPortals')}
elseif($Mode -eq 'LivingAndroid') {$arguments+=@('-quit','-executeMethod','PatiAdventure.Editor.ProjectValidation.BuildLivingAndroid')}
elseif($Mode -eq 'WorldAndroid') {$arguments+=@('-quit','-executeMethod','PatiAdventure.Editor.ProjectValidation.BuildWorldPolishAndroid')}
elseif($Mode -eq 'TerrainAndroid') {$arguments+=@('-quit','-executeMethod','PatiAdventure.Editor.ProjectValidation.BuildTerrainAndroid')}
elseif($Mode -eq 'MobileAndroid') {$arguments+=@('-quit','-executeMethod','PatiAdventure.Editor.ProjectValidation.BuildMobileAndroid')}
elseif($Mode -eq 'DaylightAndroid') {$arguments+=@('-quit','-executeMethod','PatiAdventure.Editor.ProjectValidation.BuildDaylightAndroid')}
elseif($Mode -eq 'ExpandedAndroid') {$arguments+=@('-quit','-executeMethod','PatiAdventure.Editor.ProjectValidation.BuildExpandedAndroid')}
elseif($Mode -eq 'StartupRendering') {$arguments+=@('-runTests','-testPlatform','PlayMode','-assemblyNames','PatiAdventure.PlayModeTests','-testResults',('"'+$validationRoot+'/ValidationResults/'+$Mode+'.xml"'))}
else {$arguments+=@('-runTests','-testPlatform',$Mode,'-assemblyNames',('PatiAdventure.'+$Mode+'Tests'),'-testResults',('"'+$validationRoot+'/ValidationResults/'+$Mode+'.xml"'))}
if($TestFilter){$arguments+=@('-testFilter',('"'+$TestFilter+'"'))}
$process=Start-Process -FilePath $Unity -ArgumentList $arguments -WindowStyle Hidden -PassThru
Write-Output ('Unity '+$Mode+' process ID: '+$process.Id)
if($Wait){$process.WaitForExit();if($process.ExitCode -ne 0){throw "Unity $Mode failed: $($process.ExitCode)"};Write-Output ('Unity '+$Mode+' completed successfully.')}
