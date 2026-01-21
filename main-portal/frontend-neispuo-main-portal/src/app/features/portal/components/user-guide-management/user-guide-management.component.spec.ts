import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserGuideManagementComponent } from './user-guide-management.component';

describe('UserGuideManagementComponent', () => {
  let component: UserGuideManagementComponent;
  let fixture: ComponentFixture<UserGuideManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserGuideManagementComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserGuideManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
