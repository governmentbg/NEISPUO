import { Injectable, BadRequestException, ConflictException } from '@nestjs/common';
import { User } from '../../user.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Repository, Not, Connection, TransactionManager } from 'typeorm';
import { UserDTO } from '../user/user.dto';
import { PasswordValidator } from '../../../../shared/services/password-validator/password-validator.service';
import { Role } from '../../../../domain/role/role.entity';
import { RoleNameEnum } from 'src/domain/role/enums/role-name.enum';
import { BcryptService } from '../../../../shared/services/bcrypt/bcrypt.service';
import { RandomStringService } from '../../../../shared/services/random-string/random-string.service';
import { FailedEmailDelivery } from '../../../../domain/failed-email-delivery/failed-email-delivery.entity';
import { ConfigService } from '../../../../shared/services/config/config.service';

@Injectable()
export class RegisterService extends TypeOrmCrudService<User> {
    constructor(
        @InjectRepository(User) repo: Repository<User>,
        private passwordValidator: PasswordValidator,
        private connection: Connection,
        private bcryptService: BcryptService,
        private randomStringService: RandomStringService,
        @InjectRepository(FailedEmailDelivery)
        private readonly failedEmailDeliveryRepo: Repository<FailedEmailDelivery>,
        private readonly configService: ConfigService
    ) {
        super(repo);
    }

    private validatePassword(body: UserDTO) {
        if (body.password !== body.confirmPassword) {
            throw new BadRequestException('Password confirmation does not match');
        }

        const { passwordValid, message } = this.passwordValidator.validate(body.password);
        if (!passwordValid) {
            throw new BadRequestException(message);
        }
    }

    private async validateNotTaken(body: UserDTO) {
        const preexistingUser = await this.repo.findOne({ email: body.email });
        if (preexistingUser) {
            throw new ConflictException('Email already taken');
        }
    }

    private async sendVerificationEmail(user: User) {
        const uiEndpoint =
            this.configService.get('API_BASE_URL') + this.configService.get('UI_VERIFY_EMAIL');
        const url = `${uiEndpoint}?email=${user.email}&token=${user.emailVerificationToken}`;
        const lastName = user.lastName;
    }

    // async register(body: UserDTO) {
    //     const roles = this.connection.getRepository(Role);
    //     await this.validatePassword(body);
    //     await this.validateNotTaken(body);

    //     const studentRole = await roles.findOne({ where: { roleName: RoleNameEnum.STUDENT } });
    //     const user: User = {
    //         ...body,
    //         roles: [studentRole],
    //         password: await this.bcryptService.hash(body.password),
    //         emailVerificationToken: this.randomStringService.random(32)
    //     };
    //     await this.repo.save(user);
    //     await this.sendVerificationEmail(user);
    // }
}
