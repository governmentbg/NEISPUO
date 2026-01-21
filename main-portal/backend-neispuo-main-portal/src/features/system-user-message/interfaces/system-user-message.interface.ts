export interface SystemUserMessageWithRoles {
  id: number;
  title: string;
  content: string;
  startDate: Date;
  endDate: Date;
  roles: {
    id: number;
    name: string;
    description: string;
  }[];
}
