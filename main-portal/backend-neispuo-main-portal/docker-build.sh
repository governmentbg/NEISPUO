date
docker kill main-portal-be
docker rm main-portal-be
docker image rm main-portal-be
docker system prune -f
docker build --no-cache -t main-portal-be .
if [ $? -eq 1 ]; then
  exit 1
fi
docker run --name main-portal-be -d -p 3001:3001 main-portal-be
date
