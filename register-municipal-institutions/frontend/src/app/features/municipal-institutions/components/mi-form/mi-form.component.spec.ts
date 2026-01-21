import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MiFormComponent } from './mi-form.component';

describe('MiFormComponent', () => {
  let component: MiFormComponent;
  let fixture: ComponentFixture<MiFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MiFormComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MiFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
