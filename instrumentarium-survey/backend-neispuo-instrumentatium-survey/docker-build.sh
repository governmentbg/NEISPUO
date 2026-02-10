date
docker kill survey-be
docker rm survey-be
docker image rm survey-be
docker build --no-cache -t survey-be .
if [ $? -eq 1 ]; then
  exit 1
fi
docker run --name survey-be -d -p 3003:3001 survey-be
date
