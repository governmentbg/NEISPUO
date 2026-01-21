import { Controller, UseGuards } from '@nestjs/common';
import { Crud, CrudController } from '@nestjsx/crud';
import { IdentityProvider } from '../../identity-provider.entity';
import { IdentityProviderService } from './identity-provider.service';

// @Crud({
//     model: {
//         type: IdentityProvider
//     },
//     routes: {
//         only: ['getManyBase']
//     }
// })
@Controller('v1/identity-provider')
export class IdentityProviderController implements CrudController<IdentityProvider> {
    get base(): CrudController<IdentityProvider> {
        return this;
    }
    constructor(public service: IdentityProviderService) {}
}
