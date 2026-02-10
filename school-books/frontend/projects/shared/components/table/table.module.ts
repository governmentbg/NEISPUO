import { CdkTableModule } from '@angular/cdk/table';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import {
  MatPaginatorDefaultOptions,
  MatPaginatorIntl,
  MatPaginatorModule,
  MAT_PAGINATOR_DEFAULT_OPTIONS
} from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule } from '@angular/material/sort';
import { DEFAULT_PAGE_SIZE } from './table-datasource';
import { TableComponent } from './table.component';

@NgModule({
  declarations: [TableComponent],
  imports: [CdkTableModule, CommonModule, MatPaginatorModule, MatProgressSpinnerModule, MatSortModule],
  exports: [TableComponent],
  providers: [
    {
      provide: MatPaginatorIntl,
      useFactory: () => {
        const matPaginatorIntl = new MatPaginatorIntl();
        matPaginatorIntl.itemsPerPageLabel = '';
        matPaginatorIntl.getRangeLabel = (page, pageSize, length) => {
          if (length === 0 || pageSize === 0) {
            return `0 от ${length}`;
          }

          length = Math.max(length, 0);

          const startIndex = page * pageSize;

          // If the start index exceeds the list length, do not try and fix the end index to the end.
          const endIndex = startIndex < length ? Math.min(startIndex + pageSize, length) : startIndex + pageSize;

          return `${startIndex + 1} – ${endIndex} от ${length}`;
        };
        return matPaginatorIntl;
      }
    },
    {
      provide: MAT_PAGINATOR_DEFAULT_OPTIONS,
      useFactory: () => {
        const options: MatPaginatorDefaultOptions = {
          pageSize: DEFAULT_PAGE_SIZE,
          pageSizeOptions: [DEFAULT_PAGE_SIZE, 100, 500]
        };

        return options;
      }
    }
  ]
})
export class TableModule {}
