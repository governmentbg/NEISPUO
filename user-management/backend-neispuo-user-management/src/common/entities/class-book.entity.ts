import { ApiModelProperty } from '@nestjs/swagger';
import { Column, Entity, JoinColumn, ManyToOne, OneToMany, PrimaryColumn } from 'typeorm';
import { InstitutionEntity } from './institution.entity';
import { PersonnelSchoolBookAccess } from './personnel-school-book-access.entity';

@Entity({ schema: 'school_books', name: 'ClassBook' })
export class ClassBookEntity {
    @PrimaryColumn({ name: 'SchoolYear' })
    schoolYear: number;

    @PrimaryColumn({ name: 'ClassBookId' })
    classBookID: number;

    @Column({ name: 'InstId' })
    instId: string;

    @Column({ name: 'ClassId' })
    classId: number;

    @Column({ name: 'ClassIsLvl2' })
    classIsLvl2: BinaryType;

    @Column({ name: 'BookType' })
    bookType: number;

    @Column({ name: 'BasicClassId' })
    basicClassId: number;

    @Column({ name: 'BookName' })
    bookName: string;

    @Column({ name: 'SchoolYearProgram' })
    schoolYearProgram: string;

    @Column({ name: 'CreateDate' })
    createDate: Date;

    @Column({ name: 'CreatedBySysUserId' })
    createdBySysUserId: number;

    @Column({ name: 'ModifyDate' })
    modifyDate: number;

    @Column({ name: 'ModifiedBySysUserId' })
    modifiedBySysUserId: number;

    @Column({ name: 'Version', nullable: true })
    version: string;

    @Column({ name: 'BasicClassName' })
    basicClassName: string;

    @Column({ name: 'FullBookName' })
    fullBookName: string;

    @ApiModelProperty()
    @OneToMany(() => PersonnelSchoolBookAccess, (personnelSchoolBookAccess) => personnelSchoolBookAccess.classBook)
    @JoinColumn({ name: 'classBookId' })
    personnelSchoolBookAccess: ClassBookEntity[];

    @ManyToOne((type) => InstitutionEntity, (institutionEntity) => institutionEntity.classBooks, {
        eager: true,
    })
    @JoinColumn({ name: 'InstId', referencedColumnName: 'institutionID' })
    institution: InstitutionEntity;
}
