import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DivideMiPreviewDeleteComponent } from './divide-mi-preview-delete.component';

describe('MiPreviewExistingComponent', () => {
  let component: DivideMiPreviewDeleteComponent;
  let fixture: ComponentFixture<DivideMiPreviewDeleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DivideMiPreviewDeleteComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DivideMiPreviewDeleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
