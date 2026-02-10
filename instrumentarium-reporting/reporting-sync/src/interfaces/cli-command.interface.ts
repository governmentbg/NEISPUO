export interface CliCommand {
  name: string;
  description: string;
  action: () => Promise<void>;
} 