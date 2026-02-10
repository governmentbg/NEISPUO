import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { RIFlexFieldValueService } from './routes/ri-flex-field-value/ri-flex-field-value.service';
import { RIFlexFieldValueController } from './routes/ri-flex-field-value/ri-flex-field-value.controller';
import { RIFlexFieldValue } from './ri-flex-field-value.entity';

@Module({
    imports: [TypeOrmModule.forFeature([RIFlexFieldValue])],
    exports: [TypeOrmModule],

    controllers: [RIFlexFieldValueController],
    providers: [RIFlexFieldValueService],
})
export class RIFlexFieldValueModule {}
