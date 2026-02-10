import { Injectable } from '@nestjs/common';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { ForgotPassword } from '../../forgot-password.entity';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class ForgotPasswordService extends TypeOrmCrudService<ForgotPassword> {
    constructor(@InjectRepository(ForgotPassword) repo: Repository<ForgotPassword>) {
        super(repo);
    }
}
