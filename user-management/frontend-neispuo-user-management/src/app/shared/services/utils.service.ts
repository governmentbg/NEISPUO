import { Injectable } from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';

@Injectable({
    providedIn: 'root',
})
export class UtilsService {
    markAllAsDirty(abstractControl: AbstractControl) {
        if (abstractControl instanceof AbstractControl) {
            abstractControl.markAsDirty();

            const childControls: AbstractControl[] = Object.values((abstractControl as any).controls || {});
            for (const control of childControls) {
                this.markAllAsDirty(control);
            }
        }
    }

    reloadSameRoute(router: Router) {
        // bellow navigateByUrl('/') hack works around https://github.com/angular/angular/issues/21115
        const thisUrl = router.url;
        router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
            router.navigateByUrl(thisUrl);
        });
    }
}
