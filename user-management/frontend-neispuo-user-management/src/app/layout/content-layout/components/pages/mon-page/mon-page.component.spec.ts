import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MonPageComponent } from './mon-page.component';

describe('MonPageComponent', () => {
    let component: MonPageComponent;
    let fixture: ComponentFixture<MonPageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [MonPageComponent],
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(MonPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
