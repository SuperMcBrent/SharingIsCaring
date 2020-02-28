const length = 4;
const complexity = 8;
const rounds = 10;
const asciiA = 65;

let board = [];
let guesses = [];
let colors = [];
let code;
let tempGuess = "";

let currentLine = 0;

const squareSize = 30;

const offsetTop = 20;
const offsetLeft = 20;

let tempGuessTop = 0;
let tempGuessLeft = 0;

let setup = () => {
    CreateColors(complexity);
    console.log(colors);
    code = CreateCode();
    console.log(code);
    //Compare(code,"AAAA");
    // while (currentLine < 30) {
    //     AddGuess(CreateCode());
    // }
    console.log(guesses);
    BuildGuessBuilder();
}

let DisplayTemporaryGuess = (y,x,tempguess) => {
    let left = x;
    tempguess.split('').forEach(element => {
        // element met zelfde coordinaat in ID verwijderen
        if (document.contains(document.getElementById(left+","+y))) {
            //console.log("Element met id:" +left+","+y + " werd verwijderd.")
            document.getElementById(left+","+y).remove();
        }
        DrawSquare(y,left,element);
        left += squareSize;
    });
}

let ClearTemporaryGuess = (y,x) => {
    tempGuess = "";
    let left = x;
    for (let index = 0; index < length; index++) {
        // element met zelfde coordinaat in ID verwijderen
        if (document.contains(document.getElementById(left+","+y))) {
            //console.log("Element met id:" +left+","+y + " werd verwijderd.")
            document.getElementById(left+","+y).remove();
        }
        left += squareSize;
    }
}

let BuildGuessBuilder = () => {
    //werken met line number en eronder plaatsen met 1 kottke ruimte
    // OF selector rechts plaatsen.
    // over selectiemethode moet nog nagedacht worden.
    let left = offsetLeft + (squareSize * length) + squareSize;
    let top = offsetTop;

    tempGuessLeft = left + (2 * squareSize);
    tempGuessTop = top + (2 * squareSize);

    BuildButton(top + (4 * squareSize),left,squareSize * 4, squareSize, "CONFIRM","#6eeb34");
    BuildButton(top + (4 * squareSize),left + (squareSize * 5),squareSize * 3, squareSize, "CLEAR", "#eb3434");

    colors.forEach(element => {
        DrawSelector(top,left,element.value);
        left += squareSize;
    });
}

let GameFunction = (command) => {
    if (command === "CONFIRM") {
        if (tempGuess.length < length) {
            console.log("Guess is niet lang genoeg.");
            return;
        }
        console.log("Keuze bevestigd.");
        AddGuess(tempGuess);
        ClearTemporaryGuess(tempGuessTop,tempGuessLeft);
        return;
    }
    if (command === "CLEAR") {
        ClearTemporaryGuess(tempGuessTop,tempGuessLeft);
        console.log("Cleared.");
        return;
    }
}

let BuildButton = (y,x,width,heigth,content,color) => {
    console.log("Drawing button" + code + " at top: " + y + ", left: " + x + ".");
    let baseVakje = document.createElement("p");
    baseVakje.classList.add("vakje");
    baseVakje.id = x + "," + y;
    baseVakje.style.position = "absolute";
    baseVakje.style.left= x +"px";
    baseVakje.style.top= y + "px";
    baseVakje.style.border = "1px solid black";
    baseVakje.style.cursor = "pointer";
    baseVakje.style.width = width + "px";
    baseVakje.style.height = heigth + "px";
    baseVakje.style.backgroundColor = color;
    baseVakje.onclick = function() {GameFunction(content);};
    //baseVakje.onclick = () => {onclickEvent(x,y);};
    let baseVakjeContent = document.createTextNode(content);
    baseVakje.appendChild(baseVakjeContent);
    document.body.appendChild(baseVakje);
}

let SelectColorForGuess = (code) => {
    if (tempGuess.length < length) {
        tempGuess += code;
        //console.log("Kleurcode: " + code + " werd geselecteerd ("+tempGuess+").")
        DisplayTemporaryGuess(tempGuessTop,tempGuessLeft,tempGuess);
    } else {
        console.log("Code is al te lang.");
    }
}

