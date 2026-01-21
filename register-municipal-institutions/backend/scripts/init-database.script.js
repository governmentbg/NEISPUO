'use strict';

const { env } = require('process');

/*
  This script enables us to connect to mysql server without specifying a database.
  This is necessary in order to create database.
  TODO: Consider removing Knex dependency in favor of using other already installed mysql connectors
  TODO: Implement db recreate script (make sure process.env.NODE_ENV check bellow is being used)
*/

const ormConfig = require('../ormconfig').find(connection => connection.name === 'default');

const typeOrmToKnexConfig = typeOrm => {
    return {
        client: typeOrm.type,
        useNullAsDefault: true,
        connection: {
            // database: typeOrm.database, // connect without database selected
            user: process.env.DB_USERNAME,
            password: process.env.DB_PASSWORD,
            host: process.env.DB_HOST,
            port: process.env.DB_PORT
        },
        pool: {
            min: 2,
            max: 10
        }
    };
};

const { client, connection } = typeOrmToKnexConfig(ormConfig);
const knex = require('knex')({ client, connection });

// set drop database flag
const dropFlag = process.argv[2] === '--force';

async function initDb() {
    try {
        if (dropFlag && process.env.NODE_ENV !== 'development') {
            throw new Error(
                '\n☠️\tDrop database is supported in development environment only!\t☠️\n☠️️\tYou are welcome!\t☠️\n'
            );
        } else if (dropFlag) {
            await knex.raw(`DROP DATABASE IF EXISTS \`${ormConfig.database}\` ;`);
            console.log('Successfully dropped db');
        }

        await knex.raw(
            `CREATE DATABASE IF NOT EXISTS \`${ormConfig.database}\` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;`
        );
        console.log(`Successfully ${dropFlag ? 're' : ''}created db`);
    } catch (e) {
        console.log(e);
        console.log('\nSomething went wrong with your init-database.script script. See log above');
        process.exit(1);
    } finally {
        knex.destroy();
    }
}

initDb();
