# Azure integration Internal NEISPUO API - V1
 
## School

### Data transfer object and Swagger

1. Data transfer object - SchoolDTO
   ```
      {
      name: 'Hristo Botev',
      schoolId: 10004,
      description: 'IX OU Hristo Botev',
      headmaster: {
          firstName:	'Георги',
          middleName:	'Георгиев',
          lastName:	'Иванов',
      },
      phone: '0994284912',
      address: {
          town: {
          townId: 1,
          townName: 'София-град'
          },
          area: {
          areaId: 1,
          areaName: 'Слатина',

          },
          country: {
          countryId: 1,
          countryName: 1
          },
          postalCode: 1000,
          street:	'Иван Вазов 13'
      },
      highestGrade: 6,
      lowestGrade: 4,
      }
   ```
2. Swagger

   ```
           "definitions": {
              "name": {
                  "type": "string"
              },
              "schoolId": {
                  "type": "integer",
                  "format": "int32"
              },
              "description": {
                  "type": "string"
              },
              "headmaster": {
                  "type": "object",
                  "properties": {
                      "firstName": {
                          "type": "string"
                      },
                      "middleName": {
                          "type": "string"
                      },
                      "lastName": {
                          "type": "string"
                      }
                  }
              },
              "phone": {
                  "type": "string"
              },
              "address": {
                  "type": "object",
                  "properties": {
                      "town": {
                          "type": "object",
                          "properties": {
                              "townId": {
                                  "type": "integer",
                                  "format": "int32"
                              },
                              "townName": {
                                  "type": "string"
                              }
                          }
                      },
                      "area": {
                          "type": "object",
                          "properties": {
                              "areaId": {
                                  "type": "integer",
                                  "format": "int32"
                              },
                              "areaName": {
                                  "type": "string"
                              }
                          }
                      },
                      "country": {
                          "type": "object",
                          "properties": {
                              "countryId": {
                                  "type": "integer",
                                  "format": "int32"
                              },
                              "countryName": {
                                  "type": "integer",
                                  "format": "int32"
                              }
                          }
                      },
                      "postalCode": {
                          "type": "integer",
                          "format": "int32"
                      },
                      "street": {
                          "type": "string"
                      }
                  }
              },
              "highestGrade": {
                  "type": "integer",
                  "format": "int32"
              },
              "lowestGrade": {
                  "type": "integer",
                  "format": "int32"
              }
          }
   ```

   ### Methods

   1. POST
      - URI: http://dev.usermng-server.mon.bg/v1/azure-integration/school
      - Body - SchoolDTO
   1. PUT
      - URI: http://dev.usermng-server.mon.bg/v1/azure-integration/school
      - Body - SchoolDTO
   1. DELETE
      - URI: http://dev.usermng-server.mon.bg/v1/azure-integration/school/institituionID

## Student

### Data transfer object and Swagger

1. Data transfer object - StudentDTO
   ```
   {
        "personID": 1,
        "firstName": "ivnaov",
        "middleName": "petrov",
        "lastName": "georgiev",
        "birthDate": "2021-09-13T11:19:19.134Z",
        "schoolId": 10005, // може без schoolid
        "grade": "5 a",
   }
   ```
2. Swagger

   ```
              {
              "definitions": {
                  "personID": {
                      "type": "integer",
                      "format": "int32",
                      "nullable": true,
                  },
                  "firstName": {
                      "type": "string",
                      "nullable": false
                  },
                  "middleName": {
                      "type": "string",
                      "nullable": true,
                  },
                  "lastName": {
                      "type": "string",
                      "nullable": false
                  },
                  "birthDate": {
                      "type": "string",
                      "nullable": true,
                  },
                  "schoolId": {
                      "type": "integer",
                      "format": "int32",
                      "nullable": false
                  },
                  "grade": {
                      "type": "string",
                      "nullable": true,
                  },
                  "requestedSysRoleID": {
                      "type": "integer",
                      "format": "int32",
                      "nullable": false
                  }
              }
          }
   ```

   ### Methods

   1. POST
      - URI: http://dev.usermng-server.mon.bg/v1/azure-integration/student
      - Body - StudentDTO
   1. PUT
      - URI: http://dev.usermng-server.mon.bg/v1/azure-integration/student
      - Body - StudentDTO
   1. DELETE
      - URI: http://dev.usermng-server.mon.bg/v1/azure-integration/student/personID

## Teacher

### Data transfer object and Swagger

1. Data transfer object - TeacherDTO
   ```
   {
    "personID": 1,
    "firstName": "ivnaov",
    "middleName": "petrov",
    "lastName": "georgiev",
    "birthDate": "2021-09-13T11:19:19.134Z",
    "schoolId": 10005,
    "grade": "5 a" // да изпращаме null
   }
   ```
2. Swagger

   ```
              {
              "definitions": {
                  "personID": {
                      "type": "integer",
                      "format": "int32",
                      "nullable": true,
                  },
                  "firstName": {
                      "type": "string",
                      "nullable": false
                  },
                  "middleName": {
                      "type": "string",
                      "nullable": true,
                  },
                  "lastName": {
                      "type": "string",
                      "nullable": false
                  },
                  "birthDate": {
                      "type": "string",
                      "nullable": true,
                  },
                  "schoolId": {
                      "type": "integer",
                      "format": "int32",
                      "nullable": false
                  },
                  "grade": {
                      "type": "string",
                      "nullable": true,
                  },
                  "requestedSysRoleID": {
                      "type": "integer",
                      "format": "int32",
                      "nullable": false
                  }
              }
          }
   ```

   ### Methods

   1. POST
      - URI: http://dev.usermng-server.mon.bg/v1/azure-integration/teacher
      - Body - TeacherDTO
   1. PUT
      - URI: http://dev.usermng-server.mon.bg/v1/azure-integration/teacher
      - Body - TeacherDTO
   1. DELETE
      - URI: http://dev.usermng-server.mon.bg/v1/azure-integration/teacher/personID
