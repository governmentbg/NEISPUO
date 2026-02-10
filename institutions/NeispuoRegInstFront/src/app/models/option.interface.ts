export interface Option {
  code: number | string;
  label: string;
  isValid?: boolean;
  formName?: string;
  eikObligatory?: boolean;
  instType?: number;
  instKind?: number;
  isDraft?: boolean;
  isAnullment?: boolean;
  
  disabled?: boolean;
}
