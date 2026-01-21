import { Injectable, NotFoundException } from '@nestjs/common';
import { User } from '../../user.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository, Not } from 'typeorm';
import { UpdateUserDto } from './update-user.dto';

@Injectable()
export class UserService extends TypeOrmCrudService<User> {
    constructor(@InjectRepository(User) public readonly repo: Repository<User>) {
        super(repo);
    }

    async userEmailExists(userEmail: string, userUuid?: string) {
        const user = userUuid
            ? await this.repo.findOne({ where: { id: Not(userUuid), email: userEmail } })
            : await this.repo.findOne({ where: { email: userEmail } });
        return !!user;
    }

    async updateUser(userId: string, body: UpdateUserDto) {
        const user = await this.repo.findOne({ id: userId });

        if (!user) {
            throw new NotFoundException(`Could not find user with id: ${userId}`);
        }

        return await this.repo.update({ id: userId }, { ...body });
    }
}
