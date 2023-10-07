import { Component,Input,SimpleChanges} from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { timer } from 'rxjs';
@Component({
  selector: 'app-memory',
  templateUrl: './memory.component.html',
  styleUrls: ['./memory.component.css']
})
export class MemoryComponent {
  displayedColumns: string[] = ['col1', 'col2', 'col3', 'col4'];
  @Input()  Elements:Element[]=[];
  dataSource = new MatTableDataSource<Element>([
    { col1: 'A1', col2: 'B1', col3: 'C1', col4: 'D1' },
    { col1: 'A2', col2: 'B2', col3: 'C2', col4: 'D2' },
    { col1: 'A3', col2: 'B3', col3: 'C3', col4: 'D3' },
    { col1: 'A4', col2: 'B4', col3: 'C4', col4: 'D4' },
  ]);

  ngOnChanges(change:SimpleChanges){
    if(change===undefined) return
    try{
      this.Elements=change['Elements'].currentValue;
      this.Elements=[...this.Elements];
      this.dataSource.data=this.Elements;
      this.dataSource.data=[...this.dataSource.data];
      
    }
    catch(error){

    }

      
      

    
  }
  

  
}

export interface Element {
  col1: string;
  col2: string;
  col3: string;
  col4: string;
}