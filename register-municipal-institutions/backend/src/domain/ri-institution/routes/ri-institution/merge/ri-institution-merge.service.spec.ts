import { Institution } from '@domain/institution/institution.entity';
import { ProcedureType } from '@domain/procedure-type/procedure-type.entity';
import { RIFlexFieldValue } from '@domain/ri-flex-field-value/ri-flex-field-value.entity';
import { AuditService } from '@domain/ri-institution/audit/audit.service';
import { RIInstitution } from '@domain/ri-institution/ri-institution.entity';
import { RIProcedureService } from '@domain/ri-procedure/routes/ri-procedure/ri-procedure.service';
import { StatusType } from '@domain/status-type/status-type.entity';
import { TransformType } from '@domain/transform-type/transform-type.entity';
import { Test, TestingModule } from '@nestjs/testing';
import { getConnectionToken, getRepositoryToken } from '@nestjs/typeorm';
import { RIInstitutionService } from '../ri-institution.service';
import { RIInstitutionMergeService } from './ri-institution-merge.service';

jest.mock('@nestjsx/crud-typeorm', () => ({
    TypeOrmCrudService: class {
        constructor() { }
    },
}));

describe('Ri-institutionMergeService', () => {
    let service: RIInstitutionMergeService;

    const mockAuditService = {
        insertAudit: jest.fn(),
    };

    const mockRIInstitutionService = {
        generateInstitutionID: jest.fn().mockResolvedValue('institution-id'),
        azureSyncCreateSchool: jest.fn(),
        azureSyncDeleteSchool: jest.fn(),
        deleteProcedures: jest.fn(),
    };

    const mockProcedureService = {
        createProcedure: jest.fn().mockResolvedValue({
            InstitutionID: 'procedure-institution-id',
        }),
    };

    const mockRIInstitutionRepository = {
        save: jest.fn().mockResolvedValue({
            Name: 'RiInstitution-name',
            Abbreviation: 'Abbreviation',
        }),
        connection: {},
    };

    const mockTransformTypeRepository = {
        findOne: jest.fn().mockResolvedValue({
            TransformTypeID: 5,
            Name: 'откриване/вписване',
            PublicName: 'действаща',
            IsNEISPUOActive: true,
            IsNEISPUOData: false,
            IsNEISPUOAccessDenied: false,
            IsValid: true,
            ValidFrom: '2020-11-18T22:00:00.000Z',
            ValidTo: null,
        }),
    };

    const mockProcedureTypeRepository = {
        findOne: jest.fn().mockResolvedValue({
            ProcedureTypeID: 1,
            Name: 'Откриване/ вписване на институция',
            Description: null,
            IsValid: true,
            ValidFrom: '2020-11-18T22:00:00.000Z',
            ValidTo: null,
        }),
    };

    const mockStatusTypeRepository = {
        findOne: jest.fn().mockResolvedValue({
            StatusTypeID: 4,
            Name: 'Приключена, потвърдена',
            Description: null,
            IsValid: true,
            ValidFrom: '2020-11-18T22:00:00.000Z',
            ValidTo: null,
        }),
    };

    const mockRiFlexFieldRepository = {
        save: jest.fn(),
    };

    const authedRequest = {
        get: jest.fn().mockReturnValue('Authorization Bearer'),
    };

    beforeEach(async () => {
        const repositories = {
            [getRepositoryToken(TransformType)]: mockTransformTypeRepository,
            [getRepositoryToken(ProcedureType)]: mockProcedureTypeRepository,
            [getRepositoryToken(StatusType)]: mockStatusTypeRepository,
            [getRepositoryToken(RIInstitution)]: mockRIInstitutionRepository,
            [getRepositoryToken(Institution)]: mockRIInstitutionRepository,
            [getRepositoryToken(RIFlexFieldValue)]: mockRiFlexFieldRepository,
        };

        const mockTransactionManager = {
            getRepository(Class: any) {
                return repositories[getRepositoryToken(Class)];
            },
        };

        const mockConnection = {
            transaction: (cb: Function) => cb(mockTransactionManager),
        };
        const module: TestingModule = await Test.createTestingModule({

            providers: [
                { provide: RIInstitutionService, useValue: mockRIInstitutionService },
                { provide: getConnectionToken(), useValue: mockConnection },
                { provide: RIProcedureService, useValue: mockProcedureService },
                { provide: AuditService, useValue: mockAuditService },
                { provide: getRepositoryToken(RIInstitution), useValue: mockRIInstitutionRepository },
                RIInstitutionMergeService,
            ],

        }).compile();
        service = module.get<RIInstitutionMergeService>(RIInstitutionMergeService);
    });

    it('should successfully merge an institutions', async () => {
        const body = {
            miToCreate: {
                RIProcedure: {
                    __name__: 'RIProcedureFromBody',
                },
                Town: 'Lazy Town',
                RIFlexFieldValues: [
                    { value: 'flex1' },
                    { value: 'flex2' },
                ],
            },
misToDelete: [{
                RIProcedure: {
                    __name__: 'RIProcedureToDelete',
                },
                Town: 'DeleteTown',
                RIFlexFieldValues: [
                    { value: 'flex3' },
                    { value: 'flex4' },
                ],
            }],
        };

        const result = await service.mergeInstitutions(authedRequest as any, body);
        expect(mockTransformTypeRepository.findOne).toHaveBeenCalledWith(12);
        expect(mockTransformTypeRepository.findOne).toHaveBeenCalledTimes(2);
        expect(mockProcedureTypeRepository.findOne).toHaveBeenCalledWith(3);
        expect(mockProcedureTypeRepository.findOne).toHaveBeenCalledTimes(2);
        expect(mockStatusTypeRepository.findOne).toHaveBeenCalledWith(4);
        expect(mockStatusTypeRepository.findOne).toHaveBeenCalledTimes(2);
        expect(mockRIInstitutionService.deleteProcedures).toHaveBeenCalledTimes(1);
        expect(mockRIInstitutionService.generateInstitutionID).toHaveBeenCalledWith(body.miToCreate.Town);
        expect(mockProcedureService.createProcedure).toHaveBeenCalledTimes(1);
        expect(mockProcedureService.createProcedure.mock.calls[0]).toMatchSnapshot();
        expect(mockRiFlexFieldRepository.save).toHaveBeenCalledWith({
            RIFlexFieldValueID: null,
            RIInstitution: { Abbreviation: 'Abbreviation', Name: 'RiInstitution-name' },
value: 'flex1',
        });
        expect(mockAuditService.insertAudit).toHaveBeenCalled();
        expect(mockRIInstitutionService.azureSyncCreateSchool).toHaveBeenCalledWith({
            Abbreviation: 'Abbreviation',
            Name: 'RiInstitution-name',
        }, 'Authorization Bearer');
        expect(mockRIInstitutionService.azureSyncDeleteSchool).toHaveBeenCalledWith(
            ...body.misToDelete,
         'Authorization Bearer',
);

        expect(result).toEqual({
            Abbreviation: 'Abbreviation',
            Name: 'RiInstitution-name',
        });
    });
});
