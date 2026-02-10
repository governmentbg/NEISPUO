import { Controller, Get, HttpStatus, Post, Res } from '@nestjs/common';
import { Response } from 'express';
import { CubeJSProxyService } from './cubejs-proxy.service';
import * as exceljs from 'exceljs';
import { LessThan } from 'typeorm';

@Controller('/v1/cubejs')
export class CubeJSProxyController {
  constructor(private readonly cubeJSProxyService: CubeJSProxyService) { }
  @Get('meta')
  async getCubeJSMetaEndpoint() {
    return this.cubeJSProxyService.getFilteredCubeJSMetaResponse();
  }

  @Get('load')
  async getCubeJSLoadGetEndpoint() {
    return this.cubeJSProxyService.getFilteredCubeJSLoad();
  }

  @Post('load')
  async getCubeJSLoadPostEndpoint(@Res() response: Response) {
    /**
     * Modify status
     */
    return response
      .status(HttpStatus.OK)
      .send(await this.cubeJSProxyService.getFilteredCubeJSLoad());
  }

  @Post('download-excel')
  async streamExcel(@Res() res: Response) {
    res.status(200);
    res.set({
      'Content-Type': 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      'Content-Disposition': 'attachment; filename=report.xlsx',
    });

    try {
      const workbook = new exceljs.stream.xlsx.WorkbookWriter({ stream: res });
      const worksheet = workbook.addWorksheet('Export');

      let columnsSet = false;

      for await (const row of this.cubeJSProxyService.streamCubeRows({ limit: 2000 })) {
        if (!columnsSet) {
          worksheet.columns = Object.keys(row).map((key) => ({
            header: key,
            key: key,
          }));
          columnsSet = true;
        }
        worksheet.addRow(row).commit();
      }

      await worksheet.commit();
      await workbook.commit();
      // Do not send any more responses here
    } catch (err) {
      // Log the error, but do not try to send a response
      console.error('Error during Excel streaming:', err);
      // Optionally, destroy the response if possible
      res.destroy();
    }
  }
}
