import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserGuideAdditionModalComponent } from './user-guide-addition-modal.component';

describe('UserGuideAdditionModalComponent', () => {
  let component: UserGuideAdditionModalComponent;
  let fixture: ComponentFixture<UserGuideAdditionModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserGuideAdditionModalComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserGuideAdditionModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
