"use strict";
const scn = document.getElementById("render-window");
const lobs = [
    document.getElementById("lobby-1"),
    document.getElementById("lobby-2"),
    document.getElementById("lobby-3"),
    document.getElementById("lobby-4")
];

const _lobbyCounts = [];

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/game")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Trace)
    .build();

connection.start().then(function () {
    console.log("connected");
})

connection.invoke("GetLobbyPlayerCounts");

connection.on("ReceiveLobbyCounts", (lobbyCounts) => {
    _lobbyCounts = lobbyCounts;
    //update lobby boxes
});