ARG MOSQUITTO_VERSION
FROM eclipse-mosquitto:2.0.21

ARG CERTPATH

RUN apk add --no-cache dos2unix
COPY config/mosquitto.conf /mosquitto/config/mosquitto.conf
COPY ${CERTPATH}/ /mosquitto/certs/
COPY docker-entrypoint.sh /

RUN dos2unix /docker-entrypoint.sh
ENTRYPOINT ["sh", "./docker-entrypoint.sh"]

CMD ["/usr/sbin/mosquitto", "-c", "/mosquitto/config/mosquitto.conf"]
