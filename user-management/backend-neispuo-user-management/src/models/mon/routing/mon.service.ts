import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { MonRepository } from '../mon.repository';

@Injectable()
export class MonService {
    constructor(@InjectRepository(MonRepository) private мonRepository: MonRepository) {}

    async getAllMon() {
        return this.мonRepository.getAllMons();
    }
}
