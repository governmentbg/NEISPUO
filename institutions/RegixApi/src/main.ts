import { NestFactory } from "@nestjs/core";
import { JwtService } from "@nestjs/jwt";
import { AppModule } from "./app.module";
import { JwtAuthGuard } from "./auth.guard";

async function bootstrap() {
  const app = await NestFactory.create(AppModule);
  app.useGlobalGuards(new JwtAuthGuard(new JwtService()));
  app.enableCors();
  await app.listen(3333);
}
bootstrap();
