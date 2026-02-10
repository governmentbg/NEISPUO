import * as bcrypt from 'bcryptjs';
import { Injectable } from '@nestjs/common';
import { ConfigService } from '../config/config.service';

@Injectable()
export class BcryptService {
    constructor(private readonly configService: ConfigService) {}

    public async compare(password: string, hash: string): Promise<boolean> {
        return bcrypt.compare(password, hash);
    }

    public async hash(password: string) {
        return bcrypt.hash(password, +process.env.BCRYPT_ROUNDS);
    }
}
