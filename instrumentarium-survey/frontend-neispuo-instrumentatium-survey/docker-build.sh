date
docker kill survey-fe
docker rm survey-fe
docker image rm survey-fe
docker system prune -f
docker build --no-cache -t survey-fe .
if [ $? -eq 1 ]; then
  exit 1
fi
docker run --name survey-fe -d -p 4203:80 survey-fe
date
