import { VisualizationTypeEnum } from '@shared/enums/visualization-type.enum';
import { RoleEnum } from '@shared/enums/role.enum';

export class Report {
  ReportID?: number;
  Name: string;
  Description: string;
  DatabaseView: string;
  SharedWith: RoleEnum[];
  Visualization: VisualizationTypeEnum;
  Query: any;
  CreatedBy: any;
}
