// this enables circular json to be stringified
require('json-circular-stringify');
import { ValidationPipe } from '@nestjs/common';
import { NestFactory } from '@nestjs/core';
import { DocumentBuilder, SwaggerModule } from '@nestjs/swagger';
import * as fs from 'fs';
import helmet from 'helmet';
import { AppModule } from './app.module';
import { GlobalExceptionFilter } from './common/filters/global-exception.filter';
import { CorsService } from './config/cors/cors.service';
import { LoggingService } from './models/logging/logging.service';

async function bootstrap() {
    const app = await NestFactory.create(AppModule);
    const corsService = app.get<CorsService>(CorsService);
    app.useGlobalFilters(new GlobalExceptionFilter());
    app.use(helmet());

    app.useGlobalPipes(
        new ValidationPipe({
            whitelist: true,
            transform: true,
            forbidNonWhitelisted: true,
            forbidUnknownValues: true,
            transformOptions: {
                enableImplicitConversion: true,
            },
        }),
    );

    const config = new DocumentBuilder()
        .setTitle('User management API')
        .setDescription('User management API')
        .setVersion('1.0')
        .addBearerAuth()
        .build();
    const document = SwaggerModule.createDocument(app, config);
    if (process.env.APP_ENV === 'development') {
        SwaggerModule.setup('api', app, document, {
            swaggerOptions: {
                tagsSorter: 'alpha',
                operationsSorter: 'alpha',
            },
        });
    }
    fs.writeFileSync('./swagger-spec.json', JSON.stringify(document));
    app.enableCors(corsService.getCorsConfig());
    const logger = app.get<LoggingService>(LoggingService);
    app.useLogger(logger);
    await app.listen(+process.env.APP_PORT || 3005);
}

bootstrap();
