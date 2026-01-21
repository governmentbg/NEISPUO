import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MiEditExistingComponent } from './mi-edit-existing.component';

describe('MiEditExistingComponent', () => {
  let component: MiEditExistingComponent;
  let fixture: ComponentFixture<MiEditExistingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MiEditExistingComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MiEditExistingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
