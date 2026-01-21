import { Body, Controller, Post, Req, UseGuards } from '@nestjs/common';
import {
  Crud,
  CrudController,
  CrudOptions,
  CrudRequest,
  Override,
  ParsedBody,
  ParsedRequest,
} from '@nestjsx/crud';
import { AuthedRequest } from 'src/shared/interfaces/authed-request.interface';
import { Report } from '../report.entity';
import { ReportService } from '../report.service';
import { ReportGuard } from './report.guard';

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
    only: [
      'getManyBase',
      'getOneBase',
      'createOneBase',
      'replaceOneBase',
      'deleteOneBase',
    ],
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

@UseGuards(ReportGuard)
@Crud(reportsCrudOptions)
@Controller('v1/saved-reports')
export class ReportController implements CrudController<Report> {
  constructor(public service: ReportService) {}

  get base(): CrudController<Report> {
    return this;
  }

  @Override('createOneBase')
  @Post()
  async createReport(
    @Req() authedRequest: AuthedRequest,
    @Body() report: Report,
  ) {
    await this.service.createReport(report, authedRequest._authObject);

    if (report.SharedWith.length) {
      await this.service.validateShareWith(report, authedRequest._authObject);
    }

    return this.service.reportsRepository.save(report);
  }

  @Override('replaceOneBase')
  async replaceOne(
    @ParsedRequest() req: CrudRequest,
    @Req() authedRequest: AuthedRequest,
    @ParsedBody() report: Report,
  ) {
    if (report.SharedWith.length) {
      await this.service.validateShareWith(report, authedRequest._authObject);
    }

    return await this.base.replaceOneBase(req, report);
  }
}
