import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BudgetInstitutionPageComponent } from './budget-institution-page.component';

describe('BudgetInstitutionPageComponent', () => {
    let component: BudgetInstitutionPageComponent;
    let fixture: ComponentFixture<BudgetInstitutionPageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [BudgetInstitutionPageComponent],
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(BudgetInstitutionPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
