import {
  Controller,
  UseGuards,
  Get,
  Param,
  Res,
  InternalServerErrorException,
  NotFoundException,
  Logger,
} from '@nestjs/common';
import { Response } from 'express';
import { JwksGuard } from 'src/shared/guards/jwks.guard';
import { UserGuidesService } from 'src/features/user-guides/routes/user-guides.service';

@UseGuards(JwksGuard)
@Controller('v1/file')
export class FileController {
  constructor(private userGuidesService: UserGuidesService) {}

  private readonly logger = new Logger(FileController.name);

  @Get('/download/:fileID')
  async getFile(
    @Param('fileID') userGuideID: number,
    @Res() response: Response,
  ) {
    try {
      const userGuide = await this.userGuidesService.getUserGuideByID(
        userGuideID,
      );

      if (!userGuide) {
        throw new NotFoundException(
          `File with userGuideID: ${userGuideID} was not found.`,
        );
      }

      response.set({
        'Content-Type': userGuide.mimeType,
        'Content-Disposition': `attachment; filename*=UTF-8''${encodeURI(
          userGuide.toString(),
        )}`,
      });
      response.send(userGuide.fileContent);
    } catch (error) {
      this.logger.error(error);
      throw new InternalServerErrorException(
        `Could not retrieve file with fileID: ${userGuideID}: ${error.message}`,
      );
    }
  }
}
