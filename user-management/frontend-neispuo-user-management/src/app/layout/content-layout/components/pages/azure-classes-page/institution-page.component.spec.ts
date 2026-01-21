import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstitutionPageComponent } from './azure-classes-page.component';

describe('InstitutionPageComponent', () => {
    let component: InstitutionPageComponent;
    let fixture: ComponentFixture<InstitutionPageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [InstitutionPageComponent],
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(InstitutionPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
