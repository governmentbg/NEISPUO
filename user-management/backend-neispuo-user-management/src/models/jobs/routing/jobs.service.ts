import { Injectable, Logger, OnApplicationBootstrap } from '@nestjs/common';
import { SchedulerRegistry } from '@nestjs/schedule';
import { CronJob, CronTime } from 'cron';
import { CronJobConfigResponseDTO } from 'src/common/dto/responses/cron-jobs-config-response.dto';
import { EntityNotCreatedException } from 'src/common/exceptions/entity-not-created.exception';
import { Connection } from 'typeorm';
import { JobsRepository } from '../jobs.repository';

@Injectable()
export class JobsService implements OnApplicationBootstrap {
    private logger = new Logger(JobsService.name);

    constructor(
        private schedulerRegistry: SchedulerRegistry,
        private jobsRepository: JobsRepository,
        private connection: Connection,
    ) {}

    async onApplicationBootstrap() {
        await this.initializeStartJobs();
    }

    async getAllCronJobsDB() {
        const crons = await this.jobsRepository.getAllJobConfigs();
        return crons;
    }

    async getAllCronJobs() {
        const crons = Array.from(this.schedulerRegistry.getCronJobs(), ([name, value]) => ({
            name: name,
            isRunning: value.running,
            cron: value,
            instance: value,
        }));
        return crons;
    }

    getCronJobByName(name: string) {
        return this.schedulerRegistry.getCronJob(name);
    }

    startJob(job: CronJob) {
        job.start();
    }

    stopJob(job: CronJob) {
        job.stop();
    }

    setJobCronByName(name: string, cronTime: any) {
        const job = this.getCronJobByName(name);
        const time = new CronTime(cronTime);
        job.setTime(time);
    }

    async initializeStartJobs() {
        const jobConfigs = await this.jobsRepository.getAllJobConfigs();
        for (const config of jobConfigs) {
            const { name, isActive, cron } = config;
            if (!this.schedulerRegistry.doesExists('cron', name)) continue;
            await this.setJobCronByName(name, cron);

            if (isActive) {
                await this.stopJobByName(name);
                await this.startJobByName(name);
            }
            if (!isActive) {
                await this.stopJobByName(name);
            }
        }
    }

    async startJobByName(name: string) {
        const job = this.getCronJobByName(name);
        this.startJob(job);
    }

    async stopJobByName(name: string) {
        const job = this.getCronJobByName(name);
        this.stopJob(job);
    }

    async updateJobByID(dto: CronJobConfigResponseDTO) {
        await this.connection.transaction(async (manager) => {
            dto.markedForRestart = 1;
            const jobConfig = await this.jobsRepository.updateJob(dto, manager);
            if (!jobConfig) throw new EntityNotCreatedException();
        });
    }

    async updateJobs() {
        const jobConfigs = await this.getAllCronJobsDB();
        for (const config of jobConfigs) {
            const { name, cron, isActive } = config;
            if (!this.schedulerRegistry.doesExists('cron', name)) continue;
            await this.setJobCronByName(name, cron);
            const job = await this.getCronJobByName(name);
            if (isActive) {
                await this.startJobByName(name);
            } else {
                await this.stopJobByName(name);
            }
        }
    }
}
