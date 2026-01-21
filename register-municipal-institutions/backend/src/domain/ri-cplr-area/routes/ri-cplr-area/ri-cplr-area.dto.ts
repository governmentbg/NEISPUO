import { RIProcedure } from '@domain/ri-procedure/ri-procedure.entity';
import { IsNotEmpty, MinLength } from 'class-validator';
import { RICPLRArea } from '../../ri-cplr-area.entity';

export class RICPLRAreaDTO extends RICPLRArea {
    RICPLRAreaID: number;

    @IsNotEmpty()
    Name: string;

    RIProcedure: RIProcedure;
}