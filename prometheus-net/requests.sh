#!/bin/bash

requests=10
interval_min=1
interval_max=10

execute_request() {
    count=0
    services=("http://localhost:5180" "http://localhost:5181" "http://localhost:5182" "http://localhost:5183")

    while true; do
        count=$((count + 1))

        selected_index=$((RANDOM % ${#services[@]}))
        url_service=${services[$selected_index]}

        echo "Using service: $url_service"

        echo
        echo POST
        echo
        curl -X POST \
            $url_service/api/weatherforecast \
            -H "accept: */*" \
            -H "Content-Type: application/json" \
            -d '{
                "date": "2024-06-18T18:42:11",
                "temperatureC": 1,
                "summary": "Freezing"
            }'

        echo
        echo
        echo GET
        echo
        curl -X GET $url_service/api/weatherforecast -H "accept: text/plain"

        # Generate a random wait time between 1 and 4 seconds
        wait_time=$((RANDOM % ($interval_max - $interval_min + 1) + $interval_min))
        echo "Waiting $wait_time second(s)"
        sleep $wait_time
        echo
    done

    echo "Finished sending requests."
}

for ((i = 1; i <= requests; i++)); do
    echo "Executing requests! (Index: $i)"
    execute_request &
done
wait
