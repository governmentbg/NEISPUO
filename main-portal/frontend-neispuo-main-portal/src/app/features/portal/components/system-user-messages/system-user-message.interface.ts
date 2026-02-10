import { SysRole } from '@shared/interfaces/sys-role.interface';

export interface SystemUserMessage {
  id?: number;
  title: string;
  content: string;
  startDate: string;
  endDate: string;
  roles: SysRole[];
}
