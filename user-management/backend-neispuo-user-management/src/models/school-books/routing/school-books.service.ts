import { ForbiddenException, Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { PositionEnum } from 'src/common/constants/enum/position.enum';
import { ClassBooksView } from 'src/common/entities/class-books.view.entity';
import { EducationalStateService } from 'src/models/educational-state/routing/educational-state.service';
import { Repository } from 'typeorm';

@Injectable()
export class SchoolBooksService {
    constructor(
        @InjectRepository(ClassBooksView) private readonly repo: Repository<ClassBooksView>,
        private educationalStateService: EducationalStateService,
    ) {}

    async getSchoolBooksByInstitutionID(institutionID: number, personID: number) {
        await this.checkPersonInstitutionRelation(personID, institutionID);
        const schoolbooks = await this.repo.find({
            where: { instId: institutionID },
            order: {
                schoolYear: 'DESC',
                fullBookName: 'ASC',
            },
        });
        return schoolbooks;
    }

    async checkPersonInstitutionRelation(personID: number, institutionID: number) {
        const eduStates = await this.educationalStateService.getUserEducationalStatesByPersonID({
            personID,
        });
        const personInstitutions = eduStates.map((eduState) => eduState.institutionID);
        const currentEduState = eduStates.find((eduState) => eduState.institutionID === institutionID);
        if (
            !personInstitutions.includes(institutionID) ||
            (personInstitutions.includes(institutionID) && currentEduState.positionID !== PositionEnum.EMPLOYEE)
        ) {
            throw new ForbiddenException();
        }
    }
}
