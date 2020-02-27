const length = 4;
const complexity = 8;
const rounds = 10;
const asciiA = 65;

let board = [];
let guesses = [];
let colors = [];
let code = "";

let currentLine = 0;

const squareSize = 20;

const offsetTop = 20;
const offsetLeft = 20;

const setup = () => {
    CreateColors(complexity);
    console.log(colors);
    CreateCode();
    console.log(code);
    Compare(code,"AAAA");
}

let AddGuess = (guessString) => {
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
    console.log("Drawing " + code + " at top: " + y + ", left: " + x + ".");
    let baseVakje = document.createElement("p");
    baseVakje.classList.add("vakje");
    baseVakje.id = x + "," + y;
    baseVakje.style.position = "absolute";
    baseVakje.style.left= x +"px";
    baseVakje.style.top= y + "px";
    baseVakje.style.border = "1px solid black";
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
            console.log(codeString[i]);
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
        //trigger winEvent.
    }

    // TEMP
    
    DrawGuess(guess,currentLine);

    //store value somewhere, tuples?
}

let CreateCode = () => {
    for (let index = 0; index < length; index++) {
        code += String.fromCharCode(Math.floor(Math.random()*4+asciiA));        
    }
}

window.addEventListener("load", setup);

let CreateColors = (complexity) => {
    let divisions = 360 / complexity;
    for (let index = 0; index < complexity; index++) {
        colors.push({value: String.fromCharCode(asciiA + index), h: 0 + index * divisions, s: 100, l: 50});
    }
}