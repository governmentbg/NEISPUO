import { Test, TestingModule } from '@nestjs/testing';
import { CONSTANTS } from 'src/common/constants/constants';
import { EventStatus } from 'src/common/constants/enum/event-status.enum';
import { ClassRepository } from 'src/models/class/class.repository';
import { ClassService } from 'src/models/class/routing/class.service';
import { TelelinkService } from 'src/models/telelink/routing/telelink.service';
import { Connection } from 'typeorm';
import { AzureEnrollmentsService } from '../../azure-enrollments/routing/azure-enrollments.service';
import { AzureClassesRepository } from '../azure-classes.repository';
import { AzureClassesService } from '../routing/azure-classes.service';

export type MockType<T> = {
    [P in keyof T]?: jest.Mock<any>;
};

describe('AzureClassesService', () => {
    let service: AzureClassesService;
    let repository: AzureClassesRepository;
    let classesRepository: ClassRepository;

    const mockAzureClass = {
        id: 10,
        rowID: 100,
        status: EventStatus.SUCCESSFUL,
    };
    const mockRepository = {
        insertAzureClass: jest.fn((entity) => entity),
        updateAzureClass: jest.fn((entity) => entity),
        deleteAzureClass: jest.fn((entity) => entity),
        getAzureClassStatus: jest.fn((entity) => entity),
        getCurriculumByCurriculumID: jest.fn((entity) => entity),
        restartWorkflow: jest.fn((entity) => entity),
    };
    const mockClassRepository = {
        getCurriculumByCurriculumID: jest.fn((entity) => entity),
    };
    const mockClassService = {
        getCurriculumByCurriculumID: jest.fn((entity) => entity),
        generateClassTitle: jest.fn((entity) => entity),
        generateClassesDTOFromSubjects: jest.fn((entity) => entity),
    };
    const mockAzureEnrollmentsService = {
        getPersonByPersonID: jest.fn((entity) => entity),
        createAzureEnrollmentUserToClass: jest.fn((entity) => entity),
        deleteAzureEnrollmentUserToClass: jest.fn((entity) => entity),
    };
    const mockConnection = {};

    beforeEach(async () => {
        const module: TestingModule = await Test.createTestingModule({
            providers: [
                { provide: AzureClassesRepository, useValue: mockRepository },
                { provide: ClassRepository, useValue: mockClassRepository },
                {
                    provide: TelelinkService,
                    useValue: {},
                },
                {
                    provide: ClassService,
                    useValue: mockClassService,
                },
                {
                    provide: AzureEnrollmentsService,
                    useValue: mockAzureEnrollmentsService,
                },
                {
                    provide: Connection,
                    useValue: mockConnection,
                },
                AzureClassesService,
            ],
        }).compile();
        service = module.get<AzureClassesService>(AzureClassesService);
        repository = module.get<AzureClassesRepository>(AzureClassesRepository);
        classesRepository = module.get<ClassRepository>(ClassRepository);
    });

    describe('createAzureClassAndEnrollUsers', () => {
        // it('should successfully insert azure class', async () => {
        //     const request = {} as Request;
        //     let dto = {} as ClassCreateRequestDTO;
        //     mockClassService.getSubjectByCurriculumID.mockReturnValue([{ curriculumID: 3 }]);
        //     mockClassService.generateClassTitle.mockReturnValue('someClass');
        //     mockRepository.insertAzureClass.mockReturnValue([{ rowID: 100 }]);
        //     dto = { curriculumID: 3, personIDs: [1, 2, 3], institutionID: 1 };
        //     const repositoryFuncSpy = jest.spyOn(repository, 'insertAzureClass');
        //     mockAzureEnrollmentsService.createAzureEnrollmentUserToClass.mockReturnValue({ data: 100 });
        //     const resultClass = await service.createAzureClassesAndEnrollUsers(request, dto);
        //     expect(resultClass).toEqual({
        //         data: {
        //             [CONSTANTS.RESPONSE_PARAM_NAME_CLASS_CREATED]: 100,
        //             [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: [100, 100, 100],
        //         },
        //     });
        //     expect(repositoryFuncSpy).toBeCalled();
        // });
        // it('should fail to insert azure class', async () => {
        //     const request = {} as Request;
        //     const dto = {} as ClassCreateRequestDTO;
        //     mockClassService.getSubjectByCurriculumID.mockReturnValue(null);
        //     await expect(service.createAzureClassesAndEnrollUsers(request, dto)).rejects.toThrow(
        //         new DataNotFoundException(),
        //     );
        // });
    });

    describe('updateAzureClass', () => {
        // it('should successfully update azure class', async () => {
        //     const request = {} as Request;
        //     let dto = {} as ClassUpdateRequestDTO;
        //     mockClassService.getSubjectByCurriculumID.mockReturnValue([{ curriculumID: 3 }]);
        //     mockClassService.generateClassTitle.mockReturnValue('someClass');
        //     mockRepository.updateAzureClass.mockReturnValue([{ rowID: 100 }]);
        //     dto = { curriculumID: 3, personIDsToDelete: [1, 2, 3], personIDsToCreate: [1, 2, 3], institutionID: 1 };
        //     const repositoryFuncSpy = jest.spyOn(repository, 'updateAzureClass');
        //     mockAzureEnrollmentsService.deleteAzureEnrollmentUserToClass.mockReturnValue({ data: 100 });
        //     mockAzureEnrollmentsService.createAzureEnrollmentUserToClass.mockReturnValue({ data: 100 });
        //     const resultClass = await service.updateAzureClass(request, dto);
        //     expect(resultClass).toEqual({
        //         data: {
        //             [CONSTANTS.RESPONSE_PARAM_NAME_CLASS_CREATED]: 100,
        //             [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: [100, 100, 100, 100, 100, 100],
        //         },
        //     });
        //     expect(repositoryFuncSpy).toBeCalled();
        // });
        // it('should fail to update azure class', async () => {
        //     const request = {} as Request;
        //     const dto = {} as ClassUpdateRequestDTO;
        //     mockClassService.getSubjectByCurriculumID.mockReturnValue(null);
        //     await expect(service.updateAzureClass(request, dto)).rejects.toThrow(new DataNotFoundException());
        // });
    });

    describe('deleteAzureClass', () => {
        it('should successfully delete azure class', async () => {
            // const request = {} as Request;
            // let dto = {} as ClassCreateRequestDTO;
            // mockClassService.getSubjectByCurriculumID.mockReturnValue([{ curriculumID: 3 }]);
            // mockClassService.generateClassTitle.mockReturnValue('someClass');
            // mockRepository.deleteAzureClass.mockReturnValue([{ rowID: 100 }]);
            // dto = { curriculumID: 3, personIDs: [1, 2, 3], institutionID: 1 };
            // const repositoryFuncSpy = jest.spyOn(repository, 'deleteAzureClass');
            // const resultClass = await service.deleteAzureClass(request, dto);
            // expect(resultClass).toEqual({
            //     data: { [CONSTANTS.RESPONSE_PARAM_NAME_CHECK_CODE]: 100 },
            // });
            // expect(repositoryFuncSpy).toBeCalled();
        });
        // it('should fail to delete azure class', async () => {
        //     const request = {} as Request;
        //     const dto = {} as ClassDeleteRequestDTO;
        //     mockClassService.getSubjectByCurriculumID.mockReturnValue(null);
        //     await expect(service.deleteAzureClass(request, dto)).rejects.toThrow(new DataNotFoundException());
        // });
    });

    describe('getAzureClassStatus', () => {
        it('should get azure class status', async () => {
            mockRepository.getAzureClassStatus.mockResolvedValue(mockAzureClass);
            const result = await service.getAzureClassStatus(100);
            expect(result).toEqual({
                data: { [CONSTANTS.RESPONSE_PARAM_NAME_EVENT_STATUS]: 'SUCCESSFUL' },
            });
        });
    });

    describe('restartClassWorkflow', () => {
        it('should restart class workflow', async () => {
            mockRepository.restartWorkflow.mockResolvedValue(100);
            const result = await service.restartClassWorkflow(10);
            expect(result).toEqual(100);
        });
    });
});
