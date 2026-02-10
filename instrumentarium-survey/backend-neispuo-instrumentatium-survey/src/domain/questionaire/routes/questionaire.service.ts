import { Injectable } from '@nestjs/common';
import { Questionaire } from '../questionaire.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class QuestionaireService extends TypeOrmCrudService<Questionaire> {
    constructor(@InjectRepository(Questionaire) repo: Repository<Questionaire>) {
        super(repo);
    }
}