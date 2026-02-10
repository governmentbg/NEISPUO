import {
 Body, Controller, Param, Post, Req, UseGuards,
} from '@nestjs/common';
import { AuthedRequest } from '@shared/interfaces/authed-request.interface';
import { RIInstitutionDTO } from '../ri-institution.dto';
import { RIInstitutionGuard } from '../ri-institution.guard';
import { RIInstitutionMergeService } from './ri-institution-merge.service';

interface MergeDto {
    misToDelete: RIInstitutionDTO[];
    miToCreate: RIInstitutionDTO;
}

@UseGuards(RIInstitutionGuard)
@Controller('v1/ri-institution-merge')
export class RIInstitutionMergeController {
    constructor(public riInstitutionMergeService: RIInstitutionMergeService) {}

    /**
     * Сливане
     *
     * @param req
     * @param params
     * @param body
     */
    @Post()
    async mergeInstitutions(
        @Req() authedRequest: AuthedRequest,
        @Param() params: any,
        @Body() reqBody: MergeDto,
    ) {
        return await this.riInstitutionMergeService.mergeInstitutions(authedRequest, reqBody);
    }
}
