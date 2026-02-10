import { Body, Controller, Get, Logger, Post, UseGuards } from '@nestjs/common';
import { RoleEnum } from 'src/common/constants/enum/role.enum';
import { CronJobConfigResponseDTO } from 'src/common/dto/responses/cron-jobs-config-response.dto';
import { UserManagementResponse } from 'src/common/dto/responses/user-management.response';
import { RoleGuard } from 'src/common/guards/roles.guard';
import { JobsService } from './jobs.service';

@UseGuards(RoleGuard([RoleEnum.MON_ADMIN]))
@Controller('v1/jobs')
export class JobsController {
    private readonly logger = new Logger(JobsController.name);

    constructor(private jobsService: JobsService) {}

    @Get('')
    async getAllCronJobs() {
        const result = await this.jobsService.getAllCronJobsDB();
        const response = new UserManagementResponse(result);
        return response;
    }

    @Post('update')
    async updateJobByName(@Body() dto: CronJobConfigResponseDTO) {
        const result = await this.jobsService.updateJobByID(dto);
        const response = new UserManagementResponse(result);
        return response;
    }
}
