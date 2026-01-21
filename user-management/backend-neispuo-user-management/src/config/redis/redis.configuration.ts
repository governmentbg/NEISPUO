import { registerAs } from '@nestjs/config';

export default registerAs('redis', () => ({
    hostname: process.env.REDIS_HOSTNAME,
    port: process.env.REDIS_PORT,
    password: process.env.REDIS_PASSWORD,
}));
