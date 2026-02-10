date
docker kill rmi-backend
docker rm rmi-backend
docker image rm rmi-backend
docker system prune -f
docker build --no-cache -t rmi-backend .
if [ $? -eq 1 ]; then
  exit 1
fi
docker run --name rmi-backend -d -p 3004:3001 rmi-backend
date
