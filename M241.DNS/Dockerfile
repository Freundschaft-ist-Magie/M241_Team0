FROM gradle:jdk21-graal AS build
COPY gradlew /code/gradlew
COPY gradle /code/gradle
COPY build.gradle.kts /code/
COPY settings.gradle.kts /code/
WORKDIR /code
RUN ./gradlew --version
COPY src /code/src
RUN ./gradlew nativeCompile

FROM alpine:latest
WORKDIR /work/
RUN apk update && apk upgrade --no-cache
RUN apk add --no-cache gcompat
RUN chown 1001 /work \
    && chmod "g+rwX" /work \
    && chown 1001:root /work

COPY --from=build --chown=1001:root /code/build/native/nativeCompile/m241-cheap-dns /work/application

EXPOSE 8080
USER 1001
ENTRYPOINT ["./application"]
