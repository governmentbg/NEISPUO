import { Body, Controller, Post, Req } from '@nestjs/common';
import { ParentRegisterDTO } from 'src/shared/dto/parent-register.dto';
import { ParentRegisterService } from './parent-register.service';

@Controller('v1/parent-register')
export class ParentRegisterController {
  constructor(private parentRegisterService: ParentRegisterService) {}

  @Post()
  async createParent(@Req() req, @Body() createParentDTO: ParentRegisterDTO) {
    return await this.parentRegisterService.registerParent(createParentDTO);
  }
}
