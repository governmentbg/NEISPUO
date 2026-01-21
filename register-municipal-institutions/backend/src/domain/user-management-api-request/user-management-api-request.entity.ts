import { UserManagementAPIRequest } from '@shared/interfaces/user-management-api-request.interface';
import { Column, Entity, PrimaryGeneratedColumn } from 'typeorm';

@Entity({ schema: 'logs', name: 'UserManagementAPIRequest' })
export class UserManagementAPIRequestEntity implements Partial<UserManagementAPIRequest> {
    @PrimaryGeneratedColumn({ name: 'Id' })
    Id: number;

    @Column()
    AuditModuleId: number;

    @Column({
        type: 'datetime2',
    })
    CreateDate: Date;

    @Column({
        type: 'varchar',
    })
    Url: string;

    @Column({
        type: 'varchar',
    })
    Operation: string;

    @Column({
        type: 'varchar',
    })
    Request: string;

    @Column({
        type: 'varchar',
    })
    Response: string;

    @Column({ type: 'int' })
    ResponseHttpCode: number;

    @Column({ type: 'int' })
    PersonId: number;

    @Column({ type: 'int' })
    RetryCount: number;

    @Column({
        type: 'datetime2',
    })
    LastRetryDate: Date;

    @Column({ type: 'int' })
    CreatedBySysUserId: number;

    @Column({ type: 'bit' })
    IsError: number;

    @Column({ type: 'int' })
    CurriculumID: number;

    @Column({ type: 'int' })
    InstitutionID: number;
}
