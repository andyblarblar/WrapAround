// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code

var lob1 = document.getElementById("lobby-1");
lob1.innerHTML = "haha";

//function addPlayerToLobby() {
//    connection.invoke("AddPlayer");
//}

function funny() {
    lob1.innerHTML += "\nhaha";
}

setInterval(funny, 10);

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/GameHub.cs")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(function () {
    console.log("connected");
});


function globalRun() {



}

setInterval(globalRun, 50);