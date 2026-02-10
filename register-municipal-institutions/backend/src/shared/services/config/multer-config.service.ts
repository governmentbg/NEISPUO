import { Injectable } from '@nestjs/common';
import { MulterOptionsFactory, MulterModuleOptions } from '@nestjs/platform-express';
import { diskStorage } from 'multer';
import { ConfigService } from './config.service';

@Injectable()
export class MulterConfigService implements MulterOptionsFactory {
    constructor(private configService: ConfigService) {}

    createMulterOptions(): MulterModuleOptions {
        return {
            storage: diskStorage({
                destination: process.env.FILE_PATH,
            }),
        };
    }
}
