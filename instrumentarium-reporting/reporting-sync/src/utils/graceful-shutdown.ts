import { info } from '../services/logger.service';

export const setupGracefulShutdown = async (cleanupFn: () => Promise<void>): Promise<void> => {
  const shutdown = async () => {
    info('\nReceived shutdown signal, shutting down gracefully...');
    await cleanupFn();
    process.exit(0);
  };
  
  process.on('SIGINT', shutdown);
  process.on('SIGTERM', shutdown);
}; 
