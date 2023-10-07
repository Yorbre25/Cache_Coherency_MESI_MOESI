export interface transition{
    Op:string;
    Cpu_num:number;
    Pos_cache: number;
    Address:number;
    Value:number

}

export interface transition_request{
    transitions:transition[]
}

export interface stepExecutionReport{
    Transition_request:transition[];
    initial_RAM:number[];
    initial_CPU_list:CPU_list[];
    final_Ram:number[]
    final_CPU_list:CPU_list[]


}

export interface CPU_list{
    cache:cacheOBJ
    instrucctions:String[]
}

export interface cacheOBJ{
    memory:number[][]
    
}