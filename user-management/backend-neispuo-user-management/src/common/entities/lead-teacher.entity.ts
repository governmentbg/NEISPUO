import { Column, PrimaryGeneratedColumn, ViewEntity } from 'typeorm';

@ViewEntity({ schema: 'azure_temp', name: 'LeadTeacher' })
export class LeadTeacherEntity {
    @PrimaryGeneratedColumn({ name: 'PersonID' })
    PersonID: number;

    @Column({ name: 'SchoolYear ' })
    SchoolYear: number;

    @Column({ name: 'FullName' })
    FullName: string;

    @Column({ name: 'IdentityNumType' })
    IdentityNumType: string;

    @Column({ name: 'IdentityNum' })
    IdentityNum: string;

    @Column({ name: 'BirthDate' })
    BirthDate: string;

    @Column({ name: 'schoolBooksCodesID' })
    schoolBooksCodesID: string;

    @Column({ name: 'PublicEduNumber' })
    PublicEduNumber: string;

    @Column({ name: 'Nationallity' })
    Nationallity: string;

    @Column({ name: 'InstitutionID' })
    InstitutionID: number;

    @Column({ name: 'PositionName' })
    PositionName: string;

    @Column({ name: 'DischargeReason' })
    DischargeReason: string;

    @Column({ name: 'Class' })
    Class: string;

    @Column({ name: 'ClassGroup' })
    ClassGroup: string;

    @Column({ name: 'ClassID' })
    ClassID: string;

    @Column({ name: 'ClassTypeName' })
    ClassTypeName: string;

    @Column({ name: 'StudentSpeciality' })
    StudentSpeciality: string;

    @Column({ name: 'EduForm' })
    EduForm: string;

    @Column({ name: 'IsIndividualCurriculum' })
    IsIndividualCurriculum: string;

    @Column({ name: 'IsHourlyOrganization' })
    IsHourlyOrganization: string;

    @Column({ name: 'RepeaterReason' })
    RepeaterReason: string;

    @Column({ name: 'CommuterTypeName' })
    CommuterTypeName: string;

    @Column({ name: 'ResourceSupportReport' })
    ResourceSupportReport: string;

    @Column({ name: 'SpecialNeeds' })
    SpecialNeeds: string;
}
