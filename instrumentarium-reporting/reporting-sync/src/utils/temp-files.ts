import * as fs from 'fs';
import { tmpdir } from 'os';
import { error } from '../services/logger.service';

const tempFiles: string[] = [];

export function registerTempFile(path: string): void {
  tempFiles.push(path);
}

export async function safeUnlink(path: string): Promise<void> {
  if (!fs.existsSync(path)) return;
  try {
    await fs.promises.unlink(path);
  } catch(err: any) {
    error(`Error deleting temp file ${path}:`, err)
  }
}

export async function cleanupTempFiles(): Promise<void> {
  const tmp = tmpdir();
  for (const file of tempFiles) {
    if (file.startsWith(tmp) && fs.existsSync(file)) {
      try {
        fs.unlinkSync(file);
      } catch(err: any) {
        error(`Error deleting temp file ${file}:`, err)
      }
    }
  }
  tempFiles.length = 0;
}


