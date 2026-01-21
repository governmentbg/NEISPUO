import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ParentViewEntity } from 'src/common/entities/parent-view.entity';
import { ParentCrudController } from './routing/parent-crud.controller';
import { ParentCrudService } from './routing/parent-crud.service';

@Module({
    imports: [TypeOrmModule.forFeature([ParentViewEntity])],
    controllers: [ParentCrudController],
    providers: [ParentCrudService],
    exports: [ParentCrudService],
})
export class ParentModule {}
