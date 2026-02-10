import { Injectable } from '@angular/core';
import * as zenscroll from 'zenscroll';

@Injectable({
  providedIn: 'root',
})
export class ScrollService {
  constructor() { }

  scrollToFirstError(errorQuerySelector = 'small.invalid-feedback') {
    this.scrollTo(errorQuerySelector);
  }

  scrollTo(querySelector: string, scrollableElement: HTMLElement = document.documentElement): void {
    setTimeout(() => {
      zenscroll.createScroller(scrollableElement, 500, 0).center(document.querySelector(querySelector).parentElement);
    }, 0);
  }
}
