import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserTourGuideComponent } from './user-tour-guide.component';

describe('UserTourGuideComponent', () => {
  let component: UserTourGuideComponent;
  let fixture: ComponentFixture<UserTourGuideComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UserTourGuideComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserTourGuideComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
