# GET /api/Events/{eventId}
<!-- This endpoint is used to check the status of an existing event by its eventID(GUID) -->
{
<!-- this will be the guid that you are checking for -->
  "id": "string", 
<!-- this will show the workflowtype chosen when creating the entity -->
  "type": "string",
  <!-- this contains information about the event you wish to check -->
  "actionsTriggered": [
    {
<!-- this is the name of the action it is not important -->
      "action": "CreateSchool",
<!-- this is the status of the event -->
      "status": "IN_PROGRESS",
<!-- usually is filled with data for the event -->
      "data": {
      },
    }
  ]
}

# GET /api/Events
<!-- this endpoint is the same as the first one but instead of a path param with a single eventID yuo pass in an array in the body and the result is the same its just in an array containing info for each value passed in the request body array -->

# POST /api/Events

<!-- this endpoint is used to create update delete entities in azure. -->

{
<!-- you have to pass an appropriate WorkflowType from the WorkflowType enum -->
  "type": WorkflowType,
<!-- this has to be an eventDTO corresponding to the WorkflowType -->
  "attributes":  EventDTO
}

<!-- here i will list DTOs which pass as an EventDTO and i will explain from where you should get info for their population -->
# OrgEventDTO
{
    <!-- used when creating and updating a school -->
    <!-- core.Institution.InstitutionID -->
    id: string 
    <!-- core.Institution.Name -->
    name: string
    <!-- core.Institution.Abbreviation -->
    description: string
    <!-- core.SysUser.SysUserID -->
    principalId: string
    <!-- core.Person.PersonID -->
    principalName: string
    <!-- core.SysUser.Username -->
    principalEmail: string
    <!-- we not filling this right now -->
    highestGrade: number
    <!-- we not filling this right now -->
    lowestGrade: number
    <!-- we not filling this right now -->
    phone: string
    address: {
        <!-- location.Town.Name -->
        city: string
        <!-- location.LocalArea.LocalAreaID -->
        area: string
        <!-- location.Country.CountryID -->
        country: string
        <!-- we not filling this right now -->
        postalCode: string
        <!-- we not filling this right now -->
        street: string
    }
}
# DeleteOrgEventDTO
{
    <!-- used when deleting a school -->
    <!-- core.Institution.InstitutionID -->
    id: string 
}

# ClassEventDTO 
{
    <!-- used when creating or updating a class -->
    <!-- inst_year.Curriculum.CurriculumID -->
    id: string
    <!-- this is a combination of inst_year.ClassGroup.ClassName for the given CurriculumID -->
    title: string
    <!-- we not filling this right now -->
    classCode: string
    <!-- core.Institution.InstitutionID -->
    orgId: string
    term: {
    <!-- we not filling this right now -->
        id: string
    <!-- we not filling this right now -->
        name: string
        <!-- we are filling this with 2021-12-22 as a CONSTANT for now we dont care about this info-->
        startDate: string
        <!-- we are filling this with 2021-12-22 as a CONSTANT for now we dont care about this info-->
        endDate: string
    }
}

# DeleteClassEventDTO
{
    <!-- used when deleting a class -->
    <!-- inst_year.Curriculum.CurriculumID -->
    id: string 
}

# UserEventDTO
{
    <!-- used when creating or updating a user -->
    <!-- core.Person.PersonalID -->
    id: string
    <!-- core.Person.PersonalID -->
    identifier: string
    <!-- core.Person.FirstName -->
    firstName: string
    <!-- core.Person.MiddleName -->
    middleName: string
    <!-- core.Person.LastName -->
    surname: string
    <!-- this is only required for parent and we use the Encryption service in the backend to generate a RSA encrypted string for the password -->
    password: string
    <!-- core.SysUser.Username -->
    email: string
    <!-- we not filling this right now -->
    phone: string
    <!-- we not filling this right now -->
    grade: string
    <!-- core.Institution.InstitutionID -->
    schoolId: string
    <!-- core.Person.BirthDate -->
    birthDate: string
    <!-- this is a constant from the enum UserRoleType(parent,student,teacher) -->
    role: UserRoleType
    <!-- we always set this true i dont know why -->
    accountEnabled: boolean
}

# DeleteUserEventDTO
{
    <!-- used when deleting a user -->
    <!-- core.Person.PersonalID -->
    id: string,
    <!-- this is a constant from the enum UserRoleType(parent,student,teacher) -->
    role: UserRoleType
}

# EnrollmentEventDTO
{
    <!-- used when enrolling a user to schoool or class or both -->
    <!-- core.Person.PersonalID -->
    userId: string
    <!-- inst_year.Curriculum.CurriculumID -->
    classId: string
    <!-- inst_year.Curriculum.CurriculumID -->
    orgId: string
}