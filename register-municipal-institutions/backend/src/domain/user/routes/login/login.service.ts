import { Injectable, UnauthorizedException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { User } from '../../user.entity';
import { Repository } from 'typeorm';
import { compare } from 'bcryptjs';
import { JwtService } from '../../../../shared/services/jwt/jwt.service';
import { JwtPayloadDTO_Deprecated, JwtContent_Deprecated } from '../../../../shared/services/jwt/jwt.interface';

@Injectable()
export class LoginService {
    constructor(
        @InjectRepository(User) private readonly repo: Repository<User>,
        private readonly jwtService: JwtService
    ) {}

    async validateCredentials(credentials: {
        email: string;
        password: string;
    }): Promise<User | null> {
        const user = await this.repo.findOne(
            { email: credentials.email },
            { relations: ['roles'] }
        );

        const passwordValid = user && (await compare(credentials.password, user.password));
        if (!passwordValid) {
            throw new UnauthorizedException('Invalid email or password.');
        } else if (!user.emailVerified) {
            throw new UnauthorizedException('Email is not verified.');
        }

        return user;
    }

    async createAccessTokenFromUser(user: User): Promise<string> {
        const jwtPayload: JwtContent_Deprecated = {
            userId: user.id,
            email: user.email,
            roles: user.roles
        };
        return this.jwtService.sign(jwtPayload);
    }
}
