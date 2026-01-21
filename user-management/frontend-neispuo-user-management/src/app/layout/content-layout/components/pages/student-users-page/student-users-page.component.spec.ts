import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentUsersPageComponent } from './student-users-page.component';

describe('StudentPageComponent', () => {
    let component: StudentUsersPageComponent;
    let fixture: ComponentFixture<StudentUsersPageComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [StudentUsersPageComponent],
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(StudentUsersPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
