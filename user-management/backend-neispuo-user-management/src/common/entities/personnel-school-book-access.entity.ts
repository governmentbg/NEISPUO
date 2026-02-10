import { Column, Entity, JoinColumn, ManyToOne, PrimaryGeneratedColumn } from 'typeorm';
import { ClassBookEntity } from './class-book.entity';
import { PersonEntity } from './person.entity';

@Entity({ schema: 'school_books', name: 'PersonnelSchoolBookAccess' })
export class PersonnelSchoolBookAccess {
    @PrimaryGeneratedColumn({ name: 'RowID' })
    rowID: number;

    @Column({ name: 'SchoolYear' })
    schoolYear: number;

    @Column({ name: 'ClassBookID' })
    classBookID: number;

    @Column({ name: 'PersonID' })
    personID: number;

    @Column({ name: 'HasAdminAccess' })
    hasAdminAccess: boolean;

    @ManyToOne((type) => PersonEntity, (personEntity) => personEntity.personnelSchoolBookAccess, {
        eager: true,
    })
    @JoinColumn({ name: 'PersonID' })
    person: PersonEntity;

    @ManyToOne((type) => ClassBookEntity, (classBookEntity) => classBookEntity.personnelSchoolBookAccess, {
        eager: true,
    })
    @JoinColumn({ name: 'ClassBookID', referencedColumnName: 'classBookID' })
    classBook: ClassBookEntity;
}
