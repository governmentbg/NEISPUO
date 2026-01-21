import { Injectable } from '@nestjs/common';
import { Gender } from '../../gender.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class GenderService extends TypeOrmCrudService<Gender> {
    constructor(@InjectRepository(Gender) repo: Repository<Gender>) {
        super(repo);
    }
}
