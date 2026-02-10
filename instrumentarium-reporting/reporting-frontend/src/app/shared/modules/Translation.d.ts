import { Translation } from 'primeng/api/translation';
//Module Augmentation
declare module 'primeng/api/translation' {
  interface Translation {
    between?: string; // it seems PrimeNG forgot to add the 'between' filter match mode operator to translations
    notBetween?: string; // Custom PrimeNG filter match mode operator that matches CubeJS notInDateRange
  }
}