let DrawSelector = (y,x,code) => {
    //console.log("Drawing selector" + code + " at top: " + y + ", left: " + x + ".");
    let baseVakje = document.createElement("p");
    baseVakje.classList.add("vakje");
    baseVakje.id = x + "," + y;
    baseVakje.style.position = "absolute";
    baseVakje.style.left= x +"px";
    baseVakje.style.top= y + "px";
    baseVakje.style.border = "1px solid black";
    baseVakje.style.cursor = "pointer";
    baseVakje.style.width = squareSize + "px";
    baseVakje.style.height = squareSize + "px";
    colors.forEach(element => {
        if (element.value === code) {
            baseVakje.style.backgroundColor = "hsl(" + element.h + ", " + element.s + "%, " + element.l + "%)";
        }
    });
    baseVakje.onclick = function() {SelectColorForGuess(code);};
    //baseVakje.onclick = () => {onclickEvent(x,y);};
    let baseVakjeContent = document.createTextNode(code);
    baseVakje.appendChild(baseVakjeContent);
    document.body.appendChild(baseVakje);
}

let AddGuess = (guessString) => {
    var result = Compare(code,guessString);
    guesses.push({guess: guessString, good: result[0].good, almost: result[0].almost});
    DrawGuess(guessString,currentLine);
    //voorlopig textueel, maar later opname via gui.
}

let DrawGuess = (guess,line) => {
    let left = offsetLeft;
    let top = offsetTop + currentLine * squareSize;
    guess.split('').forEach(element => {
        DrawSquare(top,left,element);
        left += squareSize;
    });
    currentLine++;
}

let DrawSquare = (y,x,code) => {
    //console.log("Drawing " + code + " at top: " + y + ", left: " + x + ".");
    let baseVakje = document.createElement("p");
    baseVakje.classList.add("vakje");
    baseVakje.id = x + "," + y;
    baseVakje.style.position = "absolute";
    baseVakje.style.left= x +"px";
    baseVakje.style.top= y + "px";
    baseVakje.style.border = "1px solid black";
    baseVakje.style.cursor = "default";
    baseVakje.style.width = squareSize + "px";
    baseVakje.style.height = squareSize + "px";
    colors.forEach(element => {
        if (element.value === code) {
            baseVakje.style.backgroundColor = "hsl(" + element.h + ", " + element.s + "%, " + element.l + "%)";
        }
    });
    //baseVakje.onclick = function() {onclickEvent(x,y);};
    //baseVakje.onclick = () => {onclickEvent(x,y);};
    let baseVakjeContent = document.createTextNode(code);
    baseVakje.appendChild(baseVakjeContent);
    document.body.appendChild(baseVakje);
}

let Compare = (code, guess) => {
    let guessString = guess.split('');
    let codeString = code.split('');
    let good = 0;
    let almost = 0;
    console.log(" Code: " + code + "\nGuess: " + guess);
    if (code.length !== guess.length) {
        console.log("Code and guess are not same length.");
        return;
    }
    // rechttegenoverstaande gelijken tellen en eruithalen om double hits tegen te gaan
    for (let i = 0; i < code.length; i++) {
        if (codeString[i] === guessString[i]) {
            guessString[i] = '_';
            codeString[i] = '_';
            good++;
            continue;
        }
    }
    // verkeerde plaatsjes tellen en eruithalen om double hits tegen te gaan
    for (let i = 0; i < code.length; i++) {
        if (codeString[i] !== '_') {
            //console.log(codeString[i]);
            if (guessString.includes(codeString[i])) {
                var a = guessString.indexOf(codeString[i]);
                guessString[a] = '_';
                codeString[i] = '_';
                almost++;
            }
        }
    }
    console.log("Good: " + good + ", almost: " + almost + ".");
    if (good === code.length) {
        console.log("You win!");
        window.alert("You win!")
        //trigger winEvent.
        //reset game
    }

    var result = [];
    result.push({good: good, almost: almost})
    return result;
}

let CreateCode = () => {
    let tempCode = "";
    for (let index = 0; index < length; index++) {
        tempCode += String.fromCharCode(Math.floor(Math.random()*complexity+asciiA));        
    }
    return tempCode;
}

window.addEventListener("load", setup);

let CreateColors = (complexity) => {
    let divisions = 360 / complexity;
    for (let index = 0; index < complexity; index++) {
        colors.push({value: String.fromCharCode(asciiA + index), h: 0 + index * divisions, s: 100, l: 50});
    }
}