import { Component } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { cacheLine } from 'src/interfaces/cacheRow';

import { transition } from 'src/interfaces/transitionRequest';
import { stateDict,validStates,readRequest,respRequest } from 'src/interfaces/stateDictionary';
import { timer } from 'rxjs';


import{exampleTrans} from 'src/app/examples/firstExample'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  cache1Content:cacheLine[]=[];
  cache2Content:cacheLine[]=[];
  cache3Content:cacheLine[]=[];
  InstructionContent:String[]=[];

  cache1Checkbox:boolean=false;
  cache2Checkbox:boolean=false;
  cache3Checkbox:boolean=false;

  cache1Colors:string[]=[];
  cache2Colors:string[]=[];
  cache3Colors:string[]=[];
  defaultColors:string[]=["#ffffff","#ffffff","#ffffff","#ffffff"];

  currentReadReq={cache:0,row:0}
  markedColor:string="#00ff08";
  unmarkedColor:string="#ffffff";
  //sleep = (ms:number) => new Promise(r => setTimeout(r, ms));

  time:number=1000;

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
    this.cache2Content=[
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

    this.cache3Content=[
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
    this.InstructionContent=["Inc r3 r4","add r5 r5","read r3 45","read r2 r5"]
    this.cache1Colors=["#ffffff", "#ffffff", "#ffffff","#ffffff" ];
    this.cache2Colors=["#ffffff", "#ffffff", "#ffffff","#ffffff" ];
    this.cache3Colors=["#ffffff", "#ffffff", "#ffffff","#ffffff" ];
  
  
  }

  handleTransition(transition:transition):void{
    this.highlight(transition.Cpu_num,transition.Pos_cache);

  }

  buttonClick(){
    console.log(this.cache1Checkbox);
    console.log(this.cache2Checkbox);
    console.log(this.cache3Checkbox);

    //aqui tendria que ir la llamada para el proceso
    this.handleTransactionBundle(exampleTrans);
    
  }

  
  
  handleTransaction(tran:transition){
    let state=stateDict[tran.Op]
    
    if(validStates.some(x=> x===state)){ //true change of state MOESI
      this.changeData(tran.Cpu_num,tran.Pos_cache,tran.Op,tran.Address,tran.Value,state)
      this.highlight(tran.Cpu_num,tran.Pos_cache);
    }
    else if(state===readRequest){
      this.currentReadReq={cache:tran.Cpu_num,row:tran.Pos_cache};
      this.highlight(tran.Cpu_num,tran.Pos_cache);

    }
    else if(state===respRequest){
      this.highlight(tran.Cpu_num,tran.Pos_cache);
      timer(this.time).subscribe(()=>{

        this.turnOff(tran.Cpu_num,tran.Pos_cache);
        this.turnOff(this.currentReadReq.cache,this.currentReadReq.row);
      })
      
    }
  
  }

  handleTransactionBundle(transitions:transition[]):void{

    if(transitions.length<=0){
      timer(this.time).subscribe(()=>{
        this.setDefaultColor();
      })
      return
    }
    this.handleTransaction(transitions[0]);
    timer(this.time).subscribe(()=>{
      this.handleTransactionBundle(transitions.slice(1))
    })
    

   
    
  }

  highlight(cache:number,row:number):void{
    this.changeColor(cache,row,this.markedColor);
    
  }

  turnOff(cache:number,row:number):void{
    this.changeColor(cache,row,this.unmarkedColor);
  }
  changeColor(cache:number,row:number,color:string){
    switch(cache){
      case 0:
        this.cache1Colors[row]=color
        this.cache1Colors=[...this.cache1Colors];
        break;
      case 1:
        this.cache2Colors[row]=color
        this.cache2Colors=[...this.cache2Colors];
        break
      case 2:
        this.cache3Colors[row]=color; 
        this.cache3Colors=[...this.cache3Colors];
        break
    }
  }

  setDefaultColor(){
    this.cache1Colors=[...this.defaultColors];
    this.cache2Colors=[...this.defaultColors];
    this.cache3Colors=[...this.defaultColors];
  }

  changeData(cache:number,row:number,op:string,address:number,value:number,state:string){
    
      
      
      switch(cache){
        case 0:
          this.cache1Content[row].state=state;
          this.cache1Content[row].address=address;
          this.cache1Content[row].value=value;
          this.cache1Content=[...this.cache1Content];
          break;
        case 1:
          this.cache2Content[row].state=state;
          this.cache2Content[row].address=address;
          this.cache2Content[row].value=value;
          this.cache2Content=[...this.cache2Content];
          break
        case 2:
          this.cache3Content[row].state=state;
          this.cache3Content[row].address=address;
          this.cache3Content[row].value=value;
          this.cache3Content=[...this.cache3Content];
          break
      }
    
    

  }







  
}
