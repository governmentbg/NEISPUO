import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewModuleInfoComponent } from './new-module-info.component';

describe('NewModuleInfoComponent', () => {
  let component: NewModuleInfoComponent;
  let fixture: ComponentFixture<NewModuleInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NewModuleInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NewModuleInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
