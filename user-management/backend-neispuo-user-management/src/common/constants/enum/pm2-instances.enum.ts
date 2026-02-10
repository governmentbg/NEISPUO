export enum PM2Instances {
    ZERO = '0', //Archive jobs
    FIRST = '1', //Check jobs
    SECOND = '2', //Create jobs, Revert jobs
    THIRD = '3', //Delete jobs, token-refresh job, update-institution job, Sync jobs
    // FOURTH = '4', //Revert jobs
    // FIFTH = '5', //Sync jobs
}
