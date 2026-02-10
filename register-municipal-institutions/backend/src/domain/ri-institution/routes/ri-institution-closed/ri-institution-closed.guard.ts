import { Injectable } from '@nestjs/common';
import { RIInstitutionGuard } from '../ri-institution/ri-institution.guard';

@Injectable()
export class RIInstitutionClosedGuard extends RIInstitutionGuard {}
