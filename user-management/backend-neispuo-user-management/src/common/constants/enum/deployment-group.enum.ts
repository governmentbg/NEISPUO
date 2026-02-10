export enum DeploymentGroup {
    ARCHIVE = 'ARCHIVE', //Archive jobs
    RESTART = 'RESTART', //Restart workflows jobs
    SEND = 'SEND', //Create workflow jobs
    CHECK_ENROLLMENTS = 'CHECK_ENROLLMENTS', //Check workflow jobs
    CHECK = 'CHECK', //Check workflow jobs
    SYNC = 'SYNC', //synv workflow jobs
    REVERT = 'REVERT', //revert stuck workflows in inProcessing = 1
    DAILY_RUN = 'DAILY_RUN', //jobs that sync missed things from Consortium members
    OTHER = 'OTHER', //Delete jobs,
    CRUD = 'CRUD', // on this deployment people will call the api
}
