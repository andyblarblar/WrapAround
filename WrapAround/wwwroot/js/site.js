// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code

var 

var lobbyButton = document.createElement("div");
var lobbyTitle = document.createElement("div");
var lobbyNumP = document.createElement("div");
lobbyButton.appendChild(lobbyTitle);
lobbyButton.appendChild(lobbyNumP);
document.body.appendChild(lobbyButton);

lobbyTitle.innerHTML("haha");
lobbyNumP.innerHTML("funny123");

function addPlayerToLobby() {
    connection.invoke("AddPlayer",)
}

lobbyButton.addEventListener('click', addPlayerToLobby)

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/GameHub.cs")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(function () {
    console.log("connected");
});



