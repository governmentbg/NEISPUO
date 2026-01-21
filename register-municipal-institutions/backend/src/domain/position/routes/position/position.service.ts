import { Injectable } from '@nestjs/common';
import { Position } from '../../position.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class PositionService extends TypeOrmCrudService<Position> {
    constructor(@InjectRepository(Position) repo: Repository<Position>) {
        super(repo);
    }
}
