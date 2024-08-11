#!/bin/bash

requests=10
interval_min=1
interval_max=10

execute_request() {
    count=0
    url_service="http://entrypoint"

    while true; do
        echo "Using service: $url_service"
        echo
        echo GET
        echo
        curl -X GET $url_service/weatherforecast -H "accept: text/plain"

        # Generate a random wait time between $interval_min and $interval_max seconds
        wait_time=$((RANDOM % ($interval_max - $interval_min + 1) + $interval_min))
        echo "Waiting $wait_time second(s)"
        sleep $wait_time
        echo
        echo
    done
}

execute_post_request() {
    count=1
    while true; do
        weatherforecast=$((RANDOM % ($35 - 2 + 1) + 2))
        curl -X 'POST' http://localhost:5110/weatherforecast?value=$weatherforecast -H 'accept: */*' -d ''
        echo count: $count
        count=$((count + 1))
        sleep 2
    done
}

for ((i = 1; i <= requests; i++)); do
    echo "Executing requests! (Index: $i)"
    execute_request &
done
wait
