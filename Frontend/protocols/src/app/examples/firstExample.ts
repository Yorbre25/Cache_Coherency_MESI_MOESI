import { transition } from "src/interfaces/transitionRequest"
export let exampleTrans:transition[]= [
    { Op: "READ_REQ", Pos_cache: 0, Cpu_num: 1, Address: 0, Value: -1 },
    { Op: "RESP", Pos_cache: 0, Cpu_num: 0, Address: 0, Value: 0 },
    { Op: "SHARED", Pos_cache: 0, Cpu_num: 0, Address: 0, Value: 0 },
    { Op: "WRITE_REQ", Pos_cache: -1, Cpu_num: 0, Address: 0, Value: 0 }, 
    { Op: "SHARED", Pos_cache: 0, Cpu_num: 1, Address: 0, Value: 0 }, 
    { Op: "MOD", Pos_cache: 0, Cpu_num: 1, Address: 0, Value: 0 }, 
    { Op: "INV", "Pos_cache": 0, "Cpu_num": 0, "Address": 0, "Value": 0 }
]