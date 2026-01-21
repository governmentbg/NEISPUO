import {
    Entity,
    Column,
    PrimaryGeneratedColumn,
    ManyToMany,
    JoinTable,
    CreateDateColumn,
    UpdateDateColumn
} from 'typeorm';
import { User } from '../user/user.entity';
import { RoleDTO } from './routes/role/role.dto';
import { RoleNameEnum } from './enums/role-name.enum';

@Entity()
export class Role {
    @PrimaryGeneratedColumn('uuid')
    id?: string;

    @CreateDateColumn()
    createdAt?: Date;

    @UpdateDateColumn()
    updatedAt?: Date;

    @Column()
    roleName: RoleNameEnum;

    @ManyToMany(
        type => User,
        user => user.roles
    )
    // @JoinTable() // -> Must be in only one side of m2m
    users?: User[];
}
