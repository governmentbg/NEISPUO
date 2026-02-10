import { Module } from '@nestjs/common';
import { FileService } from './routes/file/file.service';
import { FileController } from './routes/file/file.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { File } from './file.entity';
import { SharedModule } from '../../shared/shared.module';

@Module({
    imports: [TypeOrmModule.forFeature([File]), SharedModule],
    exports: [TypeOrmModule],

    controllers: [FileController],
    providers: [FileService]
})
export class FileModule {}
