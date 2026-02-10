import { Component, OnInit } from '@angular/core';
import { CONSTANTS } from '@shared/constants';
import { JobsTableColumnsConfig, JobsTableItemsPerPage } from 'src/app/configs/datatables-config';
import { JobsResponseDTO } from '@shared/business-object-model/responses/jobs-response.dto';
import { JobsService } from 'src/app/shared/services/jobs.service';

@Component({
    selector: 'app-jobs-page',
    templateUrl: './jobs-page.component.html',
    styleUrls: ['./jobs-page.component.scss'],
})
export class JobsPageComponent implements OnInit {
    CONSTANTS = CONSTANTS;

    jobs!: JobsResponseDTO[];

    first!: number;

    itemsPerPage!: number;

    cols!: any;

    constructor(private jobsService: JobsService) {}

    filterDelay: number = CONSTANTS.PRIMENG_CONFIG_FILTER_DELAY_MILISECS;

    async ngOnInit(): Promise<void> {
        this.refreshTableData();
    }

    async saveJob(job: JobsResponseDTO) {
        const result = await this.jobsService.updateJob(job);
    }

    async refreshTableData() {
        this.first = 0;
        this.cols = JobsTableColumnsConfig;
        this.itemsPerPage = JobsTableItemsPerPage;
        const result = await this.jobsService.getAllJobs();
        this.jobs = result.payload;
    }
}
