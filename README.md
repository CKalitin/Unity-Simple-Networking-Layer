# Unity-Simple-Networking-Layer
A networking package for making multiplayer unity games.
U-SNL haha

Unity Version: 2021.3.15f1

Trello: https://trello.com/b/Fxe9qDjb

![Code Layer Diagram](https://github.com/CKalitin/Unity-Simple-Networking-Layer/blob/main/USNL_Layer-Diagram.png?raw=true)

# USNL Documentation
Documentation (Some of this is improperly formatted on Github, better to read the raw .md file):

This covers the USNL namespace. It contains all the functions needed for a user to use the package.
If you're changing the package itself, see Docs-Package.md.

See the example projects to see this in action. Many of your questions can be answered by examining it.

USNL contains 2 namespaces. USNL and USNL.Package.
The USNL namespace contains all the scripts you use to make a multiplayer game.
The USNL.Package namespace contains all the scripts that make the package work. You don't need to mess with these. 

## Installation
USNL uses a Server Client model so two Unity projects are required. Import the USNL-Server unity package on the Server and the USNL-Client unity package on the Client.

## Prefabs
USNL Management
It must be placed in the game scene and always be active. It does not have a DontDestroyOnLoad(), add your own.
The USNL Management prefab contains all necessary scripts. It is present on both the Client and Server, though they contain different scripts.
The childed Game Object, USNL Utilities, are all the scripts the user does not need to worry about.

The USNL Debug Menu Canvas prefab display debug info in-game. Use ctrl-backtick to toggle it.
It gets most of its information from the script Network Debug Info on the USNL Management prefab.
It has it's own Event System. Delete it if you have your own canvas.

## Callbacks
Callback Events are used throughout USNL.
Callback Events are setup in OnEnable() and OnDisable().
Events are passed a objects as a parameter. This is to pass any type of data to the function.
When a packet is received it passed the packet variables and information as a struct.

Server Callback Events:
OnServerStarted(object noData)
OnServerStopped(object noData)
OnClientConnected(int clientId)
OnClientDisconencted(int clientId)

Client Callback Events:
OnConnected(object noData)
OnDisconnected(object noData)

How to add a callback:
1. Create a function that will be used as the callback. Have an object as a parameter
2. Add the function to the callback events list.
    Eg. 
    In OnEnable() add USNL.CallbackEvents.OnClientConnected += MyFunction;
    Add the same in OnDisable(), except with -=.
3. Read the object parameter in the function. (Skip this if there is no data passed in the object)
    Eg.
    int clientId = (int)parameter;

## Configuring Packets
Configuring Packets:
In USNL Server/Config there is the Script Generator Scriptable Object.
On the Server and Client it is used to configure packets. Be sure the data is identical between the Server and Client.
Once a packet is configured and has been generated it can be used.

Packet Configuration Variables:
Text Field: Packet Name.
Send Type (Server Only): If the packet is to be sent to all clients, a single client, or all clients except one.
Protocol: TCP or UDP
Num variables: Number of variables

Packet Variable Configuration Variables:
Text Field: Variable name
Variable Type: Variable Type

## Send and Receiving Packets
Sending Packets:
Use USNL.PacketSend.(PacketName)() to send a packet.
The parameters are the variables specified in Script Generator.
Use intellisense to see in which order they are. Should be in the order you specified in the Script Generator.

Receiving Packets:
Events are used to receive packets in a script.

1. Create a function you want to receive the packet with a parameter of type object.
2. Add a callback event to receive the packet. 
   Eg. In OnEnable() add USNL.CallbackEvents.On(Packet Name)Packet += (Function Name);
   Add the same in OnDisable(), except with -=.
3. Read the object parameter.
    Eg.
    USNL.(Packet Name)Packet packet = (USNL.(Packet Name)Packet)objectParameter;

Code Example:
    private void OnEnable() {
        USNL.CallbackEvents.OnPlayerInputPacket += OnPlayerInputPacket;
    }

    private void OnDisable() {
        USNL.CallbackEvents.OnPlayerInputPacket -= OnPlayerInputPacket;
    }

    private void OnPlayerInputPacket(object packetObject) {
        USNL.PlayerInputPacket playerInputPacket = (USNL.PlayerInputPacket)packetObject;
        // FromClient can only be used on the Server
        Debug.Log("Client " + {playerInputPacket.FromClient} + " Input: " + new Vector2(playerInputPacket.XInput, playerInputPacket.YInput));
    }

## Starting and Stopping the Server
Code Example:
    public bool runServer;

    private void Update() {
        if (runServer && !USNL.ServerManager.instance.ServerActive) {
            USNL.ServerManager.instance.StartServer();
        }
        
        if (!runServer && USNL.ServerManager.instance.ServerActive) {
            USNL.ServerManager.instance.StopServer();
        }
    }

## Connecting and Disconnecting to the Server
You can use the Debug Menu on the Client to connect or disconnect while developing the game.

To hide the ip of the server it is converted to a 10 digit number known as an id.
Client Manager contains functions to convert between ip and id.
USNL.ClientManager.instance.IPtoID(string _ip); // Returns int id
USNL.ClientManager.instance.IDtoIP(int _id); // Returns string ip

Client Manager contains two functions to connect to the server:
USNL.ClientManager.instance.ConnectToServer();
USNL.ClientManager.instance.ConnectToServer(int serverId, int port)

Client Manager has one function to disconnect from the server:
USNL.ClientManager.instance.DisconnectFromServer()

Client Manager contains multiple public bool variables for tracking the current state of the client:
USNL.ClientManager.instance.IsConnected;
USNL.ClientManager.instance.IsAttemptingConnection
USNL.ClientManager.instance.IsHost

When ConnectToServer() is called the client tries to connect to the server for 10 seconds.
This can be stopped by calling:
USNL.ClientManager.instance.StopAttemptingConnection()

## Client-Hosted Server
Client Manager has two functions for hosting the server:
USNL.ClientManager.instance.LaunchServer();
USNL.ClientManager.instance.CloseServer();

This launches the server exe file specified in Client Manager.
See the popup of on the variable in the Server Host section of Client Manager for how to use them.

## Synced Objects
Synced Objects are shared between the Server and Client.
Synced Objects are GameObjects that have functionality on the server and are copied to the Client so the player can see them.

Server Synced Object Variables:
Synced Object Tag: This is a unique string that is used on the Client to spawn the correct prefab.
Use Local Change Values: The Synced Object Manager and Synced Object have minimum pos/rot/scale change variables. Toggle this to use the local values on the Synced Object.
Min Pos Change: Minimum position change needed to send an update to the client.
Min Rot Change: Minimum rotation change needed to send an update to the client.
Min Scale Change: Minimum scale change needed to send an update to the client.
Interpolation Toggles: Whether to interpolate this Synced Object. Disable if it's not needed to save bandwidth.

Client Synced Object Variables:
Interpolate: Toggle whether it should be interpolated. The packet associated with interpolation will still be sent unless it is toggled off on the server.

Synced Object Setup Guide:
On the Server, select the prefab you want to make a Synced Object and add the SyncedObject script to it.
Give it a unique tag.
On the Client create a prefab you want to be the equivilent of the Server's prefab and add the Synced Object script to it.
This is the prefab the player will see.
On the Client, go to USNL Server/Config/Synced Object Prefabs and add the prefab to the list of Synced Objects and give it the tag you chose earlier.

Now, when the Server's Synced Object is spawned, destroyed, moved, rotated, or scaled the same thing will happen to the Synced Object on the client.
Many instances of the same Synced Object can be uesd at once.

## Synced Object Manager
This script is on the USNL Management prefab.

Server Variables:
Vector2 Mode: Self-explanetory

Min Pos Change: Minimum position change needed to send an update to the client.
Min Rot Change: Minimum rotation change needed to send an update to the client.
Min Scale Change: Minimum scale change needed to send an update to the client.

Synced Object Client Update Rate: Seconds between Synced Object Update packets being sent to the client. The default value, 0.1667, is 30 times per second.
Nth Synced Object Update Per Interpolation: How many Synced Object Updates must be passed for an Interpolation Update to be sent to the client.
The rest of the Nth's Variables: Self-explanetory

Server Side Interpolation: Interpolation can be done Server-side or Client-side. Be sure to toggle it on the Client as well.
Interpolate Toggles: Self-explanetory

Client Variables:
Synced Object Prefabs: This is a reference to the Synced Object Prefabs Scriptable Object that contains a list of prefabs and their tags.
Local Interpolation: Interpolation can be done Server-side or Client-side. Local Interpolation makes it Client-side. Be sure to toggle it on the Server as well.

## Synced Object Interpolation
Interpolation can be done two ways. On the Server or on the Client.

In Client mode it takes the median of the previous 5 Synced Object Updates and interpolates based on that.
In Server mode it takes the median of the previous 5 frames and interpolates based on that.

Server mode results in smoother looking interpolation but comes at the expense of 2x bandwidth. This is ideal when trying to minimize Synced Object Updates per second.
Client mode is ideal when many Synced Object Updates are sent per second.

## Input Manager
The client send keyboard inputs whenever they happen automatically.
Mouse position is not sent because it is dependent on screen size.
This system results in the player only seeing the result of their inputs after the duration of the ping. It is not ideal.

Receiving Client inputs on the Server:
They keys a client has pressed is stored in a struct. Retrieve the struct like so:
USNL.ClientInput clientInput = USNL.InputManager.instance.GetClientInput(clientId);

ClientId is passed in the Callback Events OnClientConnected() and OnClientDisconnected().
You can use it to keep track of players.

The Client Input struct has many functions for getting keys or mouse buttons down:
They are identical to Unity's Input Manager
bool GetKeyDown(KeyCode keycode) // Called when key is pressed
bool GetKeyUp(KeyCode keycode) // Called when key is released
bool GetKey(KeyCode keycode) // Current state of the key
bool GetMouseButtonDown(string buttonName) // Called when mouse button is pressed
bool GetMouseButtonUp(string buttonName) // Called when mouse button is released
bool GetMouseButton(string buttonName) // Current state of the mouse button

If you'd like to send your own input you can toggle the Client to not send key pressed by itself and use the the function in InputManager.
Client Input Manager functions:
SetKeyDown(KeyCode keycode)
SetKeyUp(KeyCode keycode)
SetMouseButtonDown(string buttonName)
SetMouseButtonUp(strubg buttonName)

## Client Manager
### Inspector Variables
int serverID: Id the Client will connect to. This is overridden in one of the ConnectToServer functions.
int port: Port the Client will connect to. This is overridden in one of the ConnectToServer functions.

float attemptConnectionTime: How long the Client will attempt to connect to the Server.
float timeoutTime: How much time without packets received until the client is timedout from the Server.
ServerInfo serverInfo: Information about the Server the Client is connected to. (ServerName, ConnectedClientIds, MaxClients, ServerFull)

string serverExeName: Name of the Server exe file when it is built.
string serverPath: Path to the Server from the built Client project.
string editorServerPath: Global path to the Server when in Editor.
bool useApplicationPath: If true, it will add the path to the application to the begining of serverPath.
Package.ServerConfig serverConfig: Variables Associated with how the server will be run when the Client hosts it.

### Public Variables
int WanClientId: Other Clients use this to connect if they are no on the same network.
int LanClientId: Other Clients use this to connect if they are on the same network
string WanClientIp: Public IP
string LanClientIp: Local network IP

bool IsConnected.
bool IsAttemptingConnection.
bool IsHost.

DateTime TimeOfConnection: When the Client connected to the server.

Package.ServerConfig ServerConfig: Returns serverConfig.

bool IsServerRunning: If self-hosted Server is running.

string ServrName: Name of the server the Client is connected to.
ServerInfo ServerInfo: Returns serverInfo.

### User Functions
bool IsServerActive(): Returns true is self-hosted Server is active.

void ConnectToServer(): Connects to Server with ID and Port specified in ClientManager.
void ConnectToServer(int _id, int _port): Connects to Server with parameters.
void DisconnectFromServer(): Disconnects from Server.
void StopAttemptingConnection(): Stops attempting to connect to Server.

int IpToId(string _ip): Converts a string ip to an int id
string IdTpId(int _id): Converts an int id to a string ip

void LaunchServer(): Launches self-hosted Server on the Client's Computer
void CloseServer(): Closes self-hosted Server on the Client's Computer

## Server Manager
### Inspector Variables
ServerConfig serverConfig: USNL.Package.ServerConfig struct (serverName, Port, etc.)
bool useServerFilesInEditor: If ServerConfig should be saved and read when running game in editor. As opposed to using the values in serverConfig.

float timeoutTime: Seconds passed until a client is timed out. Should be slightly higher than timeoutTime on Client.
bool serverInfoPacketSendInterval: How often the Server Info Packet is sent with information about max players, etc.

### Public Variables
ServerConfig ServerConfig: USNL.Package.ServerConfig struct (serverName, Port, etc.)
DateTime TimeOfStartup: When ServerStart() was called
bool ServerActive: If server has successfully been started

int WanServerId: Clients use this to connect if they are not on the same network
int LanServerId: Clients use this to connect if they are on the same network
string WanServerIp: Public IP
string LanServerIp: Local network IP

### User Functions
void StartServer(): Starts Server.
void StopServer(): Stops Server.
static int GetNumberOfConnectedClients(): Returns number of connected Clients.
static int[] GetConnectedClientsIds(): Returns array of connected Client Ids.

int IpToId(string _ip): Converts a string ip to an int id
string IdTpId(int _id): Converts an int id to a string ip

bool GetClientConnected(int _clientId): Returns true if Client of id is connected.
int GetClientPacketRTT(int _clientId): Returns the specified Client's Packet RTT (Ping).
int GetClientSmoothPacketRTT(int _clientId): Returns the specified Client's Smoothed Packet RTT (Ping)(Average of last 5 RTTs).
