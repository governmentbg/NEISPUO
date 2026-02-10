date
docker kill usermng-fe
docker rm usermng-fe
docker image rm usermng-fe
docker system prune -f
docker build --no-cache -t usermng-fe .
if [ $? -eq 1 ]; then
  exit 1
fi
docker run --name usermng-fe -d -p 4205:80 usermng-fe
date
