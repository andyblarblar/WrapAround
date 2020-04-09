"use strict";
// Get canvas element
const scn = document.getElementById("render-window");
// Get rendering context from canvas element
var ctx = scn.getContext("2d");
// Determines how 'distinct' the team color has to be (higher generally means further from gray)
const C_DISTINCTNESS = 20;
// Determines how different each paddle on a team is colored
const C_SCALE = 15;
const MAX_PLAYERS = 16;
// Consts for indexing colorHues array
const R = 0, G = 1, B = 2, TEAM1 = 0, TEAM2 = 1;

// Array of lobby button-div elements
const lobs = [
    document.getElementById("lobby-1"),
    document.getElementById("lobby-2"),
    document.getElementById("lobby-3"),
    document.getElementById("lobby-4")
];

for (let i=0; i < lobs.length; i++) {
    lobs[i].addEventListener("mousedown", e => {
        //add player if left clicking
        if (e.button === 0) {
            const i2 = i;
            joinLobby(i2);
        }
    });
}

var _lobbyCounts = [];
// 'Unique' user hash (unlikely to intersect) for this player
const userHash = genHash("Pl@ceh01d&r");
// Unique ID given by server
var userId;
// Context object given by server
var _context;
// JSON representation of this player's paddle
var playerPaddle =
{ id: 0, isOnRight: false, gameId: 0, hitbox: { topLeft: { X: 20, Y: 0 }, bottomRight: { X: 30, Y: 300} }, height: 0.0, hash: userHash, MAX_SIZE: 300 }

// Flag is on when in a lobby -- used to toggle keyevents
var gameLoaded = false;
const scnHeight = 703;
// Number of pixels each paddle moves in one keystroke
var padSpeed;

//################################Colors######################################

// Randomly determine the team colors for this client (each client will have different team colors)
var team1R = Math.floor(Math.random() * 256);
// 'Distinctness' check
while (true) {
    var team1G = Math.floor(Math.random() * 256);
    if (Math.abs(team1G - team1R) > C_DISTINCTNESS)
        break;
}
// 'Distinctness' check
while (true) {
    var team1B = Math.floor(Math.random() * 256);
    if (Math.abs(team1B - team1R) > C_DISTINCTNESS && Math.abs(team1B - team1G) > C_DISTINCTNESS)
        break;
}
// Color 2 is color 1's complement
var team2R = 255 - team1R;
var team2G = 255 - team1G;
var team2B = 255 - team1B;
// Update CSS vars with these colors
document.documentElement.style
    .setProperty('--team-1-color', formatColorString(team1R, team1G, team1B));
document.documentElement.style
    .setProperty('--team-2-color', formatColorString(team2R, team2G, team2B));
// initialize Hue array as colorHues[teamNumber][R/G/B][hueIndex]
var colorHues = [
    [
        [
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        ], [
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        ], [
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        ]
    ], [
        [
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        ], [
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        ], [
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        ]
    ]
];
// Set the first hue to the team color
colorHues[TEAM1][R][0] = team1R;
colorHues[TEAM1][G][0] = team1G;
colorHues[TEAM1][B][0] = team1B;
colorHues[TEAM2][R][0] = team2R;
colorHues[TEAM2][G][0] = team2G;
colorHues[TEAM2][B][0] = team2B;

// Checks the entire colorHues for specified @team to make sure color @index is unique and a real color
function ensureValidity(team,index) {
    let i;
    // returns false if color values aren't valid
    if (colorHues[team][R][index] < 0
     || colorHues[team][G][index] < 0
     || colorHues[team][B][index] < 0
     || colorHues[team][R][index] > 255
     || colorHues[team][G][index] > 255
     || colorHues[team][B][index] > 255) return false;
    // returns false if the color isn't unique
    for (i = 0; i < colorHues.length; ++i) {
        if (i == index) continue;
        if (colorHues[team][R][i] == colorHues[team][R][index]
         && colorHues[team][G][i] == colorHues[team][G][index]
         && colorHues[team][B][i] == colorHues[team][B][index]) return false;
    }
    // otherwise, the color is valid
    return true;
}

