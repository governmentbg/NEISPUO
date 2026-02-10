import { startCronService } from '../services/cron.service';
import { error } from '../services/logger.service';

async function main() {
  try {
    await startCronService();
  } catch (err) {
    error('Cron service with error:', err);
    process.exit(1);
  }
}

main();
