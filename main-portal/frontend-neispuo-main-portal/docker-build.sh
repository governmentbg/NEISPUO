date

docker kill main-portal-fe
docker rm main-portal-fe
docker image rm main-portal-fe
docker system prune -f
docker build --build-arg ENV=test --no-cache -t main-portal-fe .
if [ $? -eq 1 ]; then
  exit 1
fi
docker run --name main-portal-fe -d -p 4201:80 main-portal-fe
date