// Initializes the colorHues array
function setUpColors() {
    let i, j;
    for (i = 0; i < 2; ++i) {
        for (j = 1; j < 16; ++j) {
            colorHues[i][R][j] = colorHues[i][R][0] + (getRndInteger(-2, 3) * C_SCALE);
            colorHues[i][G][j] = colorHues[i][G][0] + (getRndInteger(-2, 3) * C_SCALE);
            colorHues[i][B][j] = colorHues[i][B][0] + (getRndInteger(-2, 3) * C_SCALE);
            if (!ensureValidity(i, j))--j;
            console.log(colorHues);
        }
    }
}

// and call it immediately
// (setUpColors only exists for convenience and to keep i and j local scope, not that it matters much)
setUpColors();

// Connect to server
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/game")
    .withAutomaticReconnect()
    .build();

// Generates a hash based on the input string and the current time (miniscule change of non-unique hash)
function genHash(seed) {
    let hash = 0, i, chr, time = new Date();
    seed += (time.getTime() % 999997).toString();
    if (seed.length === 0) return hash;
    for (i = 0; i < seed.length; i++) {
        chr = seed.charCodeAt(i);
        hash = ((hash << 5) - hash) + chr;
        hash |= 0;
    }
    // Hash is truncated for convenience
    return Math.abs(hash).toString().slice(0,7); 
}

// Once connected
connection.start().then(function () {
    console.log("connection initialized ;)");
    // Add event listeners to lobby buttons on mouseover
    for (var i = 0; i < 4; ++i) {
        lobs[i].addEventListener("mouseover", () => { connection.invoke("GetLobbyPlayerCounts"); });
    }
    // and update them
    connection.invoke("GetLobbyPlayerCounts");
});

// Callback from GetLobbyPlayerCounts, updates the button innerHTML
connection.on("ReceiveLobbyCounts", (lobbyCounts) => {
    _lobbyCounts = lobbyCounts;
    var i;
    for (i = 0; i < 4; ++i) {
        lobs[i].getElementsByClassName("player-count-span")[0].innerHTML = _lobbyCounts[i];
    }
});

// Called every 17ms when getting context from the server
connection.on("ReceiveContextUpdate", (context) => {
    // Store the context
    _context = context;
    
    // Search for this player's paddle by ID
    context.players.forEach((item) => {

        //change paddle height if others have joined or left
        if (item.id === playerPaddle.id && item.height !== null && playerPaddle.height !== item.height) {
            playerPaddle.height = item.height;
            playerPaddle.hitbox.bottomRight = {X: 0, Y: playerPaddle.hitbox.topLeft.Y + playerPaddle.height}
        }
        
    });
    //else
    //console.log("Player not found in context");
    // Update paddle speed based on inverse of height
    padSpeed = 300.0 / Math.pow(playerPaddle.height, 0.85);

});

// Returns a css string of the Color passed in
function formatColorString(r, g, b) {
    return "rgb(" + r + ", " + g + ", " + b + ")";
}

// Get random int between min (inc) and max (exc)
function getRndInteger(min, max) {
    return Math.floor(Math.random() * (max - min)) + min;
}

// Called after each context update, renders that info to the scn canvas
function render(context) {

    //ctx.clearRect(0, 0, ctx.width, ctx.height);

    // Render Background and clear previous frame
    ctx.fillStyle = "rgb(255,255,255)";
    ctx.fillRect(0, 0, scn.width, scn.height);

    // Render Blocks
    let blockList = context.currentMap.blocks;

    if (typeof blockList !== undefined) {
        blockList.forEach((item) => {
            //if block is not destroyed
            if (item.health > 0) {
                ctx.fillStyle = formatColorString(item.color.r, item.color.g, item.color.b);
                ctx.fillRect(item.hitbox.topLeft.X, item.hitbox.topLeft.Y, 40, 20);
            }

        });
    }

    // Render Ball
    ctx.fillStyle = "rgb(140,140,140)";
    ctx.fillRect(context.ball.hitbox.topLeft.X, context.ball.hitbox.topLeft.Y, 10, 10);

    // Render Paddles
    let paddleList = context.players, i = 0;
    paddleList.forEach((item) => {
        // Fill the rects with the appropriate hue from colorHues
        if (item.id !== playerPaddle.id) {
            ctx.fillStyle = formatColorString(colorHues[item.isOnRight ? 1 : 0][R][i],
                colorHues[item.isOnRight ? 1 : 0][G][i],
                colorHues[item.isOnRight ? 1 : 0][B][i]);
            ctx.fillRect(item.hitbox.topLeft.X, item.hitbox.topLeft.Y, 10, item.height);
        }
        // And index the next color after drawing
        ++i;
    });

    //render self using local data to decouple from server updates
    ctx.fillRect(playerPaddle.hitbox.topLeft.X, playerPaddle.hitbox.topLeft.Y, 10, playerPaddle.height);

    
   // console.log(context.ball.hitbox.topLeft.X)

}

