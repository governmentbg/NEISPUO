import { BadRequestException, Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';
import { RIDocument } from '@domain/ri-document/ri-document.entity';
import { RIProcedureDto } from './ri-procedure.dto';
import { RIProcedure } from '../../ri-procedure.entity';

@Injectable()
export class RIProcedureService extends TypeOrmCrudService<RIProcedure> {
    constructor(@InjectRepository(RIProcedure) public repo: Repository<RIProcedure>) {
        super(repo);
    }

    public async generateOrderField(dto: RIProcedureDto, transactionManager: any) {
        const proceduresForInstitution = await transactionManager
            .getRepository(RIProcedure)
            .find({ InstitutionID: dto.InstitutionID });

        if (proceduresForInstitution.length === 0) {
            return 1;
        }

        /**
         * Maps the Ord fields for the given institution and with Math.max getting the one
         * that is with the biggest value and increment it in order to get the next value to insert in the db
         */
        const institutionOrdFields = proceduresForInstitution.map(
            (institution: RIProcedure) => institution.Ord,
        );
        const maxPreviousOrd = Math.max(...institutionOrdFields);
        const order = maxPreviousOrd + 1;

        return order;
    }

    public async createProcedure(dto: RIProcedureDto, transactionManager: any) {
        // TODO: remove this bulshit hack, when solve problem of persisting RIDocument thru RIProcedure
        const RIProcedureObj = await transactionManager.getRepository(RIProcedure).save(dto);

        const RIDocumentDto = dto.RIDocument;
        // TODO: add the validations in the dto, currently it does not taking into account the validations in the dto
        if (!RIDocumentDto.DocumentNo) {
            throw new BadRequestException("The field 'DocumentNo' shouldn't be empty");
        }
        if (!RIDocumentDto.StateNewspaperData) {
            throw new BadRequestException("The field 'StateNewspaperData' shouldn't be empty");
        }

        delete dto.RIDocument;
        RIDocumentDto.RIProcedure = RIProcedureObj;
        const RIDocumentObj = await transactionManager
            .getRepository(RIDocument)
            .save(RIDocumentDto);

        RIProcedureObj.RIDocument = RIDocumentObj;
        delete RIDocumentObj.RIProcedure;
        return RIProcedureObj;
        // return await transactionManager.getRepository(RIProcedure).save(dto);
    }
}
