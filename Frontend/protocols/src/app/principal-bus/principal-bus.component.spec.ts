import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrincipalBusComponent } from './principal-bus.component';

describe('PrincipalBusComponent', () => {
  let component: PrincipalBusComponent;
  let fixture: ComponentFixture<PrincipalBusComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [PrincipalBusComponent]
    });
    fixture = TestBed.createComponent(PrincipalBusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
