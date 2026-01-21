/* eslint-disable func-names */
import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class PreventButtonClickService {
    private readonly lockTime = 1000 * 60 * 5; /* 5 minutes */

    private readonly LOCAL_STORAGE_PREVENT_BUTTON_CLICK = 'preventLockButton';

    lockedElements: { identifier: string; timestamp: number }[] = [];

    constructor() {
        this.lockedElements = JSON.parse(localStorage.getItem(this.LOCAL_STORAGE_PREVENT_BUTTON_CLICK) || '[]');
    }

    lockButton(identifier: string) {
        if (this.checkButton(identifier)) return;
        this.lockedElements.push({ identifier, timestamp: new Date().getTime() });
        this.updateLocalStorageLockItems();
    }

    checkButton(identifier: string) {
        this.refreshLocalStorage();
        const idetifierExists = (obj: any) => obj.identifier === identifier;
        return this.lockedElements.some(idetifierExists);
    }

    getLocalStorageLockItems() {
        this.lockedElements = JSON.parse(localStorage.getItem(this.LOCAL_STORAGE_PREVENT_BUTTON_CLICK) || '[]');
    }

    updateLocalStorageLockItems() {
        localStorage.setItem(this.LOCAL_STORAGE_PREVENT_BUTTON_CLICK, JSON.stringify(this.lockedElements));
    }

    refreshLocalStorage() {
        this.getLocalStorageLockItems();
        for (const item of this.lockedElements) {
            if (item.timestamp + this.lockTime <= new Date().getTime())
                this.lockedElements = this.lockedElements.filter((e) => e.identifier !== item.identifier);
        }
        this.updateLocalStorageLockItems();
    }
}
