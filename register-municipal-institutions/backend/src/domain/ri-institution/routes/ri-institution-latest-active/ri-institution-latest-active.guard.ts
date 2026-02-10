import { Injectable } from '@nestjs/common';
import { RIInstitutionGuard } from '../ri-institution/ri-institution.guard';

@Injectable()
export class RIInstitutionLatestActiveGuard extends RIInstitutionGuard {}
