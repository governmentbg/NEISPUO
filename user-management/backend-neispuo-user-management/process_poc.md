1. Endpoint /student-create or /teacher-create gets called with the respective args.
1. A record gets created in a table called AzureDataSync with the following params
    - uuid
    - groupUuid 
    - state - new, in_progress, polling, failed
    - inputArgs - arguments passed from the API caller in a JSON object
    - pollingUuid - uuid returned by azure
    - resultObject - object returned by azure after a user is created


## Multi tenancy for CRON Jobs

The goal is to achieve multi-tenancy when running node via PM2 in multiple processes (https://www.npmjs.com/package/bull).

1. Read from MSSQL
1. read from Redis if a record is already being worked on (Redis will be used as mutex to lock in records so that multiple processes don't pick up and work on the same data)
1. check if record is already in redis, if not add it so that's only the current process is working on it; if it is try getting the next free record to start work on it; if you cannot find records exit;
1. do work on record e.g. polling/business logic for user creation and unlock/free the record from Redis once done.
 



## Strategy - Enrich while completed

The main concept is to enrich a JSON object via cron Jobs until the complete object is completed and we can persist it in our database in a singular transaction and verify which Azure Operations are successful or have failed. Example would be :

   *create-user.cron.ts*
   ```
   1. get record in AWAITING_USER_CREATE state which is not being processed
   2. perform azure user create operation and then change the state to POLL_USER_CREATE and add the returned from Azure uuid
   3. Update OBJECT_HISTORY to: 
   [
       {
           createdAt: date,
           operationType: 'SUBMITTED_AZURE_USER_CREATE_REQUEST',
           submittedData: {...},
           receivedData: pollingUuid,
           status: success/failed
       }
   ]
   3. Exit the Job
   ```

  *create-user-poll.cron.ts*
   ```
   1. get record in POLL_USER_CREATE state which is not being processed
   2. perform polling for azure user create operation and then change the state to AWAIT_STUDENT_ENROLLMENT and add the returned from Azure uuid
   3. Update the field OBJECT_HISTORY:
   [
       {
           createdAt: date,
           operationType: 'SUBMITTED_AZURE_USER_CREATE_REQUEST',
           submittedData: {...},
           receivedData: pollingUuid,
           status: success/failed
       }
   ]
   ```


  *persist-user-to-neispuo.cron.ts*
   ```
   1. get record in USER_NEISPUO_PERSIST state which is not being processed
   2. read OBJECT_HISTORY field only successful events and build the required object that you want to persist.
   
   Business logic could account for user getting created but failing to get enrolled. Add field in the SysUser table that will mark that so we at some point retry to enroll (e.g. Telelink service was down and now is up we need to reprocess)
   3. persist the object in the SysUser, SysRole related tables a.k.a. create a User in NEISPUO with the built object.

   ```



1. Create entry in the DB.
1. Have Cron Job run for each (or a set of records in a particular state) e.g. 
    TYPE = CREATE_NEISPUO_STUDENT which INCLUDES THE FOLLOWING TRANSITIONS:
1. 
    State = AWAITING_USER_CREATE
    OBJECT_HISTORY
    LAST_OPERATION_STATUS