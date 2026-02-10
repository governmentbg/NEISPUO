import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MunicipalPublicRegisterPageComponent } from './municipal-public-register-page.component';

describe('MunicipalPublicRegisterPageComponent', () => {
  let component: MunicipalPublicRegisterPageComponent;
  let fixture: ComponentFixture<MunicipalPublicRegisterPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MunicipalPublicRegisterPageComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MunicipalPublicRegisterPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
