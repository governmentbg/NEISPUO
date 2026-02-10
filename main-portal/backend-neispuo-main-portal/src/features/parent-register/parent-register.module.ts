import { Module } from '@nestjs/common';
import { ParentRegisterController } from './routes/parent-register/parent-register.controller';
import { ParentRegisterService } from './routes/parent-register/parent-register.service';

@Module({
  imports: [],
  providers: [ParentRegisterService],
  exports: [],
  controllers: [ParentRegisterController],
})
export class ParentRegisterModule {}
