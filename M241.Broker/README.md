# Mosquitto Broker

On Windows, you might encounter the following error when the [docker-entrypoint.sh](docker-entrypoint.sh) script runs within Docker.
```
: not found  | ./docker-entrypoint.sh: line 2:
mosquitto-1  | ./docker-entrypoint.sh: set: line 3: illegal option -
```

This is a classic CRLF issue, so using LF instead of CRLF in this script on Windows will work.
