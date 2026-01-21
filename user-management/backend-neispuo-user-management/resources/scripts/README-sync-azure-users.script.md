# Startup instructions:

1. CHANGE! ormConfig entities variable inside db config file to: 
entities: ['src/**/*.entity.ts'],
1. Open a terminal in the root directory and execute the following command :
    `npm run sync` - to sync all users for all institutions
    `npm run sync teacher INSTITUTION_ID` - to sync teachers in a particular institution
    `npm run sync student INSTITUTION_ID` - to sync students in a particular institution
     `npm run sync`
    ```
    Run for all institutions with unsynced teacher: 
    args: 
        enableUserManagementSync - sync users in database if no Azure user exists
        fromPersonCreationDate - from which person creation date do you want to start
    npm run sync teacher all true 2022-01-01 https://usermng-server.mon.bg access_token_from_prod >> 2022-01-01_teacher_all_prod.log
    ```
1. !!!VERY VERY VERY IMPORTANT!!!. After starting the script change every variable from point 1. and 2. back to normal otherwise someone could restart the app and it will not run!!!!!
1. To stop script from executing press:
CTRL + C
