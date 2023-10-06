import { Component,Input, SimpleChanges } from '@angular/core';
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
  ngOnChanges(change:SimpleChanges){
    console.log(change)
    if(change==undefined)return
    
    try{
      this.cache.data=change['cacheContent'].currentValue;
    this.cache.data=[...this.cache.data];
    }
    catch(error){}
    
    try{
      this.rowColors=change['rowColors'].currentValue;
      this.rowColors=[...this.rowColors];
    }
    catch(error){}
    

  }
}
