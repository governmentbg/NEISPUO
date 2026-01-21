namespace MON.Models.Status
{
    public class ServerInfo
    {
        public string OsPlatform { get; set; }
        public string AspDotnetVersion { get; set; }
        // The memory occupied by objects.
        public long Allocated { get; set; }
        // The value returned by this property represents the current size of memory used by the process, in bytes, that 
        // cannot be shared with other processes.
        public long PrivateMemorySize64 { get; set; }
        public long VirtualMemorySize64 { get; set; }
        // The working set includes both shared and private data. The shared data includes the pages that contain all the 
        // instructions that the process executes, including instructions in the process modules and the system libraries.
        public long WorkingSet64 { get; set; }
        public double CpuUsage { get; set; }
        public int ProcessorCount { get; set; }
        public int MinWorkerThreads { get; set; }
        public int MaxWorkerThreads { get; set; }
        public int AvailableWorkerThreads { get; set; }
        
        public int MinIOThreads { get; set; }
        public int MaxIOThreads { get; set; }
        public int AvailableIOThreads { get; set; }
    }
}
