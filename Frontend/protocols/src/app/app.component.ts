import { Component } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { cacheLine } from 'src/interfaces/cacheRow';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  //cache1:MatTableDataSource<cacheLine> =new MatTableDataSource<cacheLine>();
  cache1Content:cacheLine[]=[];
  //displayedColumns:string[]=["state","address","value"]
  title = 'protocols';

  constructor(){}
  
  ngOnInit():void{
    this.cache1Content=[
      {state:"E",
      address:2,
      value:14  
    },
    {state:"I",
      address:14,
      value:9  
    },
    {state:"M",
      address:11,
      value:99  
    },
    {state:"E",
      address:3,
      value:5  
    }
    ];
    //this.cache1.data=this.cache1Content;
  }

  
}
