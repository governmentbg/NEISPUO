import { Pipe, PipeTransform } from '@angular/core';
// this pipe is used to return  a nested element from an array with a (dot notation string)
// e.g you have an array student[] and have a string properties.roles.role and whish to get the element from the array
@Pipe({ name: 'booleanPipe' })
export class BooleanPipe implements PipeTransform {
    index(obj: any, i: any) {
        return obj[i];
    }

    transform(numberValue: number): any {
        return numberValue > 0;
    }
}
