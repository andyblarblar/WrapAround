"use strict";
const scn = document.getElementById("render-window");
const exit = document.getElementById("close-modal");

const lobs = [
    document.getElementById("lobby-1"),
    document.getElementById("lobby-2"),
    document.getElementById("lobby-3"),
    document.getElementById("lobby-4")
];

var _lobbyCounts = [];
const userHash = genHash("Pl@ceh01d&r");
var userId;
var _context = null;
var playerPaddle = {
    "Id": 0, "IsOnRight": true, "GameId": 0, "Hitbox": { "TopLeft": { "X": 0, "Y": 0 }, "BottomRight": { "X": 0, "Y": 0 } }, "Height": 0.0};
var playerStateFetched = false;
var gameLoaded = false;
var paddleR = Math.floor(Math.random() * 256).toString();
var paddleG = Math.floor(Math.random() * 256).toString();
var paddleB = Math.floor(Math.random() * 256).toString();
const scnHeight = 703;



const connection = new signalR.HubConnectionBuilder()
    .withUrl("/game")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Trace)
    .build();

// Generates a unique hash based on the input string and the current time
function genHash(seed) {
    var hash = 0, i, chr, time = new Date();
    seed += (time.getTime() % 999997).toString();
    if (seed.length === 0) return hash;
    for (i = 0; i < seed.length; i++) {
        chr = seed.charCodeAt(i);
        hash = ((hash << 5) - hash) + chr;
        hash |= 0;
    }
    return Math.abs(hash).toString();
};

connection.start().then(function () {
    console.log("connection initialized ;)");
})

connection.invoke("GetLobbyPlayerCounts");

connection.on("ReceiveLobbyCounts", (lobbyCounts) => {
    _lobbyCounts = lobbyCounts;
    var i;
    for (i = 0; i < 4; ++i) {
        lobs[i].getElementsByClassName("player-count-span")[0].innerHTML = _lobbyCounts[i];
    }
});

connection.on("ReceiveContextUpdate", (context) => {
    _context = context;
    if (!playerStateFetched) {
        context.players.forEach((item) => {
            if (item.Id === playerPaddle.Id)
                playerPaddle.Hitbox = item.Hitbox;
        });
        if (playerPaddle != null)
            playerStateFetched = true;
        //else
        //console.log("Player not found in context");
    }
    render(context);
});

//Global function loop
function loop() {

}

// Returns a css string of the Color passed in
function formatColorString(color) {
    return "rgb(" + color.R.toString() + ", " + color.G.toString() + ", " + color.B.toString() + ")";
}

// Called after each context update, renders that info to the scn canvas
function render(context) {

    console.log(context);

    gameLoaded = true;
    let ctx = scn.getContext("2d");

    // Render Background and clear previous frame
    ctx.fillStyle = "rgb(255,255,255)";
    ctx.fillRect(0, 0, scn.width, scn.height);

    // Render Blocks
    let blockList = context.currentMap.blocks;
    if (blockList != null) {
        blockList.forEach((item) => {
            ctx.fillStyle = formatColorString(item.color);
            ctx.fillRect(item.hitbox.topLeft.X, item.hitbox.topLeft.Y, 40, 20);
        });
    }

// Render Ball (Just a rectangle right now, change to texture when available)
    ctx.fillStyle = "rgb(140,140,140)";
    ctx.fillRect(context.ball.hitbox.topLeft.X, context.ball.hitbox.topLeft.Y, 10, 10);

    // Render Paddles (Just rectangles right now, change to texture when available)
    let paddleList = context.players;
    paddleList.forEach((item) => {
        ctx.fillStyle = "rgb(" + paddleR + ", " + paddleG + ", " + paddleB + ")";
        ctx.fillRect(item.hitbox.topLeft.X, item.hitbox.topLeft.Y, 10, item.height);
    });

}

// Called when a lobby box is clicked by the user, sends an addplayer request to the hub
function joinLobby(lobbyId) {
    connection.invoke("AddPlayer", lobbyId, true, userHash);
    connection.on("ReceiveId", (id) => {
        userId = id;
        playerPaddle.Id = userId;
        playerPaddle.GameId = lobbyId;
    });
}

// Called when the X is clicked in the modal
function leaveLobby() {
    connection.invoke("RemovePlayerFromLobby", playerPaddle);
    gameLoaded = false;
}

// Added to document because that's what worked *shrug*
document.addEventListener("keydown", event => {
    if (gameLoaded) {
        console.log(event.code);
        if (event.code === "ArrowUp" && playerPaddle.Hitbox.TopLeft.Y < 0) {
            playerPaddle.Hitbox.TopLeft.Y -= 0.1;
            playerPaddle.Hitbox.BottomRight.Y -= 0.1;
        } else if (event.code === "ArrowDown" && playerPaddle.Hitbox.BottomRight.Y > scnHeight) {
            playerPaddle.Hitbox.TopLeft.Y += 0.1;
            playerPaddle.Hitbox.BottomRight.Y += 0.1;
        }
        if (event.code === "ArrowUp" || event.code === "ArrowDown")
            connection.invoke("UpdatePlayerPosition", playerPaddle);
    }
});

// Loop every 17ms
//setInterval(loop,170);