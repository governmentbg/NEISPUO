import { Module } from '@nestjs/common';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ClassBookEntity } from 'src/common/entities/class-book.entity';
import { ClassBooksView } from 'src/common/entities/class-books.view.entity';
import { SchoolBooksController } from './routing/school-books.controller';
import { SchoolBooksService } from './routing/school-books.service';

@Module({
    imports: [TypeOrmModule.forFeature([ClassBooksView, ClassBookEntity])],
    providers: [SchoolBooksService],
    controllers: [SchoolBooksController],
    exports: [SchoolBooksService],
})
export class SchoolBooksModule {}
