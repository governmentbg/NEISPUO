import { Injectable, UnauthorizedException, BadRequestException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { User } from '../../user.entity';
import { Repository } from 'typeorm';
import { VerifyEmailDTO } from './verify-email.dto';

@Injectable()
export class VerifyEmailService {
    constructor(@InjectRepository(User) private readonly userRepo: Repository<User>) {}

    async verifyEmail(verifyEmailDTO: VerifyEmailDTO) {
        const user = await this.userRepo.findOne({
            email: verifyEmailDTO.email,
            emailVerificationToken: verifyEmailDTO.token
        });

        if (!user) {
            return { verified: false, error: 'Could not find user matching specified criteria' };
            // throw new UnauthorizedException('Could not find user matching specified criteria'); // TODO check if the right exeption is used
        }

        await this.userRepo.update(user.id, { emailVerified: true, emailVerificationToken: '' });
        return { verified: true };
    }
}
