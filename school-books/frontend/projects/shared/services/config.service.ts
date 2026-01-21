import { UserConfigService } from 'projects/sb-api-client/src/api/userConfig.service';
import { throwError } from '../utils/various';

export type UserConfig = {
  systemSchoolYear: number;
  tokenInstId?: number | null;
  tokenInstSchoolYear?: number | null;
};

export enum Project {
  TeachersApp = 1,
  StudentsApp = 2
}

export type LaunchConfiguration = {
  readonly project: Project;
  readonly teachersAppUrl: string;
  readonly studentsAppUrl: string;
};

export class ConfigService {
  private userConfig?: UserConfig;

  constructor(private userConfigService: UserConfigService, public readonly launchConfiguration: LaunchConfiguration) {}

  get currentUserConfig() {
    return this.userConfig ?? throwError('User config is not initialized!');
  }

  initialize() {
    return this.userConfigService
      .getUserConfig()
      .toPromise()
      .then((userConfig) => {
        this.userConfig = userConfig;
      });
  }
}
