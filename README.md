[![Build Status](https://dev.azure.com/andyblarblar/andyblarblar/_apis/build/status/andyblarblar.WrapAround?branchName=master)](https://dev.azure.com/andyblarblar/andyblarblar/_build/latest?definitionId=1&branchName=master)

Welcome to the offical repository of WrapAround! WrapAround is a multiplayer game in the vain of Pong, but with 16 players per game and 
breakable blocks like in breakout. The game is made up of a few components. First, the game runtime is hosted in an ASP.NET Core server as an injectable singleton. From there, the game is served as static HTML and JS. This JS connects to the server using Signalr, and will relay commands to the server, as well as listening for updates. When in a game, the flow looks something like this: player moves in client -> JS sends move request to server -> server updates -> context send to client -> client updates. 


## WrapAround

The WrapAround project contains the main ASP.NET Core server that both serves the static wraparound site, and the games internal "runtime". The game logic is controlled by the `ServerLoop` object, which is injected as a singleton. This object contains the lobbys, the timers for sending updates, as well as interfaces for mutating the state of lobbys. This interface (litterally) is consumed by the `GameHub` SignalR hub. this hub accepts connections at the /game endpoint. Any requests to be added to a lobby, move to a new location ect. Are sent from the client to this endpoint, and are filtered through the abstractions until the underlieing object is mutated. the /gameMaps directory holds all of the maps the server will load created by the level editor. 

## WrapAround.Logic

This project contains the game logic and entitys unrelated to the server. `GameContext` is the main class here, and contains all of the state of a lobby, as well as the core update loop of the game. The rest of the project is pretty self documenting, looking through the folders is quite simple.

## WrapAround.LevelEditor

This project contains the level editor hosted on the website. The level editor is implimented using Blazor to reuse models from the WrapAround.Logic project. Maps are saved as a JSON serilized `GameMap` object.  
