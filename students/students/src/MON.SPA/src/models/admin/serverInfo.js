export class ServerInfo{
    constructor(obj = {}){
        this.osPlatform = obj.osPlatform || '';
        this.aspDotnetVersion = obj.aspDotnetVersion || '';
        this.allocated = obj.allocated || 0;
        this.privateMemorySize64 = obj.privateMemorySize64 || 0;
        this.virtualMemorySize64 = obj.virtualMemorySize64 || 0;
        this.workingSet64 = obj.workingSet64 || 0;
        this.cpuUsage = obj.cpuUsage || 0.0;
        this.processorCount = obj.processorCount || 0;
        this.minIOThreads = obj.minIOThreads || 0;
        this.minWorkerThreads = obj.minWorkerThreads || 0;
        this.maxIOThreads = obj.maxIOThreads || 0;
        this.maxWorkerThreads = obj.maxWorkerThreads || 0;
        this.availableIOThreads = obj.availableIOThreads || 0;
        this.availableWorkerThreads = obj.availableWorkerThreads || 0;
    }
}
