import {
  Body,
  Controller,
  Delete,
  Get,
  Param,
  Post,
  Put,
  Req,
  UploadedFile,
  UseGuards,
  UseInterceptors,
} from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { JwksGuard } from 'src/shared/guards/jwks.guard';
import { UserGuide } from '../user-guides.entity';
import { UserGuidesService } from './user-guides.service';
import { FileInterceptor } from '@nestjs/platform-express';
import { AuthedRequest } from 'src/shared/dto/authed-request.interface';
import { UserGuideDTO } from 'src/shared/dto/user-guide.dto';
import { RoleGuard } from 'src/shared/guards/roles.guard';
import { RoleEnum } from 'src/shared/enums/role.enum';

@UseGuards(JwksGuard, RoleGuard([RoleEnum.MON_ADMIN, RoleEnum.CONSORTIUM_HELPDESK]))
@Crud({
  model: {
    type: UserGuide,
  },
  routes: {
    only: ['getManyBase', 'getOneBase'],
  },
  query: {
    join: {
      category: { eager: true },
    },
    exclude: ['fileContent'],
  },
})
@Controller('v1/user-guides-menu')
export class UserGuidesController implements CrudController<UserGuide> {
  constructor(public service: UserGuidesService) {}

  @Post()
  @UseInterceptors(FileInterceptor('file'))
  uploadFile(
    @Req() request: AuthedRequest,
    @Body() userGuideDTO: UserGuideDTO,
    @UploadedFile() file: Express.Multer.File,
  ) {
    if (file) {
      userGuideDTO.fileContent = file.buffer;
      userGuideDTO.mimeType = file.mimetype;
      userGuideDTO.filename = file.originalname;
    }
    return this.service.createUserGuide(request, userGuideDTO);
  }

  @Put(':id')
  @UseInterceptors(FileInterceptor('file'))
  updateUserGuide(
    @Req() request: AuthedRequest,
    @Param('id') id: number,
    @Body() userGuide: UserGuideDTO,
    @UploadedFile() file: Express.Multer.File,
  ) {
    if (file) {
      userGuide.fileContent = file.buffer;
      userGuide.mimeType = file.mimetype;
      userGuide.filename = file.originalname;
    }
    return this.service.updateUserGuideByID(request, id, userGuide);
  }

  @Get(':id')
  getFileContentByID(@Param('id') id: number) {
    return this.service.retrieveFileFromDatabase(id);
  }

  @Delete(':id')
  deleteUserGuideByID(@Req() request: AuthedRequest, @Param('id') id: number) {
    return this.service.deleteUserGuideByID(request, id);
  }
}
