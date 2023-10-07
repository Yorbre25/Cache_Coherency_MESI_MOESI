import { Component,Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-memory',
  templateUrl: './memory.component.html',
  styleUrls: ['./memory.component.css']
})
export class MemoryComponent {
  displayedColumns: string[] = ['col1', 'col2', 'col3', 'col4'];
  @Input() memData:number[][]=[]
  dataSource = new MatTableDataSource<Element>([
    { col1: 'A1', col2: 'B1', col3: 'C1', col4: 'D1' },
    { col1: 'A2', col2: 'B2', col3: 'C2', col4: 'D2' },
    { col1: 'A3', col2: 'B3', col3: 'C3', col4: 'D3' },
    { col1: 'A4', col2: 'B4', col3: 'C4', col4: 'D4' },
  ]);
}

export interface Element {
  col1: string;
  col2: string;
  col3: string;
  col4: string;
}