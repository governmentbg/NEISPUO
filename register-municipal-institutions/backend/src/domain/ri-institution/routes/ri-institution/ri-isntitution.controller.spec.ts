import { Test, TestingModule } from '@nestjs/testing';
import { getConnectionToken, getRepositoryToken } from '@nestjs/typeorm';
import { RIProcedureService } from '@domain/ri-procedure/routes/ri-procedure/ri-procedure.service';
import { AuditService } from '@domain/ri-institution/audit/audit.service';
import { TransformType } from '@domain/transform-type/transform-type.entity';
import { ProcedureType } from '@domain/procedure-type/procedure-type.entity';
import { StatusType } from '@domain/status-type/status-type.entity';
import { Institution } from '@domain/institution/institution.entity';
import { InstitutionService } from '@domain/institution/routes/institution/institution.service';
import { RIFlexFieldValue } from '@domain/ri-flex-field-value/ri-flex-field-value.entity';
import { RIProcedure } from '@domain/ri-procedure/ri-procedure.entity';
import { RIPremInstitution } from '@domain/ri-prem-institution/ri-prem-institution.entity';
import { RIInstitutionService } from './ri-institution.service';
import { RIInstitutionController } from './ri-institution.controller';
import { RIInstitution } from '../../ri-institution.entity';

describe('Ri-institutionController', () => {
    let controller: RIInstitutionController;

    const mockAuditService = {
        insertAudit: jest.fn(),
    };

    const mockRIInstitutionService = {
        generateInstitutionID: jest.fn().mockReturnValue('institution-id'),
        azureSyncCreateSchool: jest.fn(),
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
    };

    const mockRIProcedureRepository = {
        find: jest.fn().mockResolvedValue([{
            RIProcedureID: 14688,
            InstitutionID: 100064,
            TransformDetails: null,
            YearDue: 2009,
        }]),
        save: jest.fn().mockResolvedValue({
            RIProcedureID: 14688,
            InstitutionID: 100064,
            TransformDetails: null,
            YearDue: 2009,
            isActive: false,
        }),
    };

    const mockRIPremInstitutonRepository = {
        save: jest.fn().mockResolvedValue({
            PremInstitutionID: 2,
            PremDocs: 'documents',
            PremStudents: 'students',
            PremInventory: 'inventory',
            RIPremInstitutionID: null,
            InstitutionID: 100064,
        }),
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

    const mockInstitutionRepository = {
        save: jest.fn(),
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
            [getRepositoryToken(Institution)]: mockInstitutionRepository,
            [getRepositoryToken(RIFlexFieldValue)]: mockRiFlexFieldRepository,
            [getRepositoryToken(RIProcedure)]: mockRIProcedureRepository,
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
            controllers: [RIInstitutionController],
            providers: [
                { provide: RIInstitutionService, useValue: mockRIInstitutionService },
                { provide: RIProcedureService, useValue: mockProcedureService },
                { provide: AuditService, useValue: mockAuditService },
                { provide: InstitutionService, useValue: mockInstitutionRepository },
                { provide: getConnectionToken(), useValue: mockConnection },
            ],
        }).compile();
        controller = module.get<RIInstitutionController>(RIInstitutionController);
    });

    it('should successfully create institution', async () => {
        const body = {
            RIProcedure: {
                __name__: 'RIProcedureFromBody',
            },
            Town: 'Lazy Town',
            RIFlexFieldValues: [
                { value: 'flex1' },
                { value: 'flex2' },
            ],
        };

        const result = await controller.createOne(authedRequest as any, {} as any, body);
        expect(mockRIInstitutionService.generateInstitutionID).toHaveBeenCalledWith(body.Town);
        expect(mockTransformTypeRepository.findOne).toHaveBeenCalledWith(5);
        expect(mockProcedureTypeRepository.findOne).toHaveBeenCalledWith(1);
        expect(mockStatusTypeRepository.findOne).toHaveBeenCalledWith(4);
        expect(mockProcedureService.createProcedure).toHaveBeenCalledTimes(1);
        expect(mockProcedureService.createProcedure.mock.calls[0]).toMatchSnapshot();

        expect(mockRIInstitutionRepository.save).toHaveBeenCalledWith({
            RIProcedure: { InstitutionID: 'procedure-institution-id' },
            Town: 'Lazy Town',
            RIFlexFieldValues: [{ value: 'flex1' }, { value: 'flex2' }],
            InstitutionID: 'procedure-institution-id',
        });
        expect(mockRiFlexFieldRepository.save).toHaveBeenCalledWith(
            { RIInstitution: { Abbreviation: 'Abbreviation', Name: 'RiInstitution-name' }, value: 'flex1' },
        );
        expect(mockInstitutionRepository.save).toHaveBeenCalledWith({
            Abbreviation: 'Abbreviation',
            Name: 'RiInstitution-name',
        });
        expect(mockRIInstitutionService.azureSyncCreateSchool).toHaveBeenCalledWith({
            Abbreviation: 'Abbreviation',
            Name: 'RiInstitution-name',
        }, 'Authorization Bearer');
        expect(mockAuditService.insertAudit).toHaveBeenCalled();
        expect(result).toEqual({
            Abbreviation: 'Abbreviation',
            Name: 'RiInstitution-name',
        });
    });

    it('should successfully delete institution', async () => {
        const body = {
            RIProcedure: {
                __name__: 'RIProcedureFromBody',
            },
            Town: 'Lazy Town',
            RIFlexFieldValues: [
                { value: 'flex1' },
                { value: 'flex2' },
            ],
            InstitutionID: 100064,
        };
        const result = await controller.deleteOne(authedRequest as any, {} as any, {} as any, body);
        expect(mockTransformTypeRepository.findOne).toHaveBeenCalledWith(1);
        expect(mockProcedureTypeRepository.findOne).toHaveBeenCalledWith(3);
        expect(mockStatusTypeRepository.findOne).toHaveBeenCalledWith(4);
        expect(mockRIProcedureRepository.find).toHaveBeenCalledWith({ InstitutionID: body.InstitutionID });
        expect(mockRIProcedureRepository.save).toHaveBeenCalledWith({
            InstitutionID: body.InstitutionID,
            IsActive: false,
            RIProcedureID: 14688,
            TransformDetails: null,
            YearDue: 2009,
        });
        expect(mockProcedureService.createProcedure).toHaveBeenCalledTimes(2);
        expect(mockProcedureService.createProcedure.mock.calls[1]).toMatchSnapshot();
        expect(mockRIInstitutionRepository.save).toHaveBeenCalledWith({
            RIProcedure: { InstitutionID: 'procedure-institution-id' },
            RIInstitutionID: null,
            InstitutionID: 'procedure-institution-id',
            Town: 'Lazy Town',
            RIFlexFieldValues: [{ value: 'flex1' }, { value: 'flex2' }],
        });
        expect(mockRiFlexFieldRepository.save).toHaveBeenCalledTimes(4);
        expect(mockRIPremInstitutonRepository.save).toHaveBeenCalledWith({
            RIPremInstitutionID: null,
            RIProcedure: { InstitutionID: 'procedure-institution-id' },
        });
        expect(mockAuditService.insertAudit).toHaveBeenCalled();
        expect(mockRIInstitutionService.azureSyncDeleteSchool).toHaveBeenCalledWith({
            Abbreviation: 'Abbreviation',
            Name: 'RiInstitution-name',
        }, 'Authorization Bearer');
        expect(result).toEqual({
            Abbreviation: 'Abbreviation',
            Name: 'RiInstitution-name',
        });
    });
});
