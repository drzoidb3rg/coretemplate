if ($Env:DOCKER_COMPOSE_VERSION -eq $null -or $Env:DOCKER_COMPOSE_VERSION.Length -eq 0) {
    $Env:DOCKER_COMPOSE_VERSION = "release"
}

if (-not $Env:DOCKER_HOST) {
    docker-machine env --shell=powershell default | Invoke-Expression
    if (-not $?) { exit $LastExitCode }
}

docker inspect --type=image --format='{{.Comment}}' "docker-compose-$Env:DOCKER_COMPOSE_VERSION"

if (-not $?) {
    echo "Building docker-compose image..."
    docker build -t "docker-compose-$Env:DOCKER_COMPOSE_VERSION" "https://github.com/docker/compose.git#$Env:DOCKER_COMPOSE_VERSION"
    if (-not $?) { exit $LastExitCode }
}

$local="/$($PWD -replace '^(.):(.*)$', '"$1".ToLower()+"$2".Replace("\","/")' | Invoke-Expression)"
docker run --rm -ti -v /var/run/docker.sock:/var/run/docker.sock -v "${local}:$local" -w "$local" "docker-compose-$Env:DOCKER_COMPOSE_VERSION" $args
exit $LastExitCode