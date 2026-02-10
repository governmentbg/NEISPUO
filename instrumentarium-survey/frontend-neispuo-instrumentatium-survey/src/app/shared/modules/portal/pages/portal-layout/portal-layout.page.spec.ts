import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PortalLayoutPage } from './portal-layout.page';

describe('PortalLayoutComponent', () => {
  let component: PortalLayoutPage;
  let fixture: ComponentFixture<PortalLayoutPage>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [PortalLayoutPage]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PortalLayoutPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
