import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SystemUserMessagesComponent } from './system-user-messages.component';

describe('SystemUserMessagesComponent', () => {
  let component: SystemUserMessagesComponent;
  let fixture: ComponentFixture<SystemUserMessagesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SystemUserMessagesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SystemUserMessagesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
