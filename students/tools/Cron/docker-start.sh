#!/bin/sh

# Копиране на наличните променливи в /etc/environment, за да са достъпни за cron
env >> /etc/environment
cron -f

