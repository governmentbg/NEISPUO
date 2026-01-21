import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { RUORepository } from '../ruo.repository';

@Injectable()
export class RuoService {
    constructor(@InjectRepository(RUORepository) private ruoRepository: RUORepository) {}

    async getAllRUO() {
        return this.ruoRepository.getAllRUOs();
    }
}
