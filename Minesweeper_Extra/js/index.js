let board = [];
const mine = -1;
let size = 30;
let bomdichtheid = 0.12;

let kleuren = [];
//kleuren.push({value: -2, r: 64, g: 64, b: 64});
kleuren.push({value: -1, r: 163, g: 0, b: 0});
kleuren.push({value: 0, r: 128, g: 128, b: 128});
kleuren.push({value: 1, r: 138, g: 193, b: 0});
kleuren.push({value: 2, r: 0, g: 148, b: 255});
kleuren.push({value: 3, r: 178, g: 0, b: 255});
kleuren.push({value: 4, r: 255, g: 0, b: 110});
kleuren.push({value: 5, r: 255, g: 106, b: 0});
kleuren.push({value: 6, r: 255, g: 0, b: 0});
kleuren.push({value: 7, r: 255, g: 0, b: 0});
kleuren.push({value: 8, r: 255, g: 0, b: 0});

let updateKleurVanVakje = (x,y) => {
    let vakje = vakjeSelector(x,y);
    kleuren.forEach(element => {
        if (vakje.value == element.value) {
            document.getElementById(x+ "," + y).style.backgroundColor = "rgb(" + element.r + "," + element.g + "," + element.b + ")";
        }
    });
}

const setup = () => {
    //document.body.getElementsByClassName("vakje")[0].addEventListener("click", function(){ updateLus();displayBoard()});
    generateBoard(size);
    populeerVeldMetBommen(bomdichtheid);
    updateValuesVanVakjesRondBommen();
    displayBoard();
    console.log(board);
}

window.addEventListener("load", setup);

let onclickEvent = (x,y) => {
    let vakje = vakjeSelector(x,y);
    //console.log("Vakje " + x + "," + y + " is aangelikt.");
    if (vakje.value == mine) {
        zetAlleVakjesOpZichtbaar();
        updateAlleVakjesOpHetScherm();
        //alert("Je bent verloren");
    } else if (vakje.value == 0) {
        //recursiefverkennen nog toe te voegen.
        //updateVakjeInArray(x,y,vakje.value,true);
        //updateVakjeOpScherm(x,y);
        recursief(x,y);
    } else {
        updateVakjeInArray(x,y,vakje.value,true);
        updateVakjeOpScherm(x,y);
    }
}

let recursief = (_x,_y) => {
    for (let x = -1; x < 2; x++) {
        for (let y = -1; y < 2; y++) {
            if (x + _x < size && y + _y < size && x + _x >= 0 && y + _y >= 0) {
                let vakje = vakjeSelector(_x + x, _y + y);
                if (vakje.value !== mine) {
                    if (vakje.visible === false) {
                        updateVakjeInArray(vakje.x,vakje.y,vakje.value,true);
                        updateVakjeOpScherm(vakje.x,vakje.y);
                        if (vakje.value === 0) {
                            console.log("ik ben nog een 0 tegengekomen")
                            recursief(vakje.x,vakje.y);
                        }
                    }
                }
            }
        }
    }
}

let updateValuesVanVakjesRondBommen = () => {
    board.forEach(element => {
        if (element.value == mine) {
            for (let x = -1; x < 2; x++) {
                for (let y = -1; y < 2; y++) {
                    if (x + element.x < size && y + element.y < size && x + element.x >= 0 && y + element.y >= 0) {
                        if (vakjeSelector(element.x + x, element.y + y).value !== mine) {
                            vakjeSelector(element.x + x, element.y + y).value++;
                        }
                    }
                }
            }
        }
    });
}

let populeerVeldMetBommen = (percentage) => {
    if (percentage < 0.01 || percentage > 0.5) {
        percentage = 0.1;
    }
    let aantalBommen = Math.floor(board.length * percentage)

    console.log("Er worden " + aantalBommen + " bommen gemaakt voor " + board.length + " vakjes.")

    board[0].value = mine;

    for (let index = 0; index < aantalBommen; index++) {
        let random = Math.floor(Math.floor(Math.random() * board.length)); // + 1 toevoegen
        //console.log("Een bom wordt toegevoeg op: ");
        //console.log(board[random]);
        board[random].value = mine;
    }
}

let updateVakjeInArray = (x,y,value,visible) => {
    let vakje = vakjeSelector(x,y);
    vakje.value = value;
    vakje.visible = visible;
}

let zetAlleVakjesOpZichtbaar = () => {
    //console.log("Alle vakjes worden nu getoond.");
    board.forEach(element => {
        updateVakjeInArray(element.x,element.y,element.value,true);
    });
}

let updateAlleVakjesOpHetScherm = () => {
    board.forEach(element => {
        updateVakjeOpScherm(element.x,element.y);
    });
}

let updateVakjeOpScherm = (x,y) => {
    let vakje = vakjeSelector(x,y);
    let vakjeOpScherm = document.getElementById(x + "," + y);
    if (vakje.visible === true) {
        if (vakje.value == mine) {
            vakjeOpScherm.innerHTML = "💥";
            //console.log(val);
        } else if (vakje.value == 0) {
            vakjeOpScherm.value = "";
        } else {
            vakjeOpScherm.innerHTML = vakje.value;
        }
    } else {
        vakjeOpScherm.value = "";
    }
    updateKleurVanVakje(x,y);
}

let vakjeSelector = (x,y) => {
    return board.find(element => element.x == x && element.y == y);
}

let voegVakjeToe = () => {
    let baseVakje = document.createElement("p");
    baseVakje.classList.add("vakje");
    let baseVakjeContent = document.createTextNode("0");
    baseVakje.appendChild(baseVakjeContent);
    
    document.body.appendChild(baseVakje);
}

let voegVakjeToeCoords = (x,y) => {
    let offsetleft = 20;
    let offsettop = 20;
    let baseVakje = document.createElement("p");
    baseVakje.classList.add("vakje");
    baseVakje.id = x + "," + y;
    baseVakje.style.position = "absolute";
    baseVakje.style.left=offsetleft + (x * 20 ) +"px";
    baseVakje.style.top=offsettop + ( y * 20 ) + "px";
    baseVakje.style.border = "1px solid black";
    //baseVakje.style.backgroundColor = "darkgray";
    //baseVakje.onclick = function() {onclickEvent(x,y);};
    baseVakje.onclick = () => {onclickEvent(x,y);};
    let baseVakjeContent = document.createTextNode("");
    baseVakje.appendChild(baseVakjeContent);
    document.body.appendChild(baseVakje);
}

let generateBoard = (dim) => {
    for (let heigth = 0; heigth < dim; heigth++) {
        for (let width = 0; width < dim; width++) {
            board.push({x: width, y: heigth, value: 0, visible: false});
        }
    }
}

let displayBoard = () => {
    //vakjes toevoegen aan html
    board.forEach(element => {
        voegVakjeToeCoords(element.x,element.y);
    });
}