import { Injectable } from '@nestjs/common';
import { Country } from '../../country.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class CountryService extends TypeOrmCrudService<Country> {
    constructor(@InjectRepository(Country) repo: Repository<Country>) {
        super(repo);
    }
}
