# M241 Team 0

To start this project in production, create an .env in the root of the folder. An example .env is ready.

Move the following SSL certificates inside the certs folder inside root:

./certs/fullchain.pem

./certs/privkey.pem

To deploy, make sure docker is installed and start the release compose:

``` sh
    docker compose -f .\docker-compose.yml -f .\docker-compose.release.yml pull
    docker compose -f .\docker-compose.yml -f .\docker-compose.release.yml up -d --build
```

The frontend needs the url to the backend. This is currently hardcoded until a fix is found.
Adjust the Backend URL inside M241.Frontend/Dockerfile:

RUN echo "VITE_API_URL=\"{YOUR_URL}\"" > ./.env

- [Firmware](./M241.Firmware/README.md)
- [Client](./M241.Client/README.md)
- [Server](./M241.Server/README.md)
- [Broker](./M241.Broker/README.md)
