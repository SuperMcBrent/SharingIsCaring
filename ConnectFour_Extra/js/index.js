let board = [];
let colors = [];
const squareSize = 30;

const offsetTop = 20;
const offsetLeft = 20;

const connectN = 4;
let gridHeight;
let gridWidth;

let setup = () => {
    CreateColors(2);
    SetGridDimensions();
    console.log("Grid Dimensions are W: " + gridWidth + " and H: " + gridHeight + ".")
}

window.addEventListener("load", setup);

let SetGridDimensions = () => {
    gridHeight = connectN + (1 / 4 * connectN);
    gridWidth = connectN + connectN - 1;
}

let CreateColors = (complexity) => {
    let divisions = 360 / complexity;
    for (let index = 0; index < complexity; index++) {
        colors.push({value: String.fromCharCode(asciiA + index), h: 0 + index * divisions, s: 100, l: 50});
    }
}