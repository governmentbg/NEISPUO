import { Module } from '@nestjs/common';
import { NesipuoCategoryService } from './routes/neispuo-category.service';
import { NeispuoCategory } from './neispuo-category.entity'
import { TypeOrmModule } from '@nestjs/typeorm';
import { NesipuoCategoryController } from './routes/neispuo-category.controller';

@Module({
    imports: [TypeOrmModule.forFeature([NeispuoCategory])],
    controllers: [NesipuoCategoryController],
    providers: [NesipuoCategoryService],
    exports: [NesipuoCategoryService]
})
export class NesipuoCategoryModule { }
