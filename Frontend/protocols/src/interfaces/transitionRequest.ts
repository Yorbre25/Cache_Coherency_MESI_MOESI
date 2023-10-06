export interface transition{
    op:string;
    cpu_num:number;
    pos_cache: number;
    address:number;
    value:number

}

export interface transition_request{
    transitions:transition[]
}