import { MatPaginatorIntl } from "@angular/material/paginator";

const bgRangeLabel = (page: number, pageSize: number, length: number) => {
  length = Math.max(length, 0);
  pageSize = Math.max(pageSize, 0);

  const totalPages = pageSize !== 0 && length !== 0 ? Math.ceil(length / pageSize) : 1;

  return `стр. ${page + 1} от ${totalPages}`;
};

export function getBgPaginatorIntl() {
  const paginatorIntl = new MatPaginatorIntl();

  paginatorIntl.itemsPerPageLabel = "Елементи на страница:";
  paginatorIntl.nextPageLabel = "Следваща страница";
  paginatorIntl.previousPageLabel = "Предишна страница";
  paginatorIntl.getRangeLabel = bgRangeLabel;

  return paginatorIntl;
}
