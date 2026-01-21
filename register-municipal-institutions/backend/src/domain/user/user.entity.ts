import { File } from './../file/file.entity';
import { Exclude } from 'class-transformer';
import {
    Entity,
    Column,
    PrimaryGeneratedColumn,
    ManyToMany,
    JoinTable,
    OneToMany,
    UpdateDateColumn,
    CreateDateColumn
} from 'typeorm';
import { Role } from '../role/role.entity';
import { ForgotPassword } from '../forgot-password/forgot-password.entity';
import { FailedEmailDelivery } from '../failed-email-delivery/failed-email-delivery.entity';

@Entity()
export class User {
    @PrimaryGeneratedColumn('uuid')
    id?: string;

    @Column()
    @Exclude({ toPlainOnly: true })
    password?: string;

    @Column({
        unique: true
    })
    email: string;

    @Column()
    firstName: string;

    @Column()
    middleName: string;

    @Column()
    lastName: string;

    // @Column({
    //     length: 1024,
    //     asExpression: "concat(`firstName`,_utf8mb4' ',`middleName`,_utf8mb4' ',`lastName`)",
    //     generatedType: 'VIRTUAL',
    //     insert: false, // prevents mysql errors when attempting to write to virtual column
    //     update: false // prevents mysql errors when attempting to write to virtual column
    // })
    // fullName?: string;

    @Column()
    address: string;

    @Column()
    phone: string;

    @Column({ default: false })
    @Exclude({ toPlainOnly: true })
    emailVerified?: boolean;

    @Column({ default: '' })
    @Exclude({ toPlainOnly: true })
    emailVerificationToken: string;

    @ManyToMany(
        type => Role,
        role => role.users
    )
    @JoinTable() // -> Must be in only one side of m2m
    roles?: Role[];

    @OneToMany(
        type => ForgotPassword,
        forgotPassword => forgotPassword.user
    )
    forgotPasswords?: ForgotPassword[];

    @OneToMany(
        type => FailedEmailDelivery,
        failedEmailDelivery => failedEmailDelivery.user
    )
    failedEmailDeliveries?: FailedEmailDelivery[];

    @OneToMany(
        type => File,
        file => file.user
    )
    files?: File[];

    @CreateDateColumn()
    @Exclude({ toPlainOnly: true })
    createdAt?: Date | string;

    @UpdateDateColumn()
    @Exclude({ toPlainOnly: true })
    updatedAt?: Date | string;

    @Column({ type: 'datetime', nullable: true })
    @Exclude({ toPlainOnly: true })
    deletedAt?: Date | string;
}
