import { Module } from '@nestjs/common';
import { GenderService } from './routes/gender/gender.service';
import { GenderController } from './routes/gender/gender.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { Gender } from './gender.entity';

@Module({
    imports: [TypeOrmModule.forFeature([Gender])],
    exports: [TypeOrmModule],

    controllers: [GenderController],
    providers: [GenderService]
})
export class GenderModule {}
