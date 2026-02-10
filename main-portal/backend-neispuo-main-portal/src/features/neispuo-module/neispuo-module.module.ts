import { Module } from '@nestjs/common';
import { NesipuoModuleService } from './routes/neispuo-module.service';
import { NesipuoModuleController } from './routes/neispuo-module.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { NeispuoModule as NeispuoModule } from './neispuo-module.entity';

@Module({
    imports: [TypeOrmModule.forFeature([NeispuoModule])],
    controllers: [],
    providers: [NesipuoModuleService],
    exports: [NesipuoModuleService]
})
export class NeispuoModuleModule { }
