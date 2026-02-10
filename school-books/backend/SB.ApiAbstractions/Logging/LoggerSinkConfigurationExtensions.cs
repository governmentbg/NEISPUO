namespace SB.ApiAbstractions;

using Prometheus;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.PeriodicBatching;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

#pragma warning disable CA2000 // Dispose objects before losing scope

public static class LoggerSinkConfigurationExtensions
{
    public static LoggerConfiguration CustomMSSqlServer(
        this LoggerSinkConfiguration loggerConfiguration,
        string connectionString,
        MSSqlServerSinkOptions sinkOptions,
        int queueLimit,
        IFormatProvider? formatProvider = null,
        ColumnOptions? columnOptions = null,
        ITextFormatter? logEventFormatter = null)
    {
        SinkObserver sinkObserver =
            new(connectionString,
                sinkOptions,
                formatProvider,
                columnOptions,
                logEventFormatter);

        PeriodicBatchingSink periodicSink = new(
            sinkObserver,
            new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = sinkOptions.BatchPostingLimit,
                Period = sinkOptions.BatchPeriod,
                EagerlyEmitFirstEvent = sinkOptions.EagerlyEmitFirstEvent,
                QueueLimit = queueLimit,
            });

        sinkObserver.OuterSink = periodicSink;

        return loggerConfiguration.Sink(periodicSink);
    }

    private static Gauge QueueDepthTotal { get; set; } =
        Metrics.CreateGauge(
            "serilog_queue_depth_total",
            "Serilog queue depth gauge",
            new[] { "table_name" });
    private static Counter DroppedBatchesTotal { get; set; } =
        Metrics.CreateCounter(
            "serilog_dropped_batches_total",
            "Serilog dropped batches counter",
            new[] { "table_name" });
    private static Counter DroppedQueuesTotal { get; set; } =
        Metrics.CreateCounter(
            "serilog_dropped_queues_total",
            "Serilog dropped queues counter",
            new[] { "table_name" });
    private static Histogram ItemsPerBatchTotal { get; set; } =
        Metrics.CreateHistogram(
            "serilog_items_per_batch_total",
            "Serilog items per batch histogram",
            new HistogramConfiguration
            {
                // 1 to 8K buckets
                Buckets = Histogram.ExponentialBuckets(1, 2, 14),
                LabelNames = new[] { "table_name" }
            });
    private static Counter SuccessfulBatchesTotal { get; set; } =
        Metrics.CreateCounter(
            "serilog_successful_batches_total",
            "Serilog successful batches counter",
            new[] { "table_name" });
    private static Counter FailedBatchesTotal { get; set; } =
        Metrics.CreateCounter(
            "serilog_failed_batches_total",
            "Serilog failed batches counter",
            new[] { "table_name" });
    private static Counter SuccessfulBatchItemsTotal { get; set; } =
        Metrics.CreateCounter(
            "serilog_successful_batch_items_total",
            "Serilog successful batch items counter",
            new[] { "table_name" });
    private static Counter FailedBatchItemsTotal { get; set; } =
        Metrics.CreateCounter(
            "serilog_failed_batch_items_total",
            "Serilog failed batch items counter",
            new[] { "table_name" });

    private class SinkObserver : IBatchedLogEventSink, IDisposable
    {
        // copied from
        // https://github.com/serilog/serilog-sinks-periodicbatching/blob/dev/src/Serilog.Sinks.PeriodicBatching/Sinks/PeriodicBatching/BatchedConnectionStatus.c
        private const int FailuresBeforeDroppingBatch = 8;
        private const int FailuresBeforeDroppingQueue = 10;

        private readonly MSSqlServerSink innerSink;
        private readonly string tableName;

        private int failuresSinceSuccessfulBatch;

        public SinkObserver(
            string connectionString,
            MSSqlServerSinkOptions sinkOptions,
            IFormatProvider? formatProvider = null,
            ColumnOptions? columnOptions = null,
            ITextFormatter? logEventFormatter = null)
        {
            this.innerSink = new MSSqlServerSink(
                connectionString,
                sinkOptions,
                formatProvider,
                columnOptions,
                logEventFormatter);
            this.tableName = $"{sinkOptions.SchemaName ?? MSSqlServerSink.DefaultSchemaName}.{sinkOptions.TableName}";
        }

        public PeriodicBatchingSink? OuterSink { get; set; }

        public void Dispose()
        {
            // disposing only the inner sink as this
            // method is called by the outer sink
            // OuterSink.Dispose => SinkObserver.Dispose => innerSink.Dispose
            this.innerSink.Dispose();
        }

        public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            int batchCount = batch.Count();
            try
            {
                QueueDepthTotal.WithLabels(this.tableName).Set(this.GetQueueDepth());
                ItemsPerBatchTotal.WithLabels(this.tableName).Observe(batchCount);

                await this.innerSink.EmitBatchAsync(batch);

                this.failuresSinceSuccessfulBatch = 0;

                SuccessfulBatchesTotal.WithLabels(this.tableName).Inc();
                SuccessfulBatchItemsTotal.WithLabels(this.tableName).Inc(batchCount);
            }
            catch
            {
                this.failuresSinceSuccessfulBatch++;

                if (this.failuresSinceSuccessfulBatch >= FailuresBeforeDroppingBatch)
                {
                    DroppedBatchesTotal.WithLabels(this.tableName).Inc();
                }

                if (this.failuresSinceSuccessfulBatch >= FailuresBeforeDroppingQueue)
                {
                    DroppedQueuesTotal.WithLabels(this.tableName).Inc();
                }

                FailedBatchesTotal.WithLabels(this.tableName).Inc();
                FailedBatchItemsTotal.WithLabels(this.tableName).Inc(batchCount);

                throw;
            }
        }

        public Task OnEmptyBatchAsync()
        {
            QueueDepthTotal.WithLabels(this.tableName).Set(this.GetQueueDepth());
            return Task.CompletedTask;
        }

        private FieldInfo? periodicBatchingSinkQueueMemberInfo;
        private FieldInfo? boundedConcurrentQueueQueueMemberInfo;
        private int GetQueueDepth()
        {
            if (this.OuterSink == null)
            {
                throw new Exception("OuterSink is not initialized");
            }

            if (this.periodicBatchingSinkQueueMemberInfo == null)
            {
                this.periodicBatchingSinkQueueMemberInfo =
                    typeof(PeriodicBatchingSink)
                    .GetField("_queue", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?? throw new Exception("Unable to find _queue field in PeriodicBatchingSink");
            }

            if (this.boundedConcurrentQueueQueueMemberInfo == null)
            {
                this.boundedConcurrentQueueQueueMemberInfo =
                    Assembly.Load("Serilog.Sinks.PeriodicBatching")
                    ?.GetType("Serilog.Sinks.PeriodicBatching.BoundedConcurrentQueue`1")
                    ?.MakeGenericType(typeof(LogEvent))
                    ?.GetField("_queue", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?? throw new Exception("Unable to find _queue field in BoundedConcurrentQueue<LogEvent>");
            }

            var boundedConcurrentQueue =
                this.periodicBatchingSinkQueueMemberInfo.GetValue(this.OuterSink)
                ?? throw new Exception("Unable to get value of _queue field in PeriodicBatchingSink");

            var concurrentQueue = (ConcurrentQueue<LogEvent>)(
                this.boundedConcurrentQueueQueueMemberInfo.GetValue(boundedConcurrentQueue)
                ?? throw new Exception("Unable to get value of _queue field in BoundedConcurrentQueue<LogEvent>"));

            return concurrentQueue.Count;
        }
    }
}
