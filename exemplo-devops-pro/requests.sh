#!/bin/bash

count=0
interval=1
while true; do
    curl -X POST \
        "http://localhost:8080/Produto" \
        -H "accept: */*" \
        -H "Content-Type: application/json" \
        -d "{\"nome\":\"teste $count\",\"preco\":10,\"categoria\":\"abc $count\"}"

    curl -X GET "http://localhost:8080/Produto" -H "accept: text/plain"

    count=$((count + 1))
    # echo
    # echo "Waiting $interval second(s)"
    # sleep $interval
    # echo
done
