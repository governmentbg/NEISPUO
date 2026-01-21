import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { MunicipalityRepository } from '../municipality.repository';

@Injectable()
export class MunicipalityService {
    constructor(@InjectRepository(MunicipalityRepository) private municipalityRepository: MunicipalityRepository) {}

    async getAllMunicipalities() {
        return this.municipalityRepository.getAllMunicipality();
    }
}
