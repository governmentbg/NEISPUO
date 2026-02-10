import {
    Entity,
    PrimaryGeneratedColumn,
    Column,
    ManyToOne,
    CreateDateColumn,
    UpdateDateColumn
} from 'typeorm';
import { User } from '../user/user.entity';

@Entity()
export class Audit {
    @PrimaryGeneratedColumn()
    id?: number;

    @CreateDateColumn()
    createdAt?: Date;

    @UpdateDateColumn()
    updatedAt?: Date;

    @Column()
    statusCode: number;

    @Column()
    originalUrl: string;

    @Column()
    method: string; // POST PATCH PUT DELETE

    @Column('simple-json')
    requestBody: string;

    @Column('simple-json')
    requestHeaders: string;

    @Column('simple-json')
    responseBody: string;

    @ManyToOne(type => User)
    user?: User;
}
