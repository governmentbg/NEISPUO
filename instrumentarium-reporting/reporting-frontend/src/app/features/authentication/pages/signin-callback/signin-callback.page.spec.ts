import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SigninCallbackPage } from './signin-callback.page';

describe('SigninCallbackPage', () => {
  let component: SigninCallbackPage;
  let fixture: ComponentFixture<SigninCallbackPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SigninCallbackPage ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SigninCallbackPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
