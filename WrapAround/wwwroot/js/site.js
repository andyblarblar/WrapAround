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
var _context;
var playerPaddle;
var playerStateFetched = false;
var gameLoaded = false;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/game")
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Trace)
    .build();

// Generates a unique hash based on the input string and the current time
function genHash(seed) {
    var hash = 0, i, chr, time = new Date();
    seed += (time.getTime() % 6703).toString();
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

//Global function loop
function loop() {

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
            // This probably won't work yet -- See render()
            /*context.players.forEach((item) => {
                if (item.Hash == userHash)
                    playerPaddle = item;
            });
            playerStateFetched = true;*/
        }
        render(context);
    });
}

// Returns a css string of the Color passed in
function formatColorString(color) {
    return "rgb(" + color.R.toString() + ", " + color.G.toString() + ", " + color.B.toString() + ")";
}

// Called after each context update, renders that info to the scn canvas
function render(context) {

    gameLoaded = true;
    let ctx = scn.getContext("2d");

    // Render Background and clear previous frame
    ctx.fillStyle = "rgb(255,255,255)";
    ctx.fillRect(0, 0, scn.width, scn.height);

    /*
     * ALL OF THIS DOESN'T WORK YET -- the received context is undefined in many places right now
     * /

    // Render Blocks
    //let blockList = context.currentMap.Blocks;
    /*blockList.forEach((item) => {
        ctx.fillStyle = formatColorString(item.Color);
        ctx.fillRect(item.Hitbox.TopLeft.X, item.Hitbox.TopLeft.Y, 40, 20);
    });*/

    // Render Ball (Just a rectangle right now, change to texture when available)
    //ctx.fillStyle = "rgb(140,140,140)";
    //ctx.fillRect(context.ball.Hitbox.TopLeft.X, context.ball.Hitbox.TopLeft.Y, 10, 10);

    // Render Paddles (Just rectangles right now, change to texture when available)
    /*let paddleList = context.players;
    paddleList.forEach((item) => {
        ctx.fillStyle = "rgb(" + Math.floor(Math.random() * 256).toString() + ", " + Math.floor(Math.random() * 256).toString() + ", " + Math.floor(Math.random() * 256).toString() + ")";
        ctx.fillRect(item.Hitbox.TopLeft.X, item.Hitbox.TopLeft.Y, 10, item.Height);
    });*/

}

// Called when a lobby box is clicked by the user, sends an addplayer request to the hub
function joinLobby(lobbyId) {
    connection.invoke("AddPlayer", lobbyId, true, userHash);
    connection.on("ReceiveId", (id) => {
        userId = id;
    });
}

// Called when the X is clicked in the modal
function leaveLobby() {
    // Doesn't really work yet, since playerPaddle is undef b/c the context is undef
    connection.invoke("RemovePlayerFromLobby", playerPaddle);
    gameLoaded = false;
}

// Added to document because that's what worked *shrug*
document.addEventListener("keydown", event => {
    if (gameLoaded) {
        console.log(event.code);
    }
});

// Loop every 17ms
setInterval(loop,17);