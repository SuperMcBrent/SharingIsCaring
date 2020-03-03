let board = [];
let colors = [];
const squareSize = 80;
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
let gameIsFinished = false;

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
        baseVakje.onmouseenter = function() {HighlightColumn(leftMultiplier);};
        baseVakje.onmouseleave = function() {UnHighlightColumn(leftMultiplier);};
        let baseVakjeContent = document.createTextNode("");
        baseVakje.appendChild(baseVakjeContent);
        document.body.appendChild(baseVakje);
    }
}

let HighlightColumn = (column) => {
    for (let index = 0; index < gridHeight; index++) {
        let xPos = offsetLeft + (column * squareSize);
        let yPos = offsetTop + (squareSize * 2) + (index * squareSize);
        let vakje = document.getElementById(xPos+ "," + yPos);
        vakje.style.backgroundColor = "rgb(48, 168, 216)";
    }
}

let UnHighlightColumn = (column) => {
    for (let index = 0; index < gridHeight; index++) {
        let xPos = offsetLeft + (column * squareSize);
        let yPos = offsetTop + (squareSize * 2) + (index * squareSize);
        let vakje = document.getElementById(xPos+ "," + yPos);
        vakje.style.backgroundColor = "rgb(48, 48, 216)";
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
    if (gameIsFinished) {
        console.log("Game is finished");
    } else if (busy) {
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
            board.push({tokenId: tokenId, column: column, row: row, player: currentPlayer, targetTop: targetTop});
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
    var token = GetToken(id);
    let targetTop = token.targetTop;
    if (currentTop + dropSpeed > targetTop) {
        fallingToken.style.top = (currentTop + (targetTop-currentTop)) + "px";
    } else {
        fallingToken.style.top = (currentTop + dropSpeed) + "px";
    }
    if (currentTop < targetTop) {
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

let SwapPlayer = () => {
    currentPlayer = !currentPlayer;
    busy = false;
    console.log("----------------------------------------------------------");
    console.log("Player " + (currentPlayer?"red":"yellow") + " is now playing.");
}

let CheckTokenForLines = (id) => {
    console.log("Checking token with id: " + id + " for possible lines of length: " + connectN + ".");

    LineChecker(id);

    if (!gameIsFinished) {
        SwapPlayer();
    } else {
        // trigger winevent.
    }
}

let GetToken = (id) => {
    let token;
    board.forEach(element => {
        if (element.tokenId === id) {
            token = element;
        }
    });
    return token;
}

let GetTokenByPos = (row,column) => {
    let token = null;;
    board.forEach(element => {
        if (element.column === column && element.row === row) {
            token = element;
        }
    });
    return token;
}

let LineChecker = (id) => {
    let vakje = GetToken(id);
    for (let rowOffset = -1; rowOffset < 2; rowOffset++) {
        for (let columnOffset = -1; columnOffset < 2; columnOffset++) {
            if (columnOffset === 0 && rowOffset === 0) {
                //console.log("Zelfde token");
                continue;
            }
            let neighbouringToken = GetTokenByPos(vakje.row + rowOffset, vakje.column + columnOffset);
            if (neighbouringToken === null) {
                //console.log("No neighbouring token at " + (vakje.row + rowOffset) + ", " + (vakje.column + columnOffset) + ".");
                continue;
            }
            if (neighbouringToken.player !== vakje.player) {
                //console.log("Neighbouring token at abspos " + (vakje.row + rowOffset) + ", " + (vakje.column + columnOffset) + " is owned by another player");
                continue;
            }

            console.log("wassup broeder.")
            let currentVectorXpart = rowOffset;
            let currentVectorYpart = columnOffset;
            let tokensInARow = 1;
            //vector
            for (let index = 1; index <= connectN; index++) {
                let tokenToTest = GetTokenByPos(vakje.row + (currentVectorXpart * index), vakje.column + (currentVectorYpart * index));
                if (tokenToTest === null) {
                    console.log("Theres no token here, breaking...")
                    break;
                }
                if (tokenToTest.player !== vakje.player) {
                    console.log("Theres another player here, breaking...")
                    break;
                }
                //console.log("Checking token for lines at " + (vakje.row + (currentVectorXpart * index)) + ", " + (vakje.column + (currentVectorYpart * index)) + ".");
                console.log("IK heb een broeder gevonden.")
                tokensInARow++;
            }
            //tegensgestelde vector TODO
            for (let index = 1; index <= connectN; index++) {
                let tokenToTest = GetTokenByPos(vakje.row - (currentVectorXpart * index), vakje.column - (currentVectorYpart * index));
                if (tokenToTest === null) {
                    console.log("Theres no token here, breaking...")
                    break;
                }
                if (tokenToTest.player !== vakje.player) {
                    console.log("Theres another player here, breaking...")
                    break;
                }
                //console.log("Checking token for lines at " + (vakje.row + (currentVectorXpart * index)) + ", " + (vakje.column + (currentVectorYpart * index)) + ".");
                console.log("IK heb een broeder gevonden.")
                tokensInARow++;
            }
            if (tokensInARow >= connectN) {
                gameIsFinished = true;
                console.log("VICTORY");
            }
        }
    }
}