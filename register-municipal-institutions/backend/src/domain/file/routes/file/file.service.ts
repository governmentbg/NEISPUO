import { Injectable } from '@nestjs/common';
import { File } from '../../file.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class FileService extends TypeOrmCrudService<File> {
    constructor(@InjectRepository(File) public repo: Repository<File>) {
        super(repo);
    }
}
