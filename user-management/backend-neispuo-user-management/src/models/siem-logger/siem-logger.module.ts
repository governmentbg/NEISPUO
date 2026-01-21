import { DynamicModule, Global, Module } from '@nestjs/common';
import { SIEMLoggerService } from './siem-logger.service';
import { SIEMLoggerOptions } from './siem-logger-options.interface';

@Global()
@Module({})
export class SIEMLoggerModule {
    static forRoot(options: SIEMLoggerOptions): DynamicModule {
        return {
            module: SIEMLoggerModule,
            providers: [
                {
                    provide: 'SIEM_LOGGER_OPTIONS',
                    useValue: options,
                },
                SIEMLoggerService,
            ],
            exports: [SIEMLoggerService],
        };
    }

    static forFeature(): DynamicModule {
        return {
            module: SIEMLoggerModule,
        };
    }
}
