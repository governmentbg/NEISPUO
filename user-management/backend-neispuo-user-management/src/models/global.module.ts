import { Global, Module } from '@nestjs/common';
import { ModelsModule } from './models.module';

@Global()
@Module({
    imports: [ModelsModule],
    exports: [ModelsModule],
})
export class GlobalModule {}
