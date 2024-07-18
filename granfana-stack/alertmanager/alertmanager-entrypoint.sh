#!/bin/sh

echo Preparing file with environment variables
sed "s|SLACK_API_URL|${SLACK_API_URL}|g" /prometheus/alertmanager.yml > ./alertmanager.conf.yml
echo
echo
echo
echo reading new file
cat /prometheus/alertmanager.conf.yml
echo
echo
echo
echo Executing AlertManager
/bin/alertmanager --config.file=./alertmanager.conf.yml
