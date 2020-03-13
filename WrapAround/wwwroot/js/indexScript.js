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
    // Change actual player paddle variable
    playerPaddle.isOnRight = !playerPaddle.isOnRight;
    // Store elements for swapping
    var activeElement   = document.getElementById("side-active");
    var inactiveElement = document.getElementById("side-inactive");
    // Swap attributes
    activeElement.setAttribute("id", "side-inactive");
    inactiveElement.setAttribute("id", "side-active");
    activeElement.setAttribute("onmousedown", "changeSide()");
    inactiveElement.removeAttribute("onmousedown")
}