import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JoinMiPreviewDeleteComponent } from './join-mi-preview-delete.component';

describe('MiPreviewExistingComponent', () => {
  let component: JoinMiPreviewDeleteComponent;
  let fixture: ComponentFixture<JoinMiPreviewDeleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [JoinMiPreviewDeleteComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(JoinMiPreviewDeleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
