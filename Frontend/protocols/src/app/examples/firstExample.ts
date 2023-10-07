import { transition } from "src/interfaces/transitionRequest"
export let exampleTrans:transition[]= [
    {
        "Op": "READ_REQ",
        "Pos_cache": 2,
        "Cpu_num": 0,
        "Address": 7,
        "Value": -1
      },
      {
        "Op": "RESP",
        "Pos_cache": -1,
        "Cpu_num": 3,
        "Address": 7,
        "Value": 0
      },
      {
        "Op": "EX",
        "Pos_cache": 2,
        "Cpu_num": 0,
        "Address": 7,
        "Value": 0
      },
      {
        "Op": "READ_REQ",
        "Pos_cache": 0,
        "Cpu_num": 1,
        "Address": 12,
        "Value": -1
      },
      {
        "Op": "RESP",
        "Pos_cache": -1,
        "Cpu_num": 3,
        "Address": 12,
        "Value": 0
      },
      {
        "Op": "EX",
        "Pos_cache": 0,
        "Cpu_num": 1,
        "Address": 12,
        "Value": 0
      },
      {
        "Op": "MOD",
        "Pos_cache": 0,
        "Cpu_num": 1,
        "Address": 12,
        "Value": 0
      }
]