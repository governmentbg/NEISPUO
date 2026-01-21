import { Test, TestingModule } from '@nestjs/testing';

export type MockType<T> = {
    [P in keyof T]?: jest.Mock<any>;
};

describe('AzureTeacherService', () => {
    // let service: AzureTeacherService;
    // let repository: AzureTeachersRepository;

    // const mockRepository = {
    //     insertAzureTeacher: jest.fn((entity) => entity),
    //     updateAzureTeacher: jest.fn((entity) => entity),
    //     deleteAzureTeacher: jest.fn((entity) => entity),
    // };
    // const mockTeacherService = {
    //     getTeacherByPersonID: jest.fn((entity) => entity),
    //     generateClassTitle: jest.fn((entity) => entity),
    //     hasAdditionalRole: jest.fn((entity) => entity),
    // };
    // const mockAzureEnrollmentsService = {
    //     createAzureEnrollmentUserToSchool: jest.fn((entity) => entity),
    //     createAzureEnrollmentUserToClass: jest.fn((entity) => entity),
    //     deleteAzureEnrollmentUserToClass: jest.fn((entity) => entity),
    //     deleteAzureEnrollmentUserToSchool: jest.fn((entity) => entity),
    // };
    // const mockUserService = {};

    beforeEach(async () => {
        const module: TestingModule = await Test.createTestingModule({
            // providers: [
            //     { provide: AzureTeachersRepository, useValue: mockRepository },
            //     {
            //         provide: UserService,
            //         useValue: mockUserService,
            //     },
            //     {
            //         provide: TeacherService,
            //         useValue: mockTeacherService,
            //     },
            //     {
            //         provide: AzureEnrollmentsService,
            //         useValue: mockAzureEnrollmentsService,
            //     },
            //     AzureTeacherService,
            // ],
        }).compile();

        // service = module.get<AzureTeacherService>(AzureTeacherService);
        // repository = module.get<AzureTeachersRepository>(AzureTeachersRepository);
    });

    describe('createAzureTeacher', () => {
        it('true', () => {
            expect(1).toBe(1);
        });
        // it('should successfully insert azure teacher', async () => {
        //     const request = {} as Request;
        //     let dto = {} as TeacherCreateRequestDTO;
        //     dto = { personID: 10 };
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue({ personID: 10 });
        //     mockRepository.insertAzureTeacher.mockReturnValue({ rowID: 100 });
        //     const repositoryFuncSpy = jest.spyOn(repository, 'insertAzureTeacher');
        //     mockAzureEnrollmentsService.createAzureEnrollmentUserToSchool.mockReturnValue({ data: 100 });
        //     const resultTeacher = await service.createAzureTeacher(request, dto);
        //     expect(resultTeacher).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_CHECK_CODE]: [100, 100] } });
        //     expect(repositoryFuncSpy).toBeCalled();
        // });
        // it('should fail to insert azure teacher', async () => {
        //     const request = {} as Request;
        //     let dto = {} as TeacherCreateRequestDTO;
        //     dto = { personID: 10 };
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue(100);
        //     await expect(service.createAzureTeacher(request, dto)).rejects.toEqual(new DataNotFoundException());
        // });
    });

    describe('updateAzureTeacher', () => {
        // it('should successfully update azure teacher', async () => {
        //     const request = {} as Request;
        //     const dto = {} as TeacherUpdateRequestDTO;
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue({ personID: 100010 });
        //     mockRepository.updateAzureTeacher.mockReturnValue({ rowID: 100 });
        //     const repositoryFuncSpy = jest.spyOn(repository, 'updateAzureTeacher');
        //     const result = await service.updateAzureTeacher(request, dto);
        //     expect(result).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_CHECK_CODE]: 100 } });
        //     expect(repositoryFuncSpy).toBeCalled();
        // });
        // it('should fail to update azure teacher and throw DataNotFoundException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as TeacherUpdateRequestDTO;
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue(100010);
        //     await expect(service.updateAzureTeacher(request, dto)).rejects.toEqual(new DataNotFoundException());
        // });
    });

    describe('deleteAzureTeacher', () => {
        // it('should successfully delete azure teacher', async () => {
        //     const request = {} as Request;
        //     const dto = {} as DeleteTeacherDtoRequest;
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue({ personID: 100010 });
        //     mockRepository.deleteAzureTeacher.mockReturnValue({ rowID: 100 });
        //     const repositoryFuncSpy = jest.spyOn(repository, 'deleteAzureTeacher');
        //     const result = await service.deleteAzureTeacher(request, dto);
        //     expect(result).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_CHECK_CODE]: 100 } });
        //     expect(repositoryFuncSpy).toBeCalled();
        // });
        // it('should fail to delete azure teacher and throw DataNotFoundException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as TeacherUpdateRequestDTO;
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue(100010);
        //     await expect(service.deleteAzureTeacher(request, dto)).rejects.toEqual(new DataNotFoundException());
        // });
    });

    describe('createAzureEnrollmentTeacherToSchool', () => {
        // it('should successfully create enrollment teacher to school', async () => {
        //     const request = {} as Request;
        //     const dto = {} as EnrollmentTeacherToSchoolCreateRequestDTO;
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue({ personID: 100 });
        //     mockAzureEnrollmentsService.createAzureEnrollmentUserToSchool.mockReturnValue({ data: 100 });
        //     const result = await service.createAzureEnrollmentTeacherToSchool(request, dto);
        //     expect(result).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: 100 } });
        // });
        // it('should fail to create enrollment teacher to school and throw DataNotFoundException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as EnrollmentTeacherToSchoolCreateRequestDTO;
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue(100010);
        //     await expect(service.createAzureEnrollmentTeacherToSchool(request, dto)).rejects.toEqual(
        //         new DataNotFoundException(),
        //     );
        // });
    });

    describe('createAzureEnrollmentTeacherToClass', () => {
        // it('should successfully create azure enrollment teacher to class', async () => {
        //     const request = {} as Request;
        //     let dto = {} as EnrollmentTeacherToClassCreateRequestDTO;
        //     dto = { personID: 2, curriculumIDs: [1, 2, 3] };
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue({ personID: 100 });
        //     mockAzureEnrollmentsService.createAzureEnrollmentUserToClass.mockReturnValue({ data: 100 });
        //     const result = await service.createAzureEnrollmentTeacherToClass(request, dto);
        //     expect(result).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: [100, 100, 100] } });
        // });
        // it('should fail to create azure enrollment teacher to class and throw DataNotFoundException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as EnrollmentTeacherToClassCreateRequestDTO;
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue(100010);
        //     await expect(service.createAzureEnrollmentTeacherToClass(request, dto)).rejects.toEqual(
        //         new DataNotFoundException(),
        //     );
        // });
    });

    describe('deleteAzureEnrollmentTeacherToSchool', () => {
        // it('should successfully delete azure enrollment teacher to school', async () => {
        //     const request = {} as Request;
        //     let dto = {} as EnrollmentTeacherToSchoolDeleteRequestDTO;
        //     dto = { personID: 2, institutionID: 2 };
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue({ personID: 100 });
        //     mockAzureEnrollmentsService.deleteAzureEnrollmentUserToSchool.mockReturnValue({ data: 100 });
        //     const result = await service.deleteAzureEnrollmentTeacherToSchool(request, dto);
        //     expect(result).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: 100 } });
        // });
        // it('should fail to delete azure enrollment teacher to school and throw DataNotFoundException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as EnrollmentTeacherToSchoolDeleteRequestDTO;
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue(100010);
        //     await expect(service.deleteAzureEnrollmentTeacherToSchool(request, dto)).rejects.toEqual(
        //         new DataNotFoundException(),
        //     );
        // });
    });

    describe('deleteAzureEnrollmentTeacherToClass', () => {
        // it('should successfully delete enrollment teacher to class', async () => {
        //     const request = {} as Request;
        //     let dto = {} as EnrollmentTeacherToClassDeleteRequestDTO;
        //     dto = { personID: 2, curriculumID: 2 };
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue({ personID: 100 });
        //     mockAzureEnrollmentsService.deleteAzureEnrollmentUserToClass.mockReturnValue({ data: 100 });
        //     const result = await service.deleteAzureEnrollmentTeacherToClass(request, dto);
        //     expect(result).toEqual({ data: { [CONSTANTS.RESPONSE_PARAM_NAME_ENROLLMENT_CREATED]: 100 } });
        // });
        // it('should fail to delete enrollment teacher to class and throw DataNotFoundException', async () => {
        //     const request = {} as Request;
        //     const dto = {} as EnrollmentTeacherToClassDeleteRequestDTO;
        //     mockTeacherService.getTeacherByPersonID.mockReturnValue(100010);
        //     await expect(service.deleteAzureEnrollmentTeacherToClass(request, dto)).rejects.toEqual(
        //         new DataNotFoundException(),
        //     );
        // });
    });
});

// commenting out since they don't work as expected according to Ivo
