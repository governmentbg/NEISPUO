# Startup instructions:

1. CHANGE! TELELINK_SCOPE variable inside ENV file to: 
https://graph.microsoft.com/.default
2. CHANGE! ormConfig entities variable inside db config file to: 
entities: ['src/**/*.entity.ts'],
3. Open a terminal in the root directory and execute the following command:
npm run sync-organizations
4. !!!VERY VERY VERY IMPORTANT!!!. After starting the script change every variable from point 1. and 2. back to normal otherwise someone could restart the app and it will not run!!!!!
5. To stop script from executing press:
CTRL + C
