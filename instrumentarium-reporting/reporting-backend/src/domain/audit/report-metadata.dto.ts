export interface ReportMetadataDTO {
  id: number;
  name: string;
  endpoint: string;
  action: 'READ' | 'CREATE' | 'UPDATE' | 'DOWNLOAD';
}
