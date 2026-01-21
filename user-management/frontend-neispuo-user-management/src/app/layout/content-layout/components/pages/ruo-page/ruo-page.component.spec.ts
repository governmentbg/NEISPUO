import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RuoPageComponent } from './ruo-page.component';

describe('RuoPageComponent', () => {
    let component: RuoPageComponent;
    let fixture: ComponentFixture<RuoPageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [RuoPageComponent],
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(RuoPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
