import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherAzureUsersPageComponent } from './other-azure-users-page.component';

describe('OtherAzureUsersPageComponent', () => {
    let component: OtherAzureUsersPageComponent;
    let fixture: ComponentFixture<OtherAzureUsersPageComponent>;

    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [OtherAzureUsersPageComponent],
        }).compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(OtherAzureUsersPageComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
