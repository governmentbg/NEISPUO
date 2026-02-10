date
docker kill usermng-be
docker rm usermng-be
docker image rm usermng-be
docker build --no-cache -t usermng-be .
if [ $? -eq 1 ]; then
  exit 1
fi
docker run --name usermng-be -d -p 3005:3001 usermng-be
date
