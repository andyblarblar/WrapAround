"use strict";
const scn = document.getElementById("render-window");
const exit = document.getElementById("close-modal");
const C_DISTINCTNESS = 20;
const MAX_PLAYERS = 16;

const lobs = [
    document.getElementById("lobby-1"),
    document.getElementById("lobby-2"),
    document.getElementById("lobby-3"),
    document.getElementById("lobby-4")
];

var _lobbyCounts = [];
const userHash = genHash("Pl@ceh01d&r");
var userId;
var _context;
var playerPaddle = {
    id: 0, isOnRight: false, gameId: 0, hitbox: { topLeft: { X: 20, Y: 0 }, bottomRight: { X: 20, Y: 0 } }, height: 0.0, hash: userHash, MAX_SIZE: 300, position: { X: 20, Y: 0 }
};
var playerStateFetched = false;
var gameLoaded = false;
const scnHeight = 703;
var padSpeed;
document.documentElement.style
    .setProperty('--team-1-color', team1Color);
document.documentElement.style
    .setProperty('--team-2-color', team2Color);
var team1R = Math.floor(Math.random() * 256);
while (true) {
    var team1G = Math.floor(Math.random() * 256);
    if (Math.abs(team1G - team1R) > C_DISTINCTNESS)
        break;
}
while (true) {
    var team1B = Math.floor(Math.random() * 256);
    if (Math.abs(team1B - team1R) > C_DISTINCTNESS && Math.abs(team1B - team1G) > C_DISTINCTNESS)
        break;
}
var team2R = 255 - team1R;
var team2G = 255 - team1G;
var team2B = 255 - team1B;
var team1Color = formatColorString(team1R, team1G, team1B);
var team2Color = formatColorString(team2R, team2G, team2B);

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/game")
    .withAutomaticReconnect()
    .build();

// Generates a hash based on the input string and the current time (miniscule change of non-unique hash)
function genHash(seed) {
    var hash = 0, i, chr, time = new Date();
    seed += (time.getTime() % 999997).toString();
    if (seed.length === 0) return hash;
    for (i = 0; i < seed.length; i++) {
        chr = seed.charCodeAt(i);
        hash = ((hash << 5) - hash) + chr;
        hash |= 0;
    }
    return Math.abs(hash).toString().slice(0,7); 
};

connection.start().then(function () {
    console.log("connection initialized ;)");
    for(var i = 0; i < 4; ++i) {
        lobs[i].addEventListener("mouseover", () => { connection.invoke("GetLobbyPlayerCounts"); });
    }
    connection.invoke("GetLobbyPlayerCounts");
})



connection.on("ReceiveLobbyCounts", (lobbyCounts) => {
    _lobbyCounts = lobbyCounts;
    var i;
    for (i = 0; i < 4; ++i) {
        lobs[i].getElementsByClassName("player-count-span")[0].innerHTML = _lobbyCounts[i];
    }
});

connection.on("ReceiveContextUpdate", (context) => {
    _context = context;
    //console.log(playerPaddle);
    context.players.forEach((item) => {
        console.log(item);
        if (item.id == playerPaddle.id) {
            playerPaddle.hitbox.topLeft.Y = item.hitbox.topLeft.Y;
            playerPaddle.hitbox.bottomRight.Y = item.hitbox.bottomRight.Y;
            playerPaddle.height = item.height;
            playerPaddle.position.Y = item.hitbox.topLeft.Y;
            //console.log(playerPaddle);
        }
        
    });
    //else
    //console.log("Player not found in context");
    padSpeed = 300.0 / Math.pow(playerPaddle.height,0.75);
    render(context);
});

// Returns a css string of the Color passed in
function formatColorString(r, g, b) {
    return "rgb(" + r + ", " + g + ", " + b + ")";
}

// Called after each context update, renders that info to the scn canvas
function render(context) {

    gameLoaded = true;
    let ctx = scn.getContext("2d");

    // Render Background and clear previous frame
    ctx.fillStyle = "rgb(255,255,255)";
    ctx.fillRect(0, 0, scn.width, scn.height);

    // Render Blocks
    let blockList = context.currentMap.blocks;
    if (typeof blockList !== undefined) {
        blockList.forEach((item) => {
            ctx.fillStyle = formatColorString(item.color.R,item.color.G,item.color.B);
            ctx.fillRect(item.hitbox.topLeft.X, item.hitbox.topLeft.Y, 40, 20);
        });
    }

// Render Ball (Just a rectangle right now, change to texture when available)
    ctx.fillStyle = "rgb(140,140,140)";
    ctx.fillRect(context.ball.hitbox.topLeft.X, context.ball.hitbox.topLeft.Y, 10, 10);

    // Render Paddles (Just rectangles right now, change to texture when available)
    let paddleList = context.players;
    paddleList.forEach((item) => {
        ctx.fillStyle = team1Color;
        ctx.fillRect(item.hitbox.topLeft.X, item.hitbox.topLeft.Y, 10, item.height);
    });

   // console.log(context.ball.hitbox.topLeft.X)

}

// Called when a lobby box is clicked by the user, sends an addplayer request to the hub
function joinLobby(lobbyId) {
    connection.invoke("AddPlayer", lobbyId, playerPaddle.isOnRight, userHash);
}

connection.on("ReceiveId", (id) => {
    userId = id;
    playerPaddle.id = userId;
    playerPaddle.gameId = lobbyId;
});

// Called when the X is clicked in the modal
function leaveLobby() {
    connection.invoke("RemovePlayerFromLobby", playerPaddle);
    gameLoaded = false;
}

// Added to document because that's what worked *shrug*
document.addEventListener("keydown", event => {
    if (gameLoaded) {
        
        console.log(event.code);
        console.log(playerPaddle.position.Y);
        console.log(playerPaddle.position.X);
        if (event.code === "ArrowUp" && playerPaddle.hitbox.topLeft.Y > 0) {
            console.log("UP");
            playerPaddle.hitbox.topLeft.Y -= padSpeed;
            playerPaddle.hitbox.bottomRight.Y -= padSpeed;
            playerPaddle.position.Y -= padSpeed;
        } else if (event.code === "ArrowDown" && playerPaddle.hitbox.bottomRight.Y < scnHeight) {
            playerPaddle.hitbox.topLeft.Y += padSpeed;
            playerPaddle.hitbox.bottomRight.Y += padSpeed;
            playerPaddle.position.Y += padSpeed;
            console.log("DOWN");
        }
        if (event.code === "ArrowUp" || event.code === "ArrowDown") {
            connection.invoke("UpdatePlayerPosition", playerPaddle.hash, playerPaddle.position.X, playerPaddle.position.Y, playerPaddle.gameId, playerPaddle.id);
            console.log("MOVE SENT");
        }
    }
});

// Loop every 17ms
//setInterval(loop,170);