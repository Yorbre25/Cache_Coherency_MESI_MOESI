import { Component,Input, SimpleChange } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { cacheLine } from 'src/interfaces/cacheRow';

@Component({
  selector: 'app-cache',
  templateUrl: './cache.component.html',
  styleUrls: ['./cache.component.css']
})
export class CacheComponent {
  displayedColumns:string[]=["state","address","value"]
  @Input()rowColors: string[] = [];
  cache:MatTableDataSource<cacheLine> =new MatTableDataSource<cacheLine>();
  @Input() cacheContent:cacheLine[]=[]
  constructor(){}
  
  ngOnInit():void{
    
    this.cache.data=this.cacheContent;
  }
  ngOnChanges(change:SimpleChange){
    this.cache.data=change.currentValue;

  }
}
