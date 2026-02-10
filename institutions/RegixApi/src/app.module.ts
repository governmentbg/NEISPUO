import { HttpModule } from "@nestjs/axios";
import { Module } from "@nestjs/common";
import { JwtModule } from "@nestjs/jwt";
import { AppController } from "./app.controller";
import { AppService } from "./app.service";
import { ConfigModule } from "@nestjs/config";
import { PassportModule } from "@nestjs/passport";
import { JwtStrategy } from "./jwt-strategy";

@Module({
  imports: [HttpModule, ConfigModule.forRoot({ isGlobal: true }), PassportModule.register({ defaultStrategy: "jwt" }), JwtModule.register({})],
  controllers: [AppController],
  providers: [AppService, JwtStrategy]
})
export class AppModule {}
