import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { TownService } from './routes/town/town.service';
import { TownController } from './routes/town/town.controller';
import { Town } from './town.entity';

@Module({
    imports: [TypeOrmModule.forFeature([Town])],
    exports: [TypeOrmModule],

    controllers: [TownController],
    providers: [TownService],
})
export class TownModule {}
