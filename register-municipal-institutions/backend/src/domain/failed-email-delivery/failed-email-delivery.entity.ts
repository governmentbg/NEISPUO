import {
    Entity,
    PrimaryGeneratedColumn,
    Column,
    CreateDateColumn,
    UpdateDateColumn,
    ManyToOne
} from 'typeorm';
import { User } from '../user/user.entity';

@Entity()
export class FailedEmailDelivery {
    @PrimaryGeneratedColumn('uuid')
    id?: string;

    @CreateDateColumn()
    createdAt?: Date;

    @UpdateDateColumn()
    updatedAt?: Date;

    @Column()
    sender: string;

    @Column()
    recipient: string;

    @Column()
    subject: string;

    @Column()
    body: string;

    @Column({ type: 'text' })
    error: string;

    @ManyToOne(
        type => User,
        user => user.failedEmailDeliveries
    )
    user?: User;
}
