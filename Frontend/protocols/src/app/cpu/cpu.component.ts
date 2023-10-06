import { Component,Input, SimpleChange } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-cpu',
  templateUrl: './cpu.component.html',
  styleUrls: ['./cpu.component.css']
})
export class CpuComponent {
  displayedColumns:string[]=["Instructions"]
  InstructionsDataSource:MatTableDataSource<String> =new MatTableDataSource<String>();
  @Input() instructions:String[]=[]
  constructor(){}
  
  ngOnInit():void{
    
    this.InstructionsDataSource.data=this.instructions;
  }
  ngOnChanges(change:SimpleChange){
    this.InstructionsDataSource.data=change.currentValue;

  }
}