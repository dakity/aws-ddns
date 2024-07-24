param(
    [string] $dockerfilePath = "./Dakity.AwsTools.R53.Ddns/Dockerfile",
    [string] $user = "DOCKER-USER",
    [string] $repository = "DOCKER-REPO",
    [string] $tag = "DOCKER-TAG"
)

Write-Host "Building Docker image..."
docker build -t $repository -f $dockerfilePath .

Write-Host "Tagging image to repository..."
docker tag $repository $user/${repository}:${tag}

Write-Host "Pushing Docker image to repository..."
docker push $user/${repository}:${tag}
