import {
    Entity,
    PrimaryGeneratedColumn,
    Column,
    CreateDateColumn,
    UpdateDateColumn,
    ManyToOne
} from 'typeorm';
import { User } from '../user/user.entity';
import { Exclude } from 'class-transformer';

@Entity()
export class ForgotPassword {
    @PrimaryGeneratedColumn('uuid')
    id?: string;

    @Column()
    token: string;

    @Column({ default: false })
    used: boolean;

    /**
     * Relations
     */
    @ManyToOne(
        type => User,
        user => user.forgotPasswords,
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
