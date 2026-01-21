import { NestFactory } from '@nestjs/core';
import { AppModule } from '../../src/app.module';
import { Connection } from 'typeorm';
import { User } from '../../src/domain/user/user.entity';
import { RandomStringService } from '../../src/shared/services/random-string/random-string.service';
import { BcryptService } from '../../src/shared/services/bcrypt/bcrypt.service';
import { ConfigService } from '../../src/shared/services/config/config.service';
import { WINSTON_MODULE_NEST_PROVIDER } from 'nest-winston';
import { Role } from '../../src/domain/role/role.entity';
import fs from 'fs';
import { RoleNameEnum } from '../../src/domain/role/enums/role-name.enum';
const translatedRoleNameEnum = {
    ADMIN: 'АДМИНИСТРАТОР'
};

function getTranslatedRole(roleName: string) {
    let translatedRole = '-';
    switch (roleName) {
        case 'ADMIN':
            translatedRole = translatedRoleNameEnum.ADMIN;
            break;
    }
    return translatedRole;
}
async function bootstrap() {
    // services
    const app = await NestFactory.createApplicationContext(AppModule);
    const logger = app.get(WINSTON_MODULE_NEST_PROVIDER);
    const config = app.get(ConfigService);
    const bcrypt = app.get(BcryptService);
    const randomString = app.get(RandomStringService);
    const connection = app.get(Connection);
    const userRepo = connection.getRepository(User);
    const roleRepo = connection.getRepository(Role);

    const userRole: string = process.argv[2];
    if (!translatedRoleNameEnum.hasOwnProperty(userRole)) {
        throw new Error('Invalid user role.');
    }
    const users: User[] = JSON.parse(
        fs.readFileSync(
            `${process.cwd()}/scripts/mail-access-dispatcher/users-json/${process.argv[3]}`,
            'utf8'
        )
    );
    logger.warn(`Starting script user credential script`);

    const role = await roleRepo.findOne({ where: { roleName: userRole } });
    for (const user of users) {
        try {
            const newPass = randomString.random(8);
            const bcrypted = await bcrypt.hash(newPass);
            const newUser = {
                ...user,
                address: '',
                phone: '',
                emailVerified: true,
                middleName: '',
                roles: [role],
                password: bcrypted
            };
            await userRepo.save(newUser);
            const template = 'new-account-created';
            logger.warn(
                `Successfully created account and sent email: ${newUser.email}, uuid: ${newUser.id}.`
            );
        } catch (e) {
            logger.error(`Failed creation for ${user.email}. Error: ${e}`);
        }
    }

    logger.warn(`FINISHED script create and send user credential`);

    await app.close();
}

bootstrap();
