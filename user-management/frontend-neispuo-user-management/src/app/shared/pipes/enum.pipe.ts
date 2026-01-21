import { Pipe, PipeTransform } from '@angular/core';

// this pipe is used to return the label from a chosen select box
@Pipe({ name: 'enum' })
export class EnumPipe implements PipeTransform {
    index(obj: any, i: any) {
        return obj[i];
    }

    transform(value: any, selectionOptions: any): any {
        const selectionOption: any = selectionOptions.find((option: any) => {
            return option.value === value;
        });
        return selectionOption.label;
    }
}
