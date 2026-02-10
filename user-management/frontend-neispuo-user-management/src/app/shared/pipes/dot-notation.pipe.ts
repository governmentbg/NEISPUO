import { Pipe, PipeTransform } from '@angular/core';
// this pipe is used to return  a nested element from an array with a (dot notation string)
// e.g you have an array student[] and have a string properties.roles.role and whish to get the element from the array
@Pipe({ name: 'dotNotationPipe' })
export class DotNotationPipe implements PipeTransform {
    index(obj: any, i: any) {
        return obj[i];
    }

    transform(array: any[], dotNotationString: string): any {
        const result = dotNotationString.split('.').reduce((o: any, i: any) => o[i], array);
        return result;
    }
}
