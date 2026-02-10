import { Module } from '@nestjs/common';
import { HttpModule } from '@nestjs/axios';
import { CubeJSProxyController } from './proxy/cubejs-proxy.controller';
import { CubeJSProxyService } from './proxy/cubejs-proxy.service';
import { SchemaRoleAccessModule } from 'src/domain/schema-role-access/schema-role-access.module';

@Module({
  imports: [HttpModule, SchemaRoleAccessModule],
  controllers: [CubeJSProxyController],
  providers: [CubeJSProxyService],
})
export class CubeJSModule {}
