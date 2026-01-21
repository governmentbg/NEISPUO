import { Injectable } from '@nestjs/common';
import { Person } from '../../person.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class PersonService extends TypeOrmCrudService<Person> {
    constructor(@InjectRepository(Person) repo: Repository<Person>) {
        super(repo);
    }
}
