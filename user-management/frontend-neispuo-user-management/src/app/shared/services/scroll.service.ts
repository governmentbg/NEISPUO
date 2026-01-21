import { Injectable } from '@angular/core';
import * as zenscroll from 'zenscroll';

@Injectable({
    providedIn: 'root',
})
export class ScrollService {
    scrollToFirstError() {
        setTimeout(() => {
            const isIE = (window.document as any).documentMode;
            if (isIE) {
                zenscroll
                    .createScroller(document.body, 500, 0)
                    .center(document.querySelector('p-message[severity=error]') as HTMLElement);
            } else {
                zenscroll.center(document.querySelector('p-message[severity=error]') as HTMLElement);
            }
        }, 0);
    }

    scrollTo(querySelector: string) {
        setTimeout(() => {
            const isIE = (window.document as any).documentMode;
            if (isIE) {
                zenscroll
                    .createScroller(document.body, 500, 0)
                    .center(document.querySelector(querySelector) as HTMLElement);
            } else {
                zenscroll.center(document.querySelector(querySelector) as HTMLElement);
            }
        }, 0);
    }
}
