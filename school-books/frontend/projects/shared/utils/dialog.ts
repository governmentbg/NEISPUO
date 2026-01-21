import { ComponentType } from '@angular/cdk/portal';
import { MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material/dialog';

export interface TypedDialog<D, R> {
  // required only for type inference to work
  d: D;
  r: R;
}

type TypedDialogDataType<T> = T extends TypedDialog<infer D, any> ? D : never;
type TypedDialogResultType<T> = T extends TypedDialog<any, infer R> ? R : never;

export function openTypedDialog<T extends TypedDialog<any, any>>(
  dialog: MatDialog,
  component: ComponentType<T>,
  config?: MatDialogConfig<TypedDialogDataType<T>>
): MatDialogRef<T, TypedDialogResultType<T>> {
  return dialog.open<T, TypedDialogDataType<T>, TypedDialogResultType<T>>(component, {
    panelClass: 'sb-dialog',
    ...config
  });
}
