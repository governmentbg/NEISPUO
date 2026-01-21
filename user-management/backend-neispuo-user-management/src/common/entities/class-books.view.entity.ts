import { Column, ViewEntity } from 'typeorm';

@ViewEntity({ schema: 'school_books', name: 'vwClassBooks' })
export class ClassBooksView {
    @Column({ name: 'SchoolYear' })
    schoolYear: number;

    @Column({ name: 'ClassBookId' })
    classBookID: number;

    @Column({ name: 'InstId' })
    instId: number;

    @Column({ name: 'FullBookName' })
    fullBookName: string;

    @Column({ name: 'BookTypeName' })
    bookTypeName: string;
}
