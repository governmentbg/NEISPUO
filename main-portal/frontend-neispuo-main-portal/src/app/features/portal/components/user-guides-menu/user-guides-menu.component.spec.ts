import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { UserGuidesMenuComponent } from './user-guides-menu.component';

describe('UserGuidesMenuComponent', () => {
  let component: UserGuidesMenuComponent;
  let fixture: ComponentFixture<UserGuidesMenuComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ UserGuidesMenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserGuidesMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
