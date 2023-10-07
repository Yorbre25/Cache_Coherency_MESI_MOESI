import { Component } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { cacheLine } from 'src/interfaces/cacheRow';

import { stepExecutionReport, transition } from 'src/interfaces/transitionRequest';
import { stateDict,validStates,readRequest,respRequest } from 'src/interfaces/stateDictionary';
import { timer } from 'rxjs';

import { Element } from './memory/memory.component';




import { NetworkService } from './services/network.service';
import { STEP_REQUEST } from 'src/interfaces/request';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  cache1Content:cacheLine[]=[];
  cache2Content:cacheLine[]=[];
  cache3Content:cacheLine[]=[];
  InstructionContent1:String[]=[];
  InstructionContent2:String[]=[];
  InstructionContent3:String[]=[];
  
  
  memoryContent:number[]=[];
  memoryDistributed:Element[]=[];

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

  

  time:number=1000;
  cpu_request = -1

  constructor(private network:NetworkService){}
  
  ngOnInit():void{
    this.cache1Content=[
      {state:"I",
      address:0,
      value:0  
    },
    {state:"I",
      address:0,
      value:0  
    },
    {state:"I",
      address:0,
      value:0  
    },
    {state:"I",
      address:0,
      value:0  
    }
    ];
    this.cache2Content=[
      {state:"I",
      address:0,
      value:0  
    },
    {state:"I",
      address:0,
      value:0  
    },
    {state:"I",
      address:0,
      value:0  
    },
    {state:"I",
      address:0,
      value:0  
    }
    ];

    this.cache3Content=[
      {state:"I",
      address:0,
      value:0  
    },
    {state:"I",
      address:0,
      value:0  
    },
    {state:"I",
      address:0,
      value:0  
    },
    {state:"I",
      address:0,
      value:0  
    }
    ];
    this.InstructionContent1=["NOP","NOP","NOP","NOP"]
    this.InstructionContent2=["NOP","NOP","NOP","NOP"]
    this.InstructionContent3=["NOP","NOP","NOP","NOP"]
    this.cache1Colors=["#ffffff", "#ffffff", "#ffffff","#ffffff" ];
    this.cache2Colors=["#ffffff", "#ffffff", "#ffffff","#ffffff" ];
    this.cache3Colors=["#ffffff", "#ffffff", "#ffffff","#ffffff" ];
    this.memoryContent=[
      0,0,0,0,
      0,0,0,0,
      0,0,0,0,
      0,0,0,0
    ]
    this.memoryDistributed=[
      { col1: '0 : 0', col2: '0 : 0', col3: '0 : 0', col4: '0 : 0' },
      { col1: '0 : 0', col2: '0 : 0', col3: '0 : 0', col4: '0 : 0' },
      { col1: '0 : 0', col2: '0 : 0', col3: '0 : 0', col4: '0 : 0' },
      { col1: '0 : 0', col2: '0 : 0', col3: '0 : 0', col4: '0 : 0' }
    ];
    this.manageList();
  
  }

  handleTransition(transition:transition):void{
    this.highlight(transition.Cpu_num,transition.Pos_cache);

  }

  buttonClick(){

    let step:STEP_REQUEST={
      CPU_1:Number(this.cache1Checkbox),
      CPU_2:Number(this.cache2Checkbox),
      CPU_3:Number(this.cache3Checkbox),
      
      
    }
    this.network.get_request("/MESI/step",step).subscribe(
      (response:stepExecutionReport)=>{
        console.log(response)
        console.log(response.Transition_request)
        console.log(response.initial_CPU_list[0].instrucctions)
        this.InstructionContent1=response.initial_CPU_list[0].instrucctions;
        this.InstructionContent1=[...this.InstructionContent1]
        console.log(this.InstructionContent1)
        this.InstructionContent2=response.initial_CPU_list[1].instrucctions;
        this.InstructionContent2=[...this.InstructionContent2]
        this.InstructionContent3=response.initial_CPU_list[2].instrucctions;
        this.InstructionContent3=[...this.InstructionContent3]

        this.handleTransactionBundle(response.Transition_request);
      }
    )
    
    
  }

  
  
  handleTransaction(tran:transition){
    let state=stateDict[tran.Op]
    console.log(tran)
    this.handleTransactionBuses(tran)
    
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
    console.log("Transition Lenght:"+ transitions.length)
    if(transitions.length<=0){
      
      timer(this.time).subscribe(()=>{
        this.setDefaultColor();
        this.buttonClickEject()
      })
      return
    }
    this.handleTransaction(transitions[0]);
    timer(this.time).subscribe(()=>{
      this.handleTransactionBundle(transitions.slice(1))
    })
  }

  handleTransactionBuses(tran:transition){
    console.log("----------- Transaccion ------------------")
    console.log(this.currentReadReq)
    console.log(tran)
    console.log("----------- Transaccion ------------------")
    if(tran.Op == "SHARED"){
      this.sharedTransaction(tran)
    } 
    //this.currentReadReq
    else if(tran.Op == "WRITE_REQ" || tran.Op == "READ_REQ" || tran.Op == "RESP" ){
      this.addressTransaction(tran)
    }else{
      this.isArrowColored2 = false;
      this.isArrowColored3 = false;
      this.isArrowColored4 = false;
      this.isArrowColored1 = false;
      this.principalDataBus = 'black';
    }
  }
  principalDataBus= '';
  principalAddressBus= '';
  principalSharedBus= '';

  sharedTransaction(tran:transition){
    this.principalSharedBus = 'red';
    if(tran.Cpu_num == 0){
      this.isArrowColored9 = true;
      this.isArrowColored10 = false;
      this.isArrowColored11 = false;
    } else if(tran.Cpu_num == 1){
      this.isArrowColored9 = false;
      this.isArrowColored10 = true;
      this.isArrowColored11 = false;
    }else if(tran.Cpu_num == 2){
      this.isArrowColored9 = false;
      this.isArrowColored10 = false;
      this.isArrowColored11 = true;
    }
  }
  addressTransaction(tran:transition){
    this.principalAddressBus = 'green';
    if(tran.Cpu_num == 0){
      this.isArrowColored5 = false;
      this.isArrowColored6 = true;
      this.isArrowColored7 = false;
      this.isArrowColored8 = false;
      this.cpu_request = 0
      this.isArrowColored2 = false;
      this.isArrowColored3 = false;
      this.isArrowColored4 = false;
      this.isArrowColored1 = false;
      this.principalDataBus = 'black';
    } else if(tran.Cpu_num == 1){
      this.isArrowColored5 = false;
      this.isArrowColored6 = false;
      this.isArrowColored7 = true;
      this.isArrowColored8 = false;
      this.cpu_request = 1
      this.isArrowColored2 = false;
      this.isArrowColored3 = false;
      this.isArrowColored4 = false;
      this.isArrowColored1 = false;
      this.principalDataBus = 'black';
    }else if(tran.Cpu_num == 2){
      this.isArrowColored5 = false;
      this.isArrowColored6 = false;
      this.isArrowColored7 = false;
      this.isArrowColored8 = true;
      this.cpu_request = 2
      this.isArrowColored2 = false;
      this.isArrowColored3 = false;
      this.isArrowColored4 = false;
      this.isArrowColored1 = false;
      this.principalDataBus = 'black';
    }else if(tran.Cpu_num == 3){
      this.isArrowColored5 = true;
      this.dataTransaction(tran)
    }
  }

  address = false
  dataTransaction(tran:transition){
    
    this.principalDataBus = 'blue';
    if(this.cpu_request == 0){
      this.isArrowColored2 = true;
      this.isArrowColored3 = false;
      this.isArrowColored4 = false;
    } else if(this.cpu_request == 1){
      this.isArrowColored2 = false;
      this.isArrowColored3 = true;
      this.isArrowColored4 = false;
    }else if(this.cpu_request == 2){
      this.isArrowColored2 = false;
      this.isArrowColored3 = false;
      this.isArrowColored4 = true;
    }if(tran.Cpu_num == 3){
      this.isArrowColored1 = true;
    }
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

  // Code for 

  isArrowAnimating: boolean = false;
  animateArrow(): void {
    this.isArrowAnimating = true;

    // Opcional: si quieres que la animación se pueda ejecutar de nuevo después de completarse,
    // puedes resetear isArrowAnimating después de la duración de la animación.
    setTimeout(() => {
      this.isArrowAnimating = false;
    }, 2000); // 2000ms (2s) es la duración de la animación. Asegúrate de ajustar este valor si cambias la duración en el CSS.
  }
  
  isArrowColored1 = false;
  isArrowColored2 = false;
  isArrowColored3 = false;
  isArrowColored4 = false;
  isArrowColored5 = false;
  isArrowColored6 = false;
  isArrowColored7 = false;
  isArrowColored8 = false;
  isArrowColored9 = false;
  isArrowColored10 = false;
  isArrowColored11 = false;
  isArrowColored12 = false;
  isArrowColored13 = false;
  isArrowColored14 = false;
  isArrowColored15 = false;
  isArrowColored16 = false;
  isArrowColored17 = false;
  
  buttonClickEject(){
    this.isArrowColored1 = false;
    this.isArrowColored2 = false;
    this.isArrowColored3 = false;
    this.isArrowColored4 = false;
    this.isArrowColored5 = false;
    this.isArrowColored6 = false;
    this.isArrowColored7 = false;
    this.isArrowColored8 = false;
    this.isArrowColored9 = false;
    this.isArrowColored10 = false;
    this.isArrowColored11 = false;
    this.isArrowColored12 = false;
    this.isArrowColored13 = false;
    this.isArrowColored14 = false;
    this.isArrowColored15 = false;
    this.isArrowColored16 = false;
    this.isArrowColored17 = false;
    this.principalSharedBus = 'black'
    this.principalAddressBus = 'black'
    this.principalDataBus = 'black'
  }


  manageList(){
    console.log("Llegue a manage list")
    try{
      if( this.memoryContent.length==0)return
      
      var i:number=0;
      while(i<16){
        console.log(i);
        var element:Element={
          col1:i.toString()+" : "+this.memoryContent[i].toString(),
          col2:(i+1).toString()+" : "+this.memoryContent[i+1].toString(),
          col3:(i+2).toString()+" : "+this.memoryContent[i+2].toString(),
          col4:(i+3).toString()+" : "+this.memoryContent[i+3].toString()
        }
        this.memoryDistributed[Math.floor(i/4)]=element;
        i+=4

        this.memoryDistributed=[...this.memoryDistributed]
        console.log(this.memoryDistributed)
    }
    }
    catch(error){console.log(error)}

  }







  
}
