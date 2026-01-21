import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController, CrudOptions } from '@nestjsx/crud';
import { Report } from '../report.entity';
import { ReportService } from '../report.service';
import { SharedReportGuard } from './shared-report.guard';

export const reportsCrudOptions: CrudOptions = {
  model: {
    type: Report,
  },
  params: {
    ReportID: {
      field: 'ReportID',
      type: 'number',
      primary: true,
    },
  },
  routes: {
    only: ['getManyBase', 'getOneBase'],
  },
  query: {
    alwaysPaginate: true,
    sort: [
      {
        field: 'ReportID',
        order: 'DESC',
      },
    ],
    /**
          We need to add exclude on Primary Keys because otherwise the CRUD returns the 
          primary twice as an array which breaks update/delete services
      */
    join: {
      CreatedBy: {
        eager: true,
        allow: ['SysUserID', 'Username'],
        exclude: ['SysUserID'],
      },
    },
    exclude: ['ReportID'],
  },
};

@UseGuards(SharedReportGuard)
@Crud(reportsCrudOptions)
@Controller('v1/shared-reports')
export class SharedReportController implements CrudController<Report> {
  constructor(public service: ReportService) {}

  get base(): CrudController<Report> {
    return this;
  }
}
