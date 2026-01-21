# Installation Process
 - Git clone the project
 - npm install modules in both backend and frontend directories
 - Setup environment files 
    - frontend file location `/src/environments/environment.ts` 
    - backend file location `/config/.env`

# Seeding the DB
 - To seed questionaires, questions, criterias, indicators sub indicators run `npm run db:seed` from the backend directory
 - Seeders location: `/src/database/seeds/`

# Running the latest migrations
 -  run `npm run db:migrate` from the backend directory
 -  Migrations location: `/migrations/`

# Swagger documentation
 - Swagger documentation exported as json file `/backend/swagger-spec.json` it will be imported into Jira
 - Swagger must not be enabled all the time because this way all the endpoints will be exposed. To enable it:
    - Go to `backend/src/main.ts`
    - find the row `_setupSwagger(app);` and uncomment it
    - Go to `BACKEND_URL/api` in your browser
    - Authenticate with your bearer token