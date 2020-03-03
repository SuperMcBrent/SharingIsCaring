let board = [];
let colors = [];
const squareSize = 80;
let dropSpeed = Math.ceil(squareSize / 20);
const asciiA = 65;

const offsetTop = 20;
const offsetLeft = 20;
const scale = 5;
let gridHeight;
let gridWidth;

let playerOneKey_UP = 81; //q
let playerOneKey_DOWN = 65; //a
let playerTwoKey_UP = 105; //numpad9
let playerTwoKey_DOWN = 102; //numpad6



let setup = () => {

}

window.addEventListener("load", setup);