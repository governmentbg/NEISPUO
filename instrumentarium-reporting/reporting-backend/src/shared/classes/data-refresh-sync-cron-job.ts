import { Injectable, Logger } from '@nestjs/common';
import { Cron } from '@nestjs/schedule';
import { EntityManager } from 'typeorm';

export abstract class DataRefreshSyncCronJob {
  protected readonly logger = new Logger(DataRefreshSyncCronJob.name);
  private isRunning = false;
  constructor(
    protected readonly sourceView: string,
    protected readonly targetTable: string,
    protected readonly targetTableColumns: string,
    protected entityManager: EntityManager,
  ) {}

  @Cron('0 0-23/1 * * *')
  public async runHourly() {
    if (this.isRunning) {
      return;
    }
    this.isRunning = true;
    await this.handleCron();
    this.isRunning = false;
  }

  private async handleCron() {
    await this.entityManager.transaction(async (manager) => {
      this.logger.debug(`Turncating table ${this.targetTable}...`);
      await manager.query(`TRUNCATE TABLE ${this.targetTable}`);
      this.logger.debug(`Populating table ${this.targetTable}...`);
      await manager.query(`
        INSERT INTO ${this.targetTable} WITH (TABLOCK) (${this.targetTableColumns})
        SELECT ${this.targetTableColumns} from ${this.sourceView}
      `);
      this.logger.debug(`Successfully populated table ${this.targetTable}.`);
    });
  }
}
