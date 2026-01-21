import { startCli } from '../services/cli.service';
import { error } from '../services/logger.service';

async function main() {
  try {
    await startCli();
  } catch (err) {
    error('CLI failed with error:', err);
    process.exit(1);
  }
}

main();
