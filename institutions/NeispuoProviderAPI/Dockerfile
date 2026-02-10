FROM adoptopenjdk/openjdk11:latest
RUN mkdir /opt/app
COPY NEISPUOIntegration-1.0-SNAPSHOT.jar /opt/app
COPY exportMapping.json /opt/app
COPY jsonMapping.json /opt/app
COPY schemaMapping.json /opt/app
COPY validations.json /opt/app
COPY exportMappingIds.json /opt/app
CMD ["java", "-jar", "/opt/app/NEISPUOIntegration-1.0-SNAPSHOT.jar"]