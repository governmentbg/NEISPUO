import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddSchoolBookAccessComponent } from './add-school-book-access.component';

describe('AddSchoolBookAccessComponent', () => {
    let component: AddSchoolBookAccessComponent;
    let fixture: ComponentFixture<AddSchoolBookAccessComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [AddSchoolBookAccessComponent],
        }).compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(AddSchoolBookAccessComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
