import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NeispuoModuleCardComponent } from './neispuo-module-card.component';

describe('NeispuoModuleCardComponent', () => {
  let component: NeispuoModuleCardComponent;
  let fixture: ComponentFixture<NeispuoModuleCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NeispuoModuleCardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NeispuoModuleCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
