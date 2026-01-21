import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Connection, EntityManager, Repository } from 'typeorm';
import { Town } from '@domain/town/town.entity';
import { InstitutionDto } from './institution.dto';
import { Institution } from '../../institution.entity';

@Injectable()
export class InstitutionService extends TypeOrmCrudService<Institution> {
    constructor(
        public connection: Connection,
        @InjectRepository(Institution) public repo: Repository<Institution>,
        @InjectRepository(Town) public townRepo: Repository<Town>,
    ) {
        super(repo);
        // const entityManager = connection.manager;
    }

    public async createInstitution(dto: InstitutionDto, transactionManager: any) {
        dto.InstitutionID = await this.generateInstitutionID(dto.Town);
        const repo = transactionManager.getRepository(Institution);
        return repo.save(dto);
    }

    public updateInstitution(
        dto: InstitutionDto,
        transactionManager: EntityManager,
    ): Promise<Institution> {
        return transactionManager.getRepository(Institution).save(dto);
    }

    private async generateInstitutionID(town: any): Promise<number> {
        const townObj = await this.connection.manager.findOne(Town, town, {
            relations: ['Municipality'],
        });
        let regionId = townObj.Municipality.Region.RegionID;
        if (regionId === 3) {
            regionId = 4;
        } else if (regionId === 4) {
            regionId = 3;
        }
        const minId = regionId * 100000;
        const maxId = regionId * 100000 + 99999;

        const list = await this.connection.manager
            .createQueryBuilder()
            .select('Institution')
            .from(Institution, 'Institution')
            .where('Institution.InstitutionID BETWEEN :idMin AND :idMax', {
                idMin: minId,
                idMax: maxId,
            })
            .orderBy('InstitutionID')
            .getMany();

        let possible: number;
        for (let i = minId; i < maxId; i += 1) {
            const result = list.find((obj) => obj.InstitutionID === i);
            if (result === undefined) {
                possible = i;
                break;
            }
        }

        return possible;
    }

    private validatePersonalId(personalId: string) {
        if (personalId.length !== 10) {
            return false;
        }

        let sumOfCalculation = 0;
        const multipliers = [2, 4, 8, 5, 10, 9, 7, 3, 6];

        // Calculate the sum by the formula: multiplier[1]*а1 + multiplier[2]*а2 ... + multiplier[9]*a9
        for (let index = 0; index < 9; index++) {
            sumOfCalculation += multipliers[index] * +personalId.charAt(index);
        }

        // If the calculated sum's remainder is 10, the key will be assigned as 0, otherwise as the remainder.
        const key = sumOfCalculation % 11 !== 10 ? sumOfCalculation % 11 : 0;
        return key === +personalId.charAt(9);
    }

    private validateBulstatNineDigit(bulstat: string) {
        let sumOfCalculation = 0;
        let key = 0;
        // Calculate the sum by the formula: 1*а1 + 2*а2 .. + 8*а8
        for (let index = 0; index < 8; index++) {
            sumOfCalculation += (index + 1) * +bulstat.charAt(index);
        }
        // If the remainder of the sum and 11 is not 10, the key should be quall to the 9 digit of the bulstat
        key = sumOfCalculation % 11;
        if (key !== 10) {
            return key === +bulstat.charAt(8);
        }
        // If the remainder is 10, calculate him again with formula: 3*а1 + 4*а2 ... + 10*a8
        sumOfCalculation = 0;
        for (let index = 0; index < 8; index++) {
            sumOfCalculation += (index + 3) * +bulstat.charAt(index);
        }
        // If the newly calculated sum's remainder is 10, the key will be assigned as 0.
        key = sumOfCalculation % 11 !== 10 ? sumOfCalculation % 11 : 0;
        // Key should be equall to the 9 digit of the bulstat
        return key === +bulstat.charAt(8);
    }

    private validateBulstatThirteenDigit(bulstat: string) {
        // First 9 digits should be valid
        if (!this.validateBulstatNineDigit(bulstat.slice(0, 9))) {
            return false;
        }
        // Calculate the sum by the formula: 2*а9 + 7*а10 + 3*а11 + 5*a12
        let sumOfCalculation =
            2 * +bulstat.charAt(8) +
            7 * +bulstat.charAt(9) +
            3 * +bulstat.charAt(10) +
            5 * +bulstat.charAt(11);
        // If the remainder of the division between the sum and 11, is not 10, the remainder should be the 13 digit
        let key = sumOfCalculation % 11;
        if (key !== 10) {
            return key === +bulstat.charAt(12);
        }
        // If the remainder is 10, calculate him again with formula: 4*а9 + 9*а10 + 5*а11 + 7*a12
        sumOfCalculation = 0;
        sumOfCalculation =
            4 * +bulstat.charAt(8) +
            9 * +bulstat.charAt(9) +
            5 * +bulstat.charAt(10) +
            7 * +bulstat.charAt(11);
        // If the newly calculated sum's remainder is 10, the key will be assigned as 0.
        key = sumOfCalculation % 11 !== 10 ? sumOfCalculation % 11 : 0;
        // Key should be equall to the 13 digit of the bulstat.
        return key === +bulstat.charAt(12);
    }

    public validateBulstat(bulstat: string) {
        if (bulstat.length === 9) {
            return this.validateBulstatNineDigit(bulstat);
        } else if (bulstat.length === 10) {
            return this.validatePersonalId(bulstat);
        } else if (bulstat.length === 13) {
            return this.validateBulstatThirteenDigit(bulstat);
        }
        return false;
    }
}
