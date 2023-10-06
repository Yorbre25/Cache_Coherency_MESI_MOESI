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