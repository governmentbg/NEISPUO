export interface SystemMessageForm {
  title: string;
  content: string;
  roles: number[];
  startDate: Date | null;
  endDate: Date | null;
}

export interface SystemMessagePayload {
  id?: number;
  title: string;
  content: string;
  roles: string;
  startDate: string | null;
  endDate: string | null;
}
