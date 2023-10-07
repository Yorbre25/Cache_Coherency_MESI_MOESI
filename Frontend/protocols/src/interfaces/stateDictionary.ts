


export let stateDict: { [key: string]: string } = {};
stateDict["WRITE_REQ"]="WR";
stateDict["READ_REQ"]="RR";
stateDict["RESP"]="RSP";
stateDict["MOD"]="M"; 
stateDict["EX"]="E";
stateDict["OWN"]="O";
stateDict["SHARED"]="S";
stateDict["INV"]="INV";



export let validStates=["M","O","E","S","I"];
export let readRequest="RR"
export let respRequest="RSP";


