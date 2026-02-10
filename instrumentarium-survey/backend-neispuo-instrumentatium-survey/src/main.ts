import { NestFactory } from '@nestjs/core';
import { CrudConfigService } from '@nestjsx/crud';
import * as bodyParser from 'body-parser';
_setupCrudDefaultOptions();
import { AppModule } from './app.module';
import { SwaggerModule, DocumentBuilder } from '@nestjs/swagger';
import { INestApplication } from '@nestjs/common';
import { ConfigService } from './shared/services/config/config.service';
import { NestExpressApplication } from '@nestjs/platform-express';
import fs from 'fs';
import helmet from 'helmet';

async function bootstrap() {
    const app = await NestFactory.create<NestExpressApplication>(AppModule);

    const configService = app.get<ConfigService>(ConfigService);

    app.set('trust proxy', 1);
    app.use(bodyParser.json({ limit: `${process.env.MAX_BODY_SIZE_MB}mb` }));
    app.use(
        bodyParser.urlencoded({
            // limit: `${configService.get('MAX_BODY_SIZE_MB')}mb`,
            extended: true
        })
    );
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

    // _setupSwagger(app); --uncomment if you want to enable swagger

    await app.listen(+process.env.APP_PORT);
}

function _setupSwagger(app: INestApplication) {
    const options = new DocumentBuilder()
        .setTitle('NEISPUO Instrumentarium - Survey')
        .setDescription('NEISPUO Instrumentarium - Survey API Description')
        .setVersion('1.0')
        .addBearerAuth()
        .build();
    const document = SwaggerModule.createDocument(app, options);
    SwaggerModule.setup('api', app, document);
    fs.writeFileSync('./swagger-spec.json', JSON.stringify(document));
}

/** Docs: https://github.com/nestjsx/crud/wiki/Controllers#global-options */
function _setupCrudDefaultOptions() {
    CrudConfigService.load({
        params: {
            id: {
                field: 'id',
                type: 'number',
                primary: true
            }
        }
    });
}

bootstrap();
