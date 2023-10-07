import { Component } from '@angular/core';

@Component({
  selector: 'app-secondary-bus',
  templateUrl: './secondary-bus.component.html',
  styleUrls: ['./secondary-bus.component.css']
})
export class SecondaryBusComponent {

  isArrowAnimating: boolean = false;
  animateArrow(): void {
    this.isArrowAnimating = true;

    // Opcional: si quieres que la animación se pueda ejecutar de nuevo después de completarse,
    // puedes resetear isArrowAnimating después de la duración de la animación.
    setTimeout(() => {
      this.isArrowAnimating = false;
    }, 2000); // 2000ms (2s) es la duración de la animación. Asegúrate de ajustar este valor si cambias la duración en el CSS.
  }

  isArrowColored = false;
  change_arrowColor(){
    this.isArrowColored = !this.isArrowColored;
  }
}
