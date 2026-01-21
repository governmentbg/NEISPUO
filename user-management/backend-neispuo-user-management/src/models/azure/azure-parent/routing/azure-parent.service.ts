import { Inject, Injectable, NotFoundException, forwardRef } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { EnableDatabaseLogging } from 'src/common/decorators/enable-database-logging.decorator';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { EntityNotCreatedException } from 'src/common/exceptions/entity-not-created.exception';
import { ParentAlreadyExistsException } from 'src/common/exceptions/parent-already-exists.exception';
import { UserAlreadyExistsException } from 'src/common/exceptions/user-already-exists.exception';
import { AzureUserResponseFactory } from 'src/common/factories/azure-user-response-dto.factory';
import { EntitiesInGenerationService } from 'src/models/entities-in-generation/routing/entities-in-generation.service';
import { ParentAccessService } from 'src/models/parent-access/routing/parent-access.service';
import { PersonService } from 'src/models/person/routing/person.service';
import { UserService } from 'src/models/user/routing/user.service';
import { Connection, EntityManager } from 'typeorm';
import { ChildSchoolBookCode, ParentCreateRequestDTO } from '../../../../common/dto/requests/parent-create-request.dto';
import { AzureUsersService } from '../../azure-users/routing/azure-users.service';
import { AzureParentRepository } from '../azure-parent.repository';

@Injectable()
@EnableDatabaseLogging({
    includedMethods: ['createAzureParent'],
})
export class AzureParentService {
    constructor(
        private azureParentRepo: AzureParentRepository,
        private personService: PersonService,
        private parentAccessService: ParentAccessService,
        private azureUsersService: AzureUsersService,
        private usersService: UserService,
        @Inject(forwardRef(() => EntitiesInGenerationService))
        private entitiesInGenerationService: EntitiesInGenerationService,
        private connection: Connection,
    ) {}

    async createAzureParent(dto: ParentCreateRequestDTO, request: AuthedRequest) {
        // check if children are with valid personalID and book code (could register without children)
        // we assign this to a variable because of ESLint errors;
        let result;
        const parentDTO = { ...dto };
        const childrenIDs = await this.validateChildrenSchoolBookCodes(dto.childrenCodes);
        const entityInGenerationDTO = { identifier: parentDTO.email.toString() };
        let userExists = await this.entitiesInGenerationService.entitiesInGenerationExists(entityInGenerationDTO);
        if (!userExists) userExists = await this.usersService.userNameExists({ username: dto.email });
        if (!userExists) userExists = await this.azureUsersService.generatedUserExist({ username: dto.email });
        if (userExists) throw new ParentAlreadyExistsException();
        // create Person
        parentDTO.publicEduNumber = dto.email;
        await this.connection.transaction(async (manager) => {
            const person = await this.personService.createPerson(parentDTO, manager);
            parentDTO.personID = person.personID;
            // give parent access to his children school book (if any children)
            await this.createParentAccessToChildren(parentDTO.personID, childrenIDs, manager, request);
            // create User for Azure Job to process
            const azureParentDTO = AzureUserResponseFactory.createFromParentCreateRequestDTO(parentDTO);
            result = await this.azureParentRepo.insertAzureParent(azureParentDTO, manager, request);
            if (!result?.rowID) throw new EntityNotCreatedException();
            const { rowID } = result;
            azureParentDTO.rowID = rowID;
            const result2 = await this.azureUsersService.generateUsername(azureParentDTO, manager);
            if (this.azureUsersService.userHasFailedUserNameGeneration(result2)) throw new UserAlreadyExistsException();
            const sysUser = await this.usersService.createSysUser(
                {
                    personID: azureParentDTO.personID,
                    username: azureParentDTO.email,
                },
                manager,
            );
            await this.usersService.createSysUserSysRole(
                { sysUserID: sysUser.sysUserID, sysRoleID: RoleEnum.PARENT },
                manager,
            );
            this.entitiesInGenerationService.insertEntitiesInGeneration(entityInGenerationDTO);
        });
        return {
            data: { [CONSTANTS.RESPONSE_PARAM_NAME_USER_CREATED]: result?.rowID },
        };
    }

    async validateChildrenSchoolBookCodes(childrenCodes: ChildSchoolBookCode[]): Promise<number[]> {
        const childrenIDs = [];
        for (const schoolCode of childrenCodes) {
            const res = await this.personService.getPersonByPersonalIDAndSchoolCode(
                schoolCode.personalID,
                schoolCode.schoolBookCode,
            );
            if (!res?.personID) throw new NotFoundException();
            childrenIDs.push(res.personID);
        }

        return childrenIDs;
    }

    async createParentAccessToChildren(
        parentID: number,
        childrenIDs: number[],
        entityManager: EntityManager,
        request: AuthedRequest,
    ) {
        for (const childID of childrenIDs) {
            await this.parentAccessService.createParentAccessToChild(parentID, childID, entityManager, request);
        }
    }
}
