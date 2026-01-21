import { Injectable } from '@nestjs/common';
import { RIInstitutionService } from '../ri-institution/ri-institution.service';

@Injectable()
export class RIInstitutionClosedService extends RIInstitutionService {}
