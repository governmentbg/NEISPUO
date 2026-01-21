import { Injectable } from '@nestjs/common';
import { IdentityProvider } from '../../identity-provider.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository } from 'typeorm';

@Injectable()
export class IdentityProviderService extends TypeOrmCrudService<IdentityProvider> {
    constructor(@InjectRepository(IdentityProvider) repo: Repository<IdentityProvider>) {
        super(repo);
    }
}
