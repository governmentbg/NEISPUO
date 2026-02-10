import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProcedureChoicePage } from './procedure-choice.page';

describe('ProcedureChoicePage', () => {
  let component: ProcedureChoicePage;
  let fixture: ComponentFixture<ProcedureChoicePage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProcedureChoicePage],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProcedureChoicePage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
