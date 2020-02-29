let board = [];
let colors = [];
const squareSize = 70;
let dropSpeed = Math.ceil(squareSize / 20);
const asciiA = 65;

const offsetTop = 20;
const offsetLeft = 20;

let boardPosTop = offsetTop + (2 * squareSize);

const connectN = 4;
let gridHeight;
let gridWidth;
let currentPlayer = true;

let frame = 3;

let busy = false;

let setup = () => {
    CreateColors(2);
    SetGridDimensions();
    DrawControls(offsetTop,offsetLeft);
    GenerateBoard(boardPosTop,offsetLeft);
}

window.addEventListener("load", setup);

let CreateColors = (complexity) => {
    let divisions = 360 / complexity;
    for (let index = 0; index < complexity; index++) {
        colors.push({value: String.fromCharCode(asciiA + index), h: 0 + index * divisions, s: 100, l: 50});
    }
}

let SetGridDimensions = () => {
    gridHeight = Math.ceil(3 / 2 * connectN);
    gridWidth = (2 * connectN) - 1;
    //console.log("Grid Dimensions are W: " + gridWidth + " and H: " + gridHeight + ".")
}

let DrawControls = (top, left) => {
    for (let leftMultiplier = 0; leftMultiplier < gridWidth; leftMultiplier++) {
        let baseVakje = document.createElement("p");
        baseVakje.classList.add("selector");
        baseVakje.id = "Selector" + leftMultiplier;
        baseVakje.style.left= (left + (leftMultiplier * squareSize)) +"px";
        baseVakje.style.top= top + "px";
        baseVakje.style.width = squareSize + "px";
        baseVakje.style.height = squareSize + "px";
        baseVakje.onclick = function() {SelectColumn(leftMultiplier);};
        let baseVakjeContent = document.createTextNode("");
        baseVakje.appendChild(baseVakjeContent);
        document.body.appendChild(baseVakje);
    }
}

let GenerateBoard = (top, left) => {
    for (let topMultiplier = 0; topMultiplier < gridHeight; topMultiplier++) {
        for (let leftMultiplier = 0; leftMultiplier < gridWidth; leftMultiplier++) {
            let partialTop = top + ( topMultiplier * squareSize);
            let partialLeft = left + ( leftMultiplier * squareSize);
            
            let baseVakje = document.createElement("p");
            baseVakje.classList.add("vakje");
            baseVakje.id = partialLeft + "," + partialTop;
            baseVakje.style.left= partialLeft +"px";
            baseVakje.style.top= partialTop + "px";
            baseVakje.style.width = squareSize + "px";
            baseVakje.style.height = squareSize + "px";
            let baseVakjeContent = document.createTextNode("");
            baseVakje.appendChild(baseVakjeContent);
            document.body.appendChild(baseVakje);
        }
    }
}

let SelectColumn = (column) => {
    if (busy) {
        console.log("Still busy...");
    } else {
        //console.log("Column " + column + " was selected.");
        let row = GetTokenCountInColumn(column);
        if (row < gridHeight) {
            busy = true;
            console.log("Player " + (currentPlayer?"red":"yellow") + " dropped a token in column " + column + ".");
            let tokenId = board.length;
            console.log("Assigning id: " + tokenId + " to token.");
            let targetTop = boardPosTop + ((gridHeight - 1) * squareSize) - row * squareSize;
            console.log("This token will not fall further than top: " + targetTop);
            board.push({tokenId: tokenId, column: column, row: row, player: true, targetTop: targetTop});
            SpawnToken(offsetTop + (2 * squareSize), offsetLeft + (column * squareSize), tokenId);
            FallDown(tokenId);
        } else {
            console.log("This column is already full...");
        }
    }
}

let SpawnToken = (y,x,id) => {
    //console.log("Drawing token at top: " + y + ", left: " + x + ".");
    let baseVakje = document.createElement("p");
    baseVakje.classList.add("token");
    baseVakje.id = id;
    if (currentPlayer) {
        baseVakje.style.backgroundColor = "rgb(255, 0, 0)";
    } else {
        baseVakje.style.backgroundColor = "rgb(255, 255, 0)";
    }
    baseVakje.style.left= x +"px";
    baseVakje.style.top= y + "px";
    baseVakje.style.width = squareSize + "px";
    baseVakje.style.height = squareSize + "px";
    let baseVakjeContent = document.createTextNode("");
    baseVakje.appendChild(baseVakjeContent);
    document.body.appendChild(baseVakje);
}

let FallDown = (id) => {
    //console.log("Token with id: " + id + " is falling.");
    let fallingToken = document.getElementById(id);
    let currentTop = parseInt(fallingToken.style.top.substring(0, fallingToken.style.top.length - "px".length));
    if (currentTop + dropSpeed > GetTokenTargetTopById(id)) {
        fallingToken.style.top = (currentTop + (GetTokenTargetTopById(id)-currentTop)) + "px";
    } else {
        fallingToken.style.top = (currentTop + dropSpeed) + "px";
    }
    if (currentTop < GetTokenTargetTopById(id)) {
        setTimeout(function(){ FallDown(id); }, frame);
    } else {
        CheckTokenForLines(board.length - 1);
    }
}

let GetTokenCountInColumn = (column) => {
    let tokensInColumn = 0;
    board.forEach(element => {
        if (element.column === column) {
            tokensInColumn++;
        }
    });
    return tokensInColumn;
}

let GetTokenTargetTopById = (id) => {
    let returnval = 0;
    board.forEach(element => {
        //console.log("ElementId: " + element.tokenId + " and were looking for ID: " + id);
        if (element.tokenId === id) {
            //console.log("FOUND: " + element.targetTop);
            returnval = element.targetTop;
        }
    });
    return returnval;
    //console.log("No token with id: " + id + ".")
}

let SwapPlayer = () => {
    currentPlayer = !currentPlayer;
    busy = false;
    console.log("----------------------------------------------------------");
    console.log("Player " + (currentPlayer?"red":"yellow") + " is now playing.");
}

let CheckTokenForLines = (id) => {
    console.log("Checking token with id: " + id + " for possible lines of length: " + connectN + ".");



    SwapPlayer();
}