// Called when a lobby box is clicked by the user, sends an addplayer request to the hub
function joinLobby(lobbyId) {
    playerPaddle.gameId = lobbyId;
    connection.invoke("AddPlayer", lobbyId, playerPaddle.isOnRight, userHash);
    gameLoaded = true;
    //position player paddle
    playerPaddle.hitbox.topLeft = playerPaddle.isOnRight ? { X: 1250 - 20, Y: 0 } : { X: 20, Y: 0 };
}

// Callback from AddPlayer, saves the server-given ID for later
connection.on("ReceiveId", (id) => {
    userId = id;
    playerPaddle.id = userId;
    playerPaddle.gameId = lobbyId;
});

// Called when the X is clicked in the modal, closes the canvas and leaves the lobby
function leaveLobby() {
    connection.invoke("RemovePlayerFromLobby", playerPaddle);
    // Disable keyevents
    gameLoaded = false;
    resetPaddle();
}

//leave lobby when window is closed
window.onunload = e => leaveLobby();


//#########Key Handles##############

//helper object that fires keypress events to allow for smoother game play
var Key = {
    _pressed: {},

    UP: 38,
    DOWN: 40,

    isDown: function (keyCode) {
        return this._pressed[keyCode];
    },

    onKeydown: function (event) {
        this._pressed[event.keyCode] = true;
    },

    onKeyup: function (event) {
        delete this._pressed[event.keyCode];
    }
};

window.addEventListener("keyup", function (event) { Key.onKeyup(event); }, false);
window.addEventListener("keydown", function (event) { Key.onKeydown(event); }, false);


//send location every 30ms to server to avoid spam
setInterval(() => {
        if (gameLoaded) {
            connection.invoke("UpdatePlayerPosition",
                playerPaddle.hash,
                playerPaddle.hitbox.topLeft.X,
                playerPaddle.hitbox.topLeft.Y,
                playerPaddle.gameId,
                playerPaddle.id);
        }
    },
    30);

//update loop. Changes and renders your paddle regardless of server updates
setInterval((e) => {

        // Ignore events if game is not loaded
        if (gameLoaded) {

            // Move up/down if the move can be made in-bounds
            if (Key.isDown(Key.UP) && playerPaddle.hitbox.topLeft.Y > 0) {

                playerPaddle.hitbox.topLeft.Y -= padSpeed;
                playerPaddle.hitbox.bottomRight.Y -= padSpeed;

            } else if (Key.isDown(Key.DOWN) && playerPaddle.hitbox.bottomRight.Y < scnHeight) {

                playerPaddle.hitbox.topLeft.Y += padSpeed;
                playerPaddle.hitbox.bottomRight.Y += padSpeed;
            }
            //re-render
            render(_context);

        }
    },
    5);

//resets the defaults of a paddle for its side. Does not effect ids.
function resetPaddle() {
    playerPaddle.height = playerPaddle.MAX_SIZE;
    //reset hitbox
    playerPaddle.hitbox.topLeft = playerPaddle.isOnRight ? { X: 1250 - 20, Y: 0 } : { X: 20, Y: 0 };
    playerPaddle.hitbox.bottomRight = { X: 30, Y: playerPaddle.hitbox.topLeft.Y + playerPaddle.MAX_SIZE }


}

