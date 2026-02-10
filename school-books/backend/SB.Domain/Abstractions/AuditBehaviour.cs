namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SB.Common;
using Microsoft.Extensions.DependencyInjection;
using Autofac;

public class AuditBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<AuditBehaviour<TRequest, TResponse>> logger;
    private readonly IRequestContext requestContext;

    public AuditBehaviour(
        IServiceProvider serviceProvider,
        IRequestContext requestContext)
    {
        ILoggerFactory loggerFactory =
            serviceProvider.GetRequiredService<ILifetimeScope>()
            .ResolveNamed<ILoggerFactory>("AuditLogLoggerFactoryName");
        this.logger = loggerFactory.CreateLogger<AuditBehaviour<TRequest, TResponse>>();
        this.requestContext = requestContext;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response;

        try
        {
            response = await next();
        }
        catch (Exception ex)
        {
            if (ex is OperationCanceledException)
            {
                this.AuditLog(LogLevel.Information, null, "execution canceled", request, default);
            }
            else
            {
                this.AuditLog(LogLevel.Error, ex, "execution failed", request, default);
            }

            throw;
        }

        this.AuditLog(LogLevel.Information, null, "executed", request, response);

        return response;
    }

    private void AuditLog(LogLevel logLevel, Exception? exception, string status, TRequest request, TResponse? response)
    {
        void Log(IDictionary<string, object?> auditContext, LogLevel logLevel, Exception? exception, string message)
        {
            using (this.logger.BeginScope(auditContext))
            {
                #pragma warning disable CA2254 // Template should be a static expression
                this.logger.Log(logLevel, exception, message);
                #pragma warning restore CA2254 // Template should be a static expression
            }
        }

        string commandName = request.GetType().Name;
        string auditAction = $"{commandName} {status}";

        // using Newtonsoft.Json to serialize [JsonIgnore] properties as well
        // as there is no way to make System.Text.Json serializer to ignore these attributes
        string commandJson = JsonConvert.SerializeObject(request);

        Dictionary<string, object?> auditContext = new()
        {
            { "AuditAction", auditAction.Truncate(50) },
            { "CommandJson", commandJson },
            { "AuditModuleId", this.requestContext.AuditModuleId },
            { "RequestId", this.requestContext.RequestId },
            { "RemoteIpAddress", this.requestContext.RemoteIpAddress },
            { "UserAgent", this.requestContext.UserAgent }
        };

        if (this.requestContext.IsAuthenticated)
        {
            auditContext.Add("SysUserId", this.requestContext.SysUserId);
            auditContext.Add("SysRoleId", this.requestContext.SysRoleId);
            auditContext.Add("LoginSessionId", this.requestContext.LoginSessionId);
            auditContext.Add("Username", this.requestContext.Username);
            auditContext.Add("FirstName", this.requestContext.FirstName);
            auditContext.Add("MiddleName", this.requestContext.MiddleName);
            auditContext.Add("LastName", this.requestContext.LastName);
        }

        if (request is IAuditedCommand auditedCommand)
        {
            auditContext.Add("SchoolYear", auditedCommand.SchoolYear);
            auditContext.Add("InstId", auditedCommand.InstId);
            auditContext.Add("ObjectName", auditedCommand.ObjectName);
            auditContext.Add("ObjectId", auditedCommand.ObjectId);
            auditContext.Add("PersonId", auditedCommand.PersonId);
        }

        // using id > 0 because TResponse is unconstrained
        // see https://github.com/dotnet/roslyn/issues/53139
        if (request is IAuditedCreateCommand && response is int id && id > 0)
        {
            auditContext.AddOrUpdate("ObjectId", id);
            Log(auditContext, logLevel, exception, "Auditing IAuditedCreateCommand execution");
        }
        else if (request is IAuditedCreateMultipleCommand && response is int[] ids)
        {
            foreach (int oid in ids)
            {
                auditContext.AddOrUpdate("ObjectId", oid);
                Log(auditContext, logLevel, exception, "Auditing IAuditedCreateMultipleCommand execution");
            }
        }
        else if (request is IAuditedUpdateMultipleCommand updReq)
        {
            foreach (int oid in updReq.ObjectIds)
            {
                auditContext.AddOrUpdate("ObjectId", oid);
                Log(auditContext, logLevel, exception, "Auditing IAuditedUpdateMultipleCommand execution");
            }
        }
        else
        {
            Log(auditContext, logLevel, exception, "Auditing command execution");
        }
    }
}
