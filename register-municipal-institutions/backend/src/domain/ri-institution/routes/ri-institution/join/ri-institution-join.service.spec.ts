import { Institution } from '@domain/institution/institution.entity';
import { ProcedureType } from '@domain/procedure-type/procedure-type.entity';
import { RIFlexFieldValue } from '@domain/ri-flex-field-value/ri-flex-field-value.entity';
import { AuditService } from '@domain/ri-institution/audit/audit.service';
import { RIInstitution } from '@domain/ri-institution/ri-institution.entity';
import { RIPremInstitution } from '@domain/ri-prem-institution/ri-prem-institution.entity';
import { RIProcedure } from '@domain/ri-procedure/ri-procedure.entity';
import { RIProcedureService } from '@domain/ri-procedure/routes/ri-procedure/ri-procedure.service';
import { StatusType } from '@domain/status-type/status-type.entity';
import { TransformType } from '@domain/transform-type/transform-type.entity';
import { Test, TestingModule } from '@nestjs/testing';
import { getConnectionToken, getRepositoryToken } from '@nestjs/typeorm';
import { RIInstitutionService } from '../ri-institution.service';
import { RIInstitutionJoinService } from './ri-institution-join.service';

jest.mock('@nestjsx/crud-typeorm', () => ({
    TypeOrmCrudService: class {
        constructor() { }
    },
}));

describe('Ri-institutionJoinService', () => {
    let service: RIInstitutionJoinService;

    const mockAuditService = {
        insertAudit: jest.fn(),
    };

    const mockRIInstitutionService = {
        azureSyncDeleteSchool: jest.fn(),
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
    const mockRIProceduresRepository = {
        find: jest.fn().mockResolvedValue([{
            RIProcedure: {
                __name__: 'RIProcedureFromBody',
            },
            Town: 'Lazy Town',
        }]),
        save: jest.fn(),
    };

    const mockRIPremInstitutonRepository = {
        save: jest.fn(),
    };

    beforeEach(async () => {
        const repositories = {
            [getRepositoryToken(TransformType)]: mockTransformTypeRepository,
            [getRepositoryToken(ProcedureType)]: mockProcedureTypeRepository,
            [getRepositoryToken(StatusType)]: mockStatusTypeRepository,
            [getRepositoryToken(RIInstitution)]: mockRIInstitutionRepository,
            [getRepositoryToken(Institution)]: mockRIInstitutionRepository,
            [getRepositoryToken(RIFlexFieldValue)]: mockRiFlexFieldRepository,
            [getRepositoryToken(RIProcedure)]: mockRIProceduresRepository,
            [getRepositoryToken(RIPremInstitution)]: mockRIPremInstitutonRepository,
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
                RIInstitutionJoinService,
            ],

        }).compile();
        service = module.get<RIInstitutionJoinService>(RIInstitutionJoinService);
    });

    it('should successfully join an institutions', async () => {
        const body = {
            miToUpdate: {
                RIProcedure: {
                    InstitutionID: 0,
                    __name__: 'RIProcedureFromBody',
                },
                Town: 'Lazy Town',
                RIFlexFieldValues: [
                    { value: 'flex1' },
                    { value: 'flex2' },
                ],
            },
misToDelete: [{
                InstitutionID: 1,
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

        const result = await service.joinInstitutions(authedRequest as any, body);
        expect(mockTransformTypeRepository.findOne).toHaveBeenCalledWith(11);
        expect(mockProcedureTypeRepository.findOne).toHaveBeenCalledWith(3);
        expect(mockStatusTypeRepository.findOne).toHaveBeenCalledWith(4);
        expect(mockAuditService.insertAudit).toHaveBeenCalledTimes(2);
        expect(mockRIProceduresRepository.find).toHaveBeenCalledWith({ InstitutionID: body.misToDelete[0].InstitutionID });
        expect(mockProcedureService.createProcedure).toHaveBeenCalledTimes(1);
        expect(mockProcedureService.createProcedure.mock.calls[0]).toMatchSnapshot();
        expect(mockRiFlexFieldRepository.save).toBeCalledTimes(2);
        expect(mockRIPremInstitutonRepository.save).toHaveBeenCalledWith({
            RIPremInstitutionID: null,
            RIProcedure: {
                InstitutionID: 'procedure-institution-id',
            },
        });
        expect(mockRIInstitutionService.azureSyncDeleteSchool).toHaveBeenCalledWith(
            body.miToUpdate,
            'Authorization Bearer',
);

        expect(result).toEqual(
            body.miToUpdate,
        );
    });
});
