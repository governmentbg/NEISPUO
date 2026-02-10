import {
    Entity,
    Column,
    PrimaryGeneratedColumn,
    CreateDateColumn,
    UpdateDateColumn,
    ManyToOne,
    ManyToMany,
    OneToMany
} from 'typeorm';
import { Exclude } from 'class-transformer';
import { User } from '../user/user.entity';

@Entity()
export class File {
    @PrimaryGeneratedColumn('uuid')
    id?: string;

    @Column()
    name: string;

    @Column()
    fsName: string;

    @Column()
    mimeType: string;

    @Column()
    contentMd5: string;

    @Column()
    fileSize: number;

    @Column({ type: 'simple-json', nullable: true })
    @Exclude()
    multerMetadata: any; // Express.Multer.File

    /**
     * Relations
     */
    @ManyToOne(
        type => User,
        user => user.files,
        { nullable: false }
    )
    user?: User;

    /**
     * Trackers
     */

    @CreateDateColumn()
    @Exclude({ toPlainOnly: true })
    createdAt?: Date | string;

    @UpdateDateColumn()
    @Exclude({ toPlainOnly: true })
    updatedAt?: Date | string;
}
