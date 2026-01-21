import { SIEMLoggerOptions } from './siem-logger-options.interface';
require('dotenv').config({ path: `./.env` });

export const siemLoggerOptions: SIEMLoggerOptions = {
    host: process.env.SIEM_HOST,
    port: +process.env.SIEM_PORT,
    appId: process.env.SIEM_APP_ID,
    enabled: process.env.SIEM_ENABLED,
};
