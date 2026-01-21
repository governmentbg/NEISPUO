import { Injectable } from '@nestjs/common';
import { Interval } from '@nestjs/schedule';
import { CONSTANTS } from 'src/common/constants/constants';
import { VariablesService } from 'src/models/variables/routing/variables.service';

@Injectable()
export class UpdateJobTopValuesInterval {
    constructor(private variablesService: VariablesService) {}

    @Interval(CONSTANTS.JOB_INTERVAL_NAME_UPDATE_JOB_TOP_VALUES, CONSTANTS.JOB_INTERVAL_JOB_UPDATE_TOP_VALUES)
    async handleCron() {
        /**
         * Always execute this job as it's responsible for the update of the top values in from the variables table.
         */
        await this.variablesService.loadVariables();
    }
}
