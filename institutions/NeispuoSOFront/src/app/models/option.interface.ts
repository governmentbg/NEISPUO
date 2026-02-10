export interface Option {
  code: number | string;
  label: string;
  isValid?: boolean;
  instType?: number;
  
  disabled?: boolean;
}
