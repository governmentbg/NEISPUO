import { MigrationStatusEnum } from "src/enums/migration-status.enum";

export interface Migration {
  id: number;
  name: string;
  applied_at: Date;
  status: MigrationStatusEnum;
  error_message?: string;
  execution_time_ms: number;
}
