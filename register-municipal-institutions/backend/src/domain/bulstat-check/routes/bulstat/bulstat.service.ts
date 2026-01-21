import * as soap from 'soap';
import {
    Inject,
    Injectable,
    InternalServerErrorException,
    NotFoundException,
} from '@nestjs/common';
import { RegixOrganization } from '@domain/bulstat-check/regix-organization.interface';
import { RIInstitutionDTO } from '@domain/ri-institution/routes/ri-institution/ri-institution.dto';
import { getRepository } from 'typeorm';
import { Town } from '@domain/town/town.entity';
import { Logger } from 'winston';
import { WINSTON_MODULE_PROVIDER } from 'nest-winston';

@Injectable()
export class BulstatService {
    private readonly privateKey = Buffer.from(process.env.REGIX_PRIVATE_KEY);

    private readonly crt = Buffer.from(process.env.REGIX_CRT);

    private readonly action = process.env.REGIX_ACTION;

    private readonly url = process.env.REGIX_API_URL;

    private readonly operation = process.env.REGIX_BULSTAT_OPERATION;

    private client: any = null;

    constructor(@Inject(WINSTON_MODULE_PROVIDER) private readonly logger: Logger) {}

    public async init() {
        this.client = await soap.createClientAsync(`${process.cwd()}/WSDL.xml`, {
            endpoint: this.url,
            forceSoap12Headers: true,
        });

        const certSecurity = new soap.ClientSSLSecurity(
            Buffer.from(this.privateKey),
            Buffer.from(this.crt),
        );

        const soapHeaders = {
            'wsa:Action': this.action,
            'wsa:To': this.url,
        };

        this.client.addSoapHeader(soapHeaders, '', 'wsa', 'http://www.w3.org/2005/08/addressing');
        this.client.setSecurity(certSecurity);
    }

    async getBulstat(uic: number) {
        try {
            let result = null;
            result = await this.client.ExecuteSynchronousAsync(
                {
                    _xml: `
                            <ExecuteSynchronous xmlns="http://tempuri.org/">
                            <request>
                                <Operation>${this.operation}</Operation>
                                <Argument>
                                    <ActualStateRequest xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://egov.bg/RegiX/AV/TR/ActualStateRequest">
                                        <UIC>${uic}</UIC>
                                    </ActualStateRequest>
                                </Argument>
                            </request>
                            </ExecuteSynchronous>
                    `,
                },
                {
                    timeout: 10000,
                },
            );
            const response: RegixOrganization = result
                ? result[0]?.ExecuteSynchronousResult?.Data?.Response?.ActualStateResponse
                : null;

            if (!response) {
                this.logger.info(result);
                throw new NotFoundException(`Could not find organization for UIC ${uic}`);
            }

            const town = await getRepository(Town).findOne(
                /* eslint no-unsafe-optional-chaining: "error" */
                {
                    Code: +response?.Seat?.Address?.SettlementEKATTE,
                },
                { relations: ['Municipality', 'Municipality.Region'] },
            );

            const miTransformedData = {
                Bulstat: response.UIC,
                Name: response.Company,
                TRAddress: `${response.Seat.Address.Settlement} ${response.Seat.Address.Street}  ${response.Seat.Address.StreetNumber}`,
                TRPostCode: +response.Seat.Address.PostCode,
                Town: town,
            } as RIInstitutionDTO;

            return miTransformedData;
        } catch (e) {
            this.logger.error(e);
            throw new InternalServerErrorException('Failed to fetch Bulstat UIC.');
        }
    }
}
