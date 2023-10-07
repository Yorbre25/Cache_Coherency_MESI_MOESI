import { Component, ElementRef, ViewChild, OnInit} from '@angular/core';
@Component({
  selector: 'app-principal-bus',
  templateUrl: './principal-bus.component.html',
  styleUrls: ['./principal-bus.component.css']
})
export class PrincipalBusComponent implements OnInit {



  ngOnInit() {
  }


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
