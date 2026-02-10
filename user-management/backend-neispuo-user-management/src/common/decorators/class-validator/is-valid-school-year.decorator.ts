import { ValidationArguments, ValidationOptions, registerDecorator } from 'class-validator';
import { SchoolYearService } from 'src/common/services/school-year/school-year.service';

export function IsValidSchoolYear(validationOptions?: ValidationOptions) {
    return function (object: Object, propertyName: string) {
        registerDecorator({
            name: 'isValidSchoolYear',
            target: object.constructor,
            propertyName: propertyName,
            options: validationOptions,
            validator: {
                validate(value: any, args: ValidationArguments) {
                    if (!value) return true;
                    return SchoolYearService.isValidSchoolYear(value);
                },
                defaultMessage(args: ValidationArguments) {
                    return 'Invalid school year format. Use format like "2022" or "2022/2023"';
                },
            },
        });
    };
}
