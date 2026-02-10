import { RIFlexField } from '@domain/ri-flex-field/ri-flex-field.entity';
import { RIInstitution } from '@domain/ri-institution/ri-institution.entity';
import { RIFlexFieldValue } from '../../ri-flex-field-value.entity';

export class RIFlexFieldValueDto extends RIFlexFieldValue {
    RIInstitution: RIInstitution;

    RIFlexField: RIFlexField;
}
