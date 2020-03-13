"use strict"

var modalBGOpaque = false;
function lightsOut() {
    const element = document.getElementById("game-modal");
    if (modalBGOpaque) {
        element.style.background = "rgba(0,0,0,0.5)";
        modalBGOpaque = false;
    } else {
        element.style.background = "rgba(0,0,0,1.0)";
        modalBGOpaque = true;
    }
}

function changeSide() {
    playerPaddle.isOnRight = !playerPaddle.isOnRight;
    document.getElementById("side-active").setAttribute("id", "side-inactive");
    document.getElementById("side-inactive").setAttribute("id", "side-active");
}