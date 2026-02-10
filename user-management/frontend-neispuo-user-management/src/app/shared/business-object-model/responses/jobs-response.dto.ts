export class JobsResponseDTO {
    jobID?: number;

    name?: string;

    cron?: string;

    isRunning?: number;

    isActive?: number;

    markedForStoppage?: number;
}
