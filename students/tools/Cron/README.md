# docker-cron
This is meant to serve as an example for running cron jobs inside of Docker. Its fine for simple things, but not recommended to run crons as root.

To use this Docker image, add your cronfiles to Cronjobs and build. You could also mount a volume containing your cronjobs via ```docker run -v /path/to/cronfiles:/etc/cron.d bambash/docker-cron```. Just make sure the permissions are set properly, otherwise cron will not be able to execute your jobs.
