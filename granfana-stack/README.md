# Curls
``` sh
curl -X 'POST' 'http://localhost:5110/weatherforecast?value=1' -H 'accept: */*' -d ''
```

``` sh
i=10;
while true; do
  curl -X 'POST' "http://localhost:5110/weatherforecast?value=$i" -H 'accept: */*' -d ''
  i=$((i+1))
  echo i: $i
  sleep 1
done;
```

# References

- https://www.mytechramblings.com/posts/getting-started-with-opentelemetry-metrics-and-dotnet-part-2/