import { Injectable } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { CONSTANTS } from 'src/common/constants/constants';
import { GraphApiResponseEnum } from 'src/common/constants/enum/graph-api-response.enum';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { ParentRequestDTO } from 'src/common/dto/requests/parent-request.dto';
import { ParentViewEntity } from 'src/common/entities/parent-view.entity';
import { DataNotFoundException } from 'src/common/exceptions/data-not-found.exception';
import { GraphApiService } from 'src/models/graph-api/routing/graph-api.service';
import { PersonService } from 'src/models/person/routing/person.service';
import { UserService } from 'src/models/user/routing/user.service';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: ['azureSyncParent'],
})
export class ParentCrudService extends TypeOrmCrudService<ParentViewEntity> {
    constructor(
        @InjectRepository(ParentViewEntity) repo,
        private graphApiService: GraphApiService,
        private userService: UserService,
        private personService: PersonService,
    ) {
        super(repo);
    }

    async azureSyncParent(dto: ParentRequestDTO) {
        const { username, personID } = dto;
        const password = CONSTANTS.PARENT_PASSWORD_DEFAULT;
        const email = username;
        const parent = await this.graphApiService.getParentInfo(dto.username);

        let sysUser = await this.userService.getSysUserByPersonID(personID);
        if (!sysUser) sysUser = await this.userService.getSysUserByUsername(username);
        if (parent.status === GraphApiResponseEnum.SUCCESS && parent?.response && !sysUser) {
            const dto = { rowID: 0, password, personID, email, azureID: parent.response?.id };
            await this.userService.createAzureParent(dto);
            const sysUser = await this.userService.createSysUser({
                personID: personID,
                username: username,
            });
            await this.userService.createSysUserSysRole({ sysUserID: sysUser.sysUserID, sysRoleID: RoleEnum.PARENT });
            return {};
        } else if (parent.status === GraphApiResponseEnum.SUCCESS && parent?.response) {
            await this.personService.updateAzureIDByPublicEduNumber({
                publicEduNumber: username,
                azureID: parent.response?.id,
            });
            await this.personService.updatePublicEduNumberByPersonID({
                personID,
                publicEduNumber: parent.response?.mail,
            });
            return {};
        }
        throw new DataNotFoundException();
    }
}
