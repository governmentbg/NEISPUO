export interface DatabaseJob {
  name: string;
  description: string;
  schedule: string;
  enabled: boolean;
  error_message: string;
  created_at: Date;
  updated_at: Date;
} 