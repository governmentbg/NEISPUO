import { Test, TestingModule } from '@nestjs/testing';
import { StudentCreateRequestDTO } from 'src/common/dto/requests/student-create-request.dto';
import { StudentService } from 'src/models/student/routing/student.service';
import { TelelinkService } from 'src/models/telelink/routing/telelink.service';
import { UserService } from 'src/models/user/routing/user.service';
import { AzureStudentRepository } from '../azure-student.repository';
import { AzureStudentService } from '../routing/azure-student.service';
import { AzureEnrollmentsService } from '../../azure-enrollments/routing/azure-enrollments.service';
import { Connection } from 'typeorm';
import { EntityNotCreatedException } from 'src/common/exceptions/entity-not-created.exception';
import { AzureUsersService } from '../../azure-users/routing/azure-users.service';

export type MockType<T> = {
    [P in keyof T]?: jest.Mock<any>;
};

describe('AzureStudentService', () => {
    let service: AzureStudentService;
    let repository: AzureStudentRepository;

    const mockRepository = {
        insertAzureStudent: jest.fn((entity) => entity),
        updateAzureStudent: jest.fn((entity) => entity),
        deleteAzureStudent: jest.fn((entity) => entity),
        disableAzureStudent: jest.fn((entity) => entity),
        getStudentDiplomaCount: jest.fn((entity) => entity),
    };
    const mockStudentService = {
        getStudentByPersonID: jest.fn((entity) => entity),
    };
    const mockAzureEnrollmentsService = {
        createAzureEnrollmentUserToSchool: jest.fn((entity) => entity),
        createAzureEnrollmentUserToClass: jest.fn((entity) => entity),
        deleteAzureEnrollmentUserToSchool: jest.fn((entity) => entity),
        deleteAzureEnrollmentUserToClass: jest.fn((entity) => entity),
    };
    const mockAzureUsersService = {
        createAzureEnrollmentUserToSchool: jest.fn((entity) => entity),
    };

    const mockConnection = {};

    beforeEach(async () => {
        const module: TestingModule = await Test.createTestingModule({
            providers: [
                { provide: AzureStudentRepository, useValue: mockRepository },
                {
                    provide: TelelinkService,
                    useValue: {},
                },
                {
                    provide: StudentService,
                    useValue: mockStudentService,
                },
                {
                    provide: UserService,
                    useValue: {},
                },
                {
                    provide: AzureEnrollmentsService,
                    useValue: mockAzureEnrollmentsService,
                },
                {
                    provide: Connection,
                    useValue: mockConnection,
                },
                {
                    provide: AzureUsersService,
                    useValue: mockAzureUsersService,
                },
                AzureStudentService,
            ],
        }).compile();

        service = module.get<AzureStudentService>(AzureStudentService);
        repository = module.get<AzureStudentRepository>(AzureStudentRepository);
    });

    describe('createAzureStudent', () => {
        // it('should successfully insert azure student', async () => {
        //     const request = {} as Request;
        //     const dto = {} as StudentCreateRequestDTO;
        //     mockStudentService.getStudentByPersonID.mockReturnValue({ personID: 100010 });
        //     mockRepository.insertAzureStudent.mockReturnValue({ rowID: 100 });
        //     const repositoryFuncSpy = jest.spyOn(repository, 'insertAzureStudent');
        //     const result = await service.createAzureStudent(request, dto);
        //     expect(result).toEqual({
        //         data: { [CONSTANTS.RESPONSE_PARAM_NAME_USER_CREATED]: 100 },
        //     });
        //     expect(repositoryFuncSpy).toBeCalled();
        // });
        it('should fail to insert azure student and throw EntityNotCreatedException', async () => {
            const request = {} as Request;
            const dto = {} as StudentCreateRequestDTO;
            mockStudentService.getStudentByPersonID.mockReturnValue({ personID: 100010 });
            mockRepository.insertAzureStudent.mockReturnValue(100);
            await expect(service.createAzureStudent(request, dto)).rejects.toEqual(new EntityNotCreatedException());
        });
        // it('should fail to insert azure student and throw DataNotFoundException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as StudentCreateRequestDTO;
        //     mockStudentService.getStudentByPersonID.mockReturnValue(100010);
        //     await expect(service.createAzureStudent(request, dto)).rejects.toEqual(new DataNotFoundException());
        // });
    });

    describe('updateAzureStudent', () => {
        // it('should successfully update azure student', async () => {
        //     const request = {} as Request;
        //     const dto = {} as StudentUpdateRequestDTO;
        //     mockStudentService.getStudentByPersonID.mockReturnValue({ personID: 100 });
        //     mockRepository.updateAzureStudent.mockReturnValue({ rowID: 100 });
        //     const repositoryFuncSpy = jest.spyOn(repository, 'updateAzureStudent');
        //     const result = await service.updateAzureStudent(request, dto);
        //     expect(result).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_CHECK_CODE]: 100 } });
        //     expect(repositoryFuncSpy).toBeCalled();
        // });
        // it('should fail to update azure student and throw EntityNotCreatedException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as StudentUpdateRequestDTO;
        //     mockStudentService.getStudentByPersonID.mockReturnValue({ personID: 100 });
        //     mockRepository.updateAzureStudent.mockReturnValue(100);
        //     await expect(service.updateAzureStudent(request, dto)).rejects.toEqual(new EntityNotCreatedException());
        // });
        // it('should fail to update azure student and throw DataNotFoundException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as StudentUpdateRequestDTO;
        //     mockStudentService.getStudentByPersonID.mockReturnValue(100010);
        //     await expect(service.updateAzureStudent(request, dto)).rejects.toEqual(new DataNotFoundException());
        // });
    });

    describe('createAzureEnrollmentStudentToSchool', () => {
        // it('should successfully create enrollment student to school', async () => {
        //     const request = {} as Request;
        //     const dto = {} as EnrollmentStudentToSchoolCreateRequestDTO;
        //     mockStudentService.getStudentByPersonID.mockReturnValue({ personID: 100 });
        //     mockRepository.updateAzureStudent.mockReturnValue({ rowID: 100 });
        //     mockAzureEnrollmentsService.createAzureEnrollmentUserToSchool.mockReturnValue({ data: 100 });
        //     const result = await service.createAzureEnrollmentStudentToSchool(request, dto);
        //     expect(result).toEqual({
        //         data: {
        //             [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: [
        //                 { [CONSTANTS.RESPONSE_PARAM_NAME_CHECK_CODE]: 100 },
        //                 100,
        //             ],
        //         },
        //     });
        // });
        // it('should fail to create enrollment student to school and throw DataNotFoundException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as EnrollmentStudentToSchoolCreateRequestDTO;
        //     mockStudentService.getStudentByPersonID.mockReturnValue(100010);
        //     await expect(service.createAzureEnrollmentStudentToSchool(request, dto)).rejects.toEqual(
        //         new DataNotFoundException(),
        //     );
        // });
    });

    describe('createAzureEnrollmentStudentToClass', () => {
        // it('should successfully create azure enrollment student to class', async () => {
        //     const request = {} as Request;
        //     let dto = {} as EnrollmentStudentToClassCreateRequestDTO;
        //     dto = { personID: 2, curriculumIDs: [1, 2, 3] };
        //     mockStudentService.getStudentByPersonID.mockReturnValue({ personID: 100 });
        //     mockAzureEnrollmentsService.createAzureEnrollmentUserToClass.mockReturnValue({ data: 100 });
        //     const result = await service.createAzureEnrollmentStudentToClass(request, dto);
        //     expect(result).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: [100, 100, 100] } });
        // });
        // it('should fail to create azure enrollment student to class and throw DataNotFoundException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as EnrollmentStudentToClassCreateRequestDTO;
        //     mockStudentService.getStudentByPersonID.mockReturnValue(100010);
        //     await expect(service.createAzureEnrollmentStudentToClass(request, dto)).rejects.toEqual(
        //         new DataNotFoundException(),
        //     );
        // });
    });

    describe('deleteAzureEnrollmentStudentToSchool', () => {
        // it('should successfully delete azure enrollment teacher to school', async () => {
        //     const request = {} as Request;
        //     let dto = {} as EnrollmentStudentToSchoolDeleteRequestDTO;
        //     dto = { personID: 2, institutionID: 2 };
        //     mockStudentService.getStudentByPersonID.mockReturnValue({ personID: 100 });
        //     mockAzureEnrollmentsService.deleteAzureEnrollmentUserToSchool.mockReturnValue({ data: 100 });
        //     const result = await service.deleteAzureEnrollmentStudentToSchool(request, dto);
        //     expect(result).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: 100 } });
        // });
        // it('should fail to delete azure enrollment teacher to school and throw DataNotFoundException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as EnrollmentStudentToSchoolDeleteRequestDTO;
        //     mockStudentService.getStudentByPersonID.mockReturnValue(100010);
        //     await expect(service.deleteAzureEnrollmentStudentToSchool(request, dto)).rejects.toEqual(
        //         new DataNotFoundException(),
        //     );
        // });
    });

    describe('deleteAzureEnrollmentStudentToClass', () => {
        // it('should successfully delete enrollment student to class', async () => {
        //     const request = {} as Request;
        //     let dto = {} as EnrollmentStudentToClassDeleteRequestDTO;
        //     dto = { personID: 2, curriculumID: 2 };
        //     mockStudentService.getStudentByPersonID.mockReturnValue({ personID: 100 });
        //     mockAzureEnrollmentsService.deleteAzureEnrollmentUserToClass.mockReturnValue({ data: 100 });
        //     const result = await service.deleteAzureEnrollmentStudentToClass(request, dto);
        //     expect(result).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: 100 } });
        // });
        // it('should fail to delete enrollment student to class and throw DataNotFoundException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as EnrollmentStudentToClassDeleteRequestDTO;
        //     mockStudentService.getStudentByPersonID.mockReturnValue(100010);
        //     await expect(service.deleteAzureEnrollmentStudentToClass(request, dto)).rejects.toEqual(
        //         new DataNotFoundException(),
        //     );
        // });
    });

    describe('deleteOrDisableAzureStudent', () => {
        // it('should successfully delete azure student', async () => {
        //     const request = {} as Request;
        //     let dto = {} as StudentDeleteDisableRequestDTO;
        //     dto = { positionID: 11, personID: 1 };
        //     mockStudentService.getStudentByPersonID.mockReturnValue({ personID: 100010 });
        //     mockRepository.deleteAzureStudent.mockReturnValue({ rowID: 100 });
        //     const repositoryFuncSpy = jest.spyOn(repository, 'deleteAzureStudent');
        //     const result = await service.deleteOrDisableAzureStudent(request, dto);
        //     expect(result).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_CHECK_CODE]: 100 } });
        //     expect(repositoryFuncSpy).toBeCalled();
        // });
        // it('should successfully disable azure student', async () => {
        //     const request = {} as Request;
        //     let dto = {} as StudentDeleteDisableRequestDTO;
        //     dto = { positionID: 9, personID: 1 };
        //     mockStudentService.getStudentByPersonID.mockReturnValue({ personID: 100010 });
        //     mockRepository.disableAzureStudent.mockReturnValue({ rowID: 100 });
        //     const repositoryFuncSpy = jest.spyOn(repository, 'disableAzureStudent');
        //     const result = await service.deleteOrDisableAzureStudent(request, dto);
        //     expect(result).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_CHECK_CODE]: 100 } });
        //     expect(repositoryFuncSpy).toBeCalled();
        // });
        it('should fail to delete or disable student and throw DataNotFoundException', async () => {
            // const request = {} as Request;
            // const dto = {} as StudentDeleteDisableRequestDTO;
            // mockStudentService.getStudentByPersonID.mockReturnValue(100010);
            // await expect(service.deleteOrDisableAzureStudent(request, dto)).rejects.toEqual(
            //     new DataNotFoundException(),
            // );
        });
    });
});
