param([string]$DOCKER_MACHINE_NAME="default")
$VirtualBox=Get-ItemProperty -Path Registry::HKEY_LOCAL_MACHINE\SOFTWARE\Oracle\VirtualBox
$DockerMachine="docker-machine.exe"
$VirtualBoxManage=$VirtualBox.InstallDir + "VBoxManage.exe"
& $VirtualBoxManage showvminfo $DOCKER_MACHINE_NAME | Out-Null

if ($?) {
    echo "Machine $DOCKER_MACHINE_NAME already exists in VirtualBox."
} else {
    echo "Creating Machine $DOCKER_MACHINE_NAME in VirtualBox..."
    & $DockerMachine rm -f $DOCKER_MACHINE_NAME | Out-Null
    & $DockerMachine create -d virtualbox --virtualbox-memory 2048 $DOCKER_MACHINE_NAME
}

echo "Starting machine $DOCKER_MACHINE_NAME..."
& $DockerMachine start $DOCKER_MACHINE_NAME

$MountOptions = "defaults,iocharset=utf8"
$DockerPasswd = & $DockerMachine ssh $DOCKER_MACHINE_NAME "grep '^docker:' /etc/passwd"
if ($DockerPasswd.StartsWith('docker:')) {
    $MountOptions = "$MountOptions,$($DockerPasswd -replace '^docker:[^:]*:(\d+):(\d+):.*$', 'uid=$1,gid=$2')"
}
$VirtualBoxMounts = & $DockerMachine ssh $DOCKER_MACHINE_NAME mount | ForEach-Object {
        if ($_ -match 'on (.*) type vboxsf ') {
            $Matches[1]
        }
    }
& $DockerMachine ssh $DOCKER_MACHINE_NAME "sudo VBoxControl sharedfolder list -automount" | ForEach-Object {
    if ($_ -match '^[0-9]+ - (?<ShareName>((?<DriveLetter>[A-Za-z]):)?(?<FolderName>.*))$') {
        $MountPoint = "$($Matches['DriveLetter'])$($Matches['FolderName'])"
        if (-not ($MountPoint -match '^/')) {
            $MountPoint = "/$MountPoint"
        }
        if (-not ($VirtualBoxMounts -ccontains $MountPoint)) {
            echo "Mounting $($Matches['ShareName']) to $MountPoint..."
            & $DockerMachine ssh $DOCKER_MACHINE_NAME "sudo mkdir -p $MountPoint && sudo mount -t vboxsf -o $MountOptions $($Matches['ShareName']) $MountPoint"
        }
    }
}

Remove-Variable MountOptions, DockerPasswd, VirtualBoxMounts, MountPoint

echo "Setting environment variables for machine $DOCKER_MACHINE_NAME..."
& $DockerMachine env --shell=powershell $DOCKER_MACHINE_NAME | Invoke-Expression

echo '


                        ##         .
                  ## ## ##        ==
               ## ## ## ## ##    ===
           /"""""""""""""""""\___/ ===
      ~~~ {~ salty mcf**k face ~~ /  ===- ~~~
           \______ o           __/
             \    \         __/
              \____\_______/
'

echo "docker is configured to use the $DOCKER_MACHINE_NAME machine at $Env:DOCKER_HOST"