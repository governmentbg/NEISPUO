namespace MON.Services.Hangfire
{
    using global::Hangfire;
    using System;
    using System.Threading.Tasks;

    public interface IHangfireJob
    {
        /// <summary>
        /// The method can be used in unit tests or can be reused when the interface is injected to other places. 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task Run(IJobCancellationToken token);

        /// <summary>
        /// The method will be used in Hangfire job scheduler.
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        Task RunAtTimeOf(DateTime now);
    }
}
