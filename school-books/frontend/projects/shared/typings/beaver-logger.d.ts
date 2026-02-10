declare module 'beaver-logger' {
  import { ZalgoPromise } from 'zalgo-promise';

  const LOG_LEVEL: {
    DEBUG: 'debug';
    INFO: 'info';
    WARN: 'warn';
    ERROR: 'error';
  };

  type TransportOptions = {
    url: string;
    method: string;
    headers: Record<string, string>;
    json: unknown;
    enableSendBeacon?: boolean;
  };

  type Transport = (transportOptions: TransportOptions) => ZalgoPromise<void>;

  type LoggerOptions = {
    url?: string;
    prefix?: string;
    logLevel?: typeof LOG_LEVEL[keyof typeof LOG_LEVEL];
    transport?: Transport;
    flushInterval?: number;
    enableSendBeacon?: boolean;
    amplitudeApiKey?: string;
  };

  type Payload = Record<string, unknown>;

  type Metric = {
    name: string;
    dimensions: Payload;
  };

  type ClientPayload = Payload;
  type Log = (name: string, payload?: ClientPayload) => LoggerType;
  type Track = (payload: ClientPayload) => LoggerType;
  type LogMetric = (payload: Metric) => LoggerType;
  type LogEvent = {
    level: typeof LOG_LEVEL[keyof typeof LOG_LEVEL];
    event: string;
    payload: Payload;
  };

  type Builder = (payload: Payload) => ClientPayload;
  type AddBuilder = (buidler: Builder) => LoggerType;

  type LoggerType = {
    debug: Log;
    info: Log;
    warn: Log;
    error: Log;

    track: Track;
    metric: LogMetric;

    flush: () => ZalgoPromise<void>;
    immediateFlush: () => ZalgoPromise<void>;

    addPayloadBuilder: AddBuilder;
    addMetaBuilder: AddBuilder;
    addTrackingBuilder: AddBuilder;
    addHeaderBuilder: AddBuilder;

    setTransport: (transport: Transport) => LoggerType;
    configure: (loggerOptions: LoggerOptions) => LoggerType;
  };

  function Logger(loggerOptions: LoggerOptions): LoggerType;
}
