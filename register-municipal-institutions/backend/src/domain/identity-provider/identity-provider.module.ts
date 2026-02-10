import { Module } from '@nestjs/common';
import { IdentityProviderService } from './routes/identity-provider/identity-provider.service';
import { IdentityProviderController } from './routes/identity-provider/identity-provider.controller';
import { TypeOrmModule } from '@nestjs/typeorm';
import { IdentityProvider } from './identity-provider.entity';

@Module({
    imports: [TypeOrmModule.forFeature([IdentityProvider])],
    exports: [TypeOrmModule],

    controllers: [IdentityProviderController],
    providers: [IdentityProviderService]
})
export class IdentityProviderModule {}
