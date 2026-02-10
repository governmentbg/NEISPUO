import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolBooksAccessPageComponent } from './school-books-access-page.component';

describe('SchoolBooksAccessPageComponent', () => {
    let component: SchoolBooksAccessPageComponent;
    let fixture: ComponentFixture<SchoolBooksAccessPageComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [SchoolBooksAccessPageComponent],
        }).compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(SchoolBooksAccessPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
