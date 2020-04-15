[![Build Status](https://dev.azure.com/andyblarblar/andyblarblar/_apis/build/status/andyblarblar.WrapAround?branchName=master)](https://dev.azure.com/andyblarblar/andyblarblar/_build/latest?definitionId=1&branchName=master)

Welcome to the offical repository of WrapAround! WrapAround is a multiplayer game in the vain of Pong, but with 16 players per game and 
breakable blocks like in breakout. The game is made up of a few components. First, the game runtime is hosted in an ASP.NET Core server as an injectable singleton. From there, the game is served as static HTML and JS. This JS connects to the server using Signalr, and will relay commands to the server, as well as listening for updates. When in a game, the flow looks something like this: player moves in client -> JS sends move request to server -> server updates -> context send to client -> client updates 


## WrapAround

The WrapAround project contains 
