import { Module } from '@nestjs/common';
import { PersonalIDTypeService } from './routes/personal-id-type/personal-id-type.service';
import { PersonalIDTypeController } from './routes/personal-id-type/personal-id-type.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { PersonalIDType } from './personal-id-type.entity';

@Module({
    imports: [TypeOrmModule.forFeature([PersonalIDType])],
    exports: [TypeOrmModule],

    controllers: [PersonalIDTypeController],
    providers: [PersonalIDTypeService]
})
export class PersonalIDTypeModule {}
