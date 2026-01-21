import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { CurrentYearType } from './current-year.entity';
import { CurrentYearController } from './routes/current-year.controller';
import { CurrentYearService } from './routes/current-year.service';

@Module({
    imports: [TypeOrmModule.forFeature([CurrentYearType])],
    exports: [TypeOrmModule],

    controllers: [CurrentYearController],
    providers: [CurrentYearService],
})
export class CurrentYearModule {}
