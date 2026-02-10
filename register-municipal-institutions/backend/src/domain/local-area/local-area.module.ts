import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { LocalAreaService } from './routes/local-area/local-area.service';
import { LocalAreaController } from './routes/local-area/local-area.controller';
import { LocalArea } from './local-area.entity';

@Module({
    imports: [TypeOrmModule.forFeature([LocalArea])],
    exports: [TypeOrmModule],

    controllers: [LocalAreaController],
    providers: [LocalAreaService],
})
export class LocalAreaModule {}
