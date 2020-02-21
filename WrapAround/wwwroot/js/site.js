"use strict";
const scn = document.getElementById("render-window");
const lobs = [
    document.getElementById("lobby-1"),
    document.getElementById("lobby-2"),
    document.getElementById("lobby-3"),
    document.getElementById("lobby-4")
];

var _lobbyCounts = [];

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/game")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Trace)
    .build();

function genHash() {

}

connection.start().then(function () {
    console.log("connection initialized ;)");
})

//Global function loop
function loop() {
    connection.invoke("GetLobbyPlayerCounts");

    connection.on("ReceiveLobbyCounts", (lobbyCounts) => {
        _lobbyCounts = lobbyCounts;
        var i;
        for (i = 0; i < 4; ++i) {
            lobs[i].getElementsByClassName("player-count-span").innerHTML = _lobbyCounts[i];
        }
    });
}

function joinLobby(lobbyId) {
    connection.invoke("AddPlayer",lobbyId,)
}

setInterval(loop,30);