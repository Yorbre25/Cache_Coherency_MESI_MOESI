import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SecondaryBusComponent } from './secondary-bus.component';

describe('SecondaryBusComponent', () => {
  let component: SecondaryBusComponent;
  let fixture: ComponentFixture<SecondaryBusComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SecondaryBusComponent]
    });
    fixture = TestBed.createComponent(SecondaryBusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
