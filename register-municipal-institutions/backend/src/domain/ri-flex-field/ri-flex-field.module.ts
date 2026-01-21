import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { RIFlexFieldService } from './routes/ri-flex-field/ri-flex-field.service';
import { RIFlexFieldController } from './routes/ri-flex-field/ri-flex-field.controller';
import { RIFlexField } from './ri-flex-field.entity';

@Module({
    imports: [TypeOrmModule.forFeature([RIFlexField])],
    exports: [TypeOrmModule],

    controllers: [RIFlexFieldController],
    providers: [RIFlexFieldService],
})
export class RIFlexFieldModule {}
