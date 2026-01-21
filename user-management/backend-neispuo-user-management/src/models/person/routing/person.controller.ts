import { Controller, Get, Param, Req, UseGuards } from '@nestjs/common';
import { ApiBearerAuth, ApiOkResponse } from '@nestjs/swagger';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { ParentChildrenResponseDTO } from 'src/common/dto/responses/parent-children-response.dto';
import { PersonResponseDTO } from 'src/common/dto/responses/person-response.dto';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { PersonService } from './person.service';

@ApiBearerAuth()
@Controller('/v1/person')
export class PersonController {
    constructor(private personService: PersonService) {}

    @Get('children')
    @ApiOkResponse({ type: ParentChildrenResponseDTO })
    async getChildren(@Req() req: AuthedRequest): Promise<ParentChildrenResponseDTO[]> {
        return this.personService.getPersonChildren(req._authObject.selectedRole.PersonID);
    }

    @UseGuards(RoleGuard([RoleEnum.MON_ADMIN]))
    @Get('/:personId')
    async getPerson(@Param('personId') personId: number): Promise<PersonResponseDTO> {
        return this.personService.getPersonByPersonID(personId);
    }
}
