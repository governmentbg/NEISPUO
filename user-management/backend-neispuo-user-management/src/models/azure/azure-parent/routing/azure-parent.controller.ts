import { Body, Controller, Post, Req, UseGuards } from '@nestjs/common';
import { ApiBearerAuth } from '@nestjs/swagger';
import { AuthedRequest } from 'src/common/dto/authed-request.interface';
import { ParentCreateRequestDTO } from 'src/common/dto/requests/parent-create-request.dto';
import { EncryptionService } from 'src/common/services/encryption/encryption.service';
import { AzureParentGuard } from './azure-parent.guard';
import { AzureParentService } from './azure-parent.service';

@UseGuards(AzureParentGuard)
@ApiBearerAuth()
@Controller('v1/azure-integrations/parent')
export class AzureParentController {
    constructor(private readonly azureParentService: AzureParentService) {}

    @Post()
    async createAzureParent(@Req() request: AuthedRequest, @Body() createAzureParentDTO: ParentCreateRequestDTO) {
        return this.azureParentService.createAzureParent(
            {
                ...createAzureParentDTO,
                password: EncryptionService.encryptString(createAzureParentDTO.password),
            },
            request,
        );
    }
}
