
//function addPlayerToLobby() {
//    connection.invoke("AddPlayer");
//}


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