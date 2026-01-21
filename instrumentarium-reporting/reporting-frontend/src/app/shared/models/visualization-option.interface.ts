import { VisualizationTypeEnum } from '@shared/enums/visualization-type.enum';

export interface VisualizationOption {
  label: string;
  value: VisualizationTypeEnum;
  iconClass: string;
}
