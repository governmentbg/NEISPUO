import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstitutionHeadmasterDataComponent } from './institution-headmaster-data.component';

describe('InstitutionHeadmasterDataComponent', () => {
  let component: InstitutionHeadmasterDataComponent;
  let fixture: ComponentFixture<InstitutionHeadmasterDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InstitutionHeadmasterDataComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InstitutionHeadmasterDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
