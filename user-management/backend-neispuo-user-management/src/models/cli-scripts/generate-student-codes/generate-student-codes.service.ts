import { Injectable } from '@nestjs/common';
import { Filter } from 'src/common/dto/filter.dto';
import { Paging } from 'src/common/dto/paging.dto';
import { SchoolBookCodeService } from 'src/models/school-book-code/routing/school-book-code.service';
import { GenerateStudentCodesRepository } from './generate-student-codes.repository';

@Injectable()
export class GenerateStudentCodesService {
    constructor(
        private generateStudentCodesRepository: GenerateStudentCodesRepository,
        private schoolBookCode: SchoolBookCodeService,
    ) {}

    async getAllStudentsAndCount(paging: Paging, filters: Filter[]) {
        const count = await this.generateStudentCodesRepository.getAllStudentsCount(filters);
        const result = await this.generateStudentCodesRepository.getAllStudents(paging, filters);
        return { count, data: result };
    }

    async getAllStudents(paging: Paging, filters: Filter[]) {
        const result = await this.generateStudentCodesRepository.getAllStudents(paging, filters);
        return { data: result };
    }

    async getAllStudentsCount(filters: Filter[]) {
        const count = await this.generateStudentCodesRepository.getAllStudentsCount(filters);
        return { count };
    }

    // async assignSchoolBookCodesToAllStudents(request: AuthedRequest) {
    //     const currentPage: Paging = {
    //         from: 0,
    //         numberOfElements: CONSTANTS.DB_QUERY_PARAM_ROWS_PER_PAGE_STUDENTS,
    //     };
    //     const studentsCount = await this.getAllStudentsCount([]); // with no filters we want all the kids
    //     const totalCountofStudents = studentsCount.count;
    //     while (totalCountofStudents > currentPage.from) {
    //         currentPage.from += currentPage.numberOfElements; // increments the from value
    //         let currentBatchOfStudents;
    //         try {
    //             currentBatchOfStudents = await this.getAllStudents(currentPage, []);
    //         } catch (e) {
    //             currentPage.from -= currentPage.numberOfElements;
    //             console.log(e);
    //             continue;
    //         } // with no filters we want all the kids
    //         const personIDs = currentBatchOfStudents.data.map((student) => student.personID);
    //         try {
    //             await this.schoolBookCode.assignSchoolBookCodes({ personIDs });
    //             console.log(`Generated code for ${personIDs}`);
    //         } catch (e) {
    //             currentPage.from -= currentPage.numberOfElements;
    //             console.log(e);
    //             continue;
    //         }
    //     }
    // }
}
