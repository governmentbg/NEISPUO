import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import helmet from 'helmet';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);

  app.use(helmet());
    app.enableCors({
        origin: process.env.CORS_DOMAINS === 'any' ? '*' : process.env.CORS_DOMAINS.split(','),
        methods: process.env.CORS_METHODS,
        allowedHeaders: process.env.CORS_ALLOWED_HEADERS,
        exposedHeaders: process.env.CORS_EXPOSED_HEADERS,
        credentials: process.env.CORS_CREDENTIALS === 'true',
        maxAge: +process.env.CORS_MAX_AGE,
        preflightContinue: process.env.CORS_PREFLIGHT_CONTINUE === 'true',
        optionsSuccessStatus: +process.env.CORS_OPTIONS_SUCCESS_STATUS
    });
  await app.listen(+process.env.APP_PORT);
}
bootstrap();
