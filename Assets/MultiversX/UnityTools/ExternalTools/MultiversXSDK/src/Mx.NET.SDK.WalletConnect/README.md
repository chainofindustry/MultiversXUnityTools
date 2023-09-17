# Wallet Connect 2.0 documentation

### About SDK
The library is used to connect to xPortal wallet and sign transactions.

### How to install?
The content is delivered via nuget package:
##### RemarkableTools.Mx.WalletConnect [![Package](https://img.shields.io/nuget/v/RemarkableTools.Mx.WalletConnect)](https://www.nuget.org/packages/RemarkableTools.Mx.WalletConnect/)

---

### Quick start guide
##### Note: This guide does not include the NativeAuthClient Token for connection.

1. Variables init
```csharp
MultiversxProvider Provider = new(new MultiversxNetworkConfiguration(Network.DevNet));
NetworkConfig NetworkConfig { get; set; } = default!;
Account Account { get; set; } = default!;
```

1. Define the Metadata (is showing in xPortal when approving the connection) and NativeAuth Token
```csharp
var metadata = new Metadata()
{
    Name = "Mx.NET.WinForms",
    Description = "Mx.NET.WinForms login testing",
    Icons = new[] { "https://devnet.remarkable.tools/remarkabletools.ico" },
    Url = "https://devnet.remarkable.tools/"
};
```

2. Define the Wallet Connection
```csharp
const string CHAIN_ID = "D";
const string PROJECT_ID = "WALLET_CONNECT_PROJECT_ID";
IWalletConnect WalletConnect = new WalletConnect(metadata, PROJECT_ID, CHAIN_ID);
```

3. At start-up, you should check for old connection and setup the events
```csharp
await WalletConnect.ClientInit();
WalletConnect.OnSessionUpdateEvent += OnSessionUpdateEvent;
WalletConnect.OnSessionEvent += OnSessionEvent;
WalletConnect.OnSessionDeleteEvent += OnSessionDeleteEvent;
WalletConnect.OnSessionExpireEvent += OnSessionDeleteEvent;
WalletConnect.OnTopicUpdateEvent += OnTopicUpdateEvent;

try
{
    var isConnected = WalletConnect.TryReconnect();
    if (isConnected)
    {
        //initialize some variables
        NetworkConfig = await NetworkConfig.GetFromNetwork(Provider);
        Account = Account.From(await Provider.GetAccount(WalletConnect.Address));

        //do other operations
    }
}
catch (Exception ex)
{
    //Exception occured
}
```

4. Initialize WalletConnect connection
```csharp
await WalletConnect.Initialize();
```

5. Show a QR code generated with the link from `WalletConnect.URI`

6. Start the wallet connection
```csharp
try
{
    await WalletConnect.Connect(); 

    //initialize some variables
    NetworkConfig = await NetworkConfig.GetFromNetwork(Provider);
    Account = Account.From(await Provider.GetAccount(WalletConnect.Address));
}
catch (Exception ex)
{
    //Connection was not approved
}
```

7. Session Events
```csharp
private void OnSessionUpdateEvent(object? sender, GenericEvent<SessionUpdateEvent> @event)
{
    //Wallet connected
}

private void OnSessionEvent(object? sender, GenericEvent<SessionEvent> @event)
{
    //Session event
}

private void OnSessionDeleteEvent(object? sender, EventArgs e)
{
    //Wallet Disconnected
}

private void OnTopicUpdateEvent(object? sender, GenericEvent<TopicUpdateEvent> @event)
{
    //Topic Update
}
```

8. Disconnect function
```csharp
await WalletConnect.Disconnect(); //this will also trigger OnSessionDeleteEvent
```

9.1 Create a transaction then sign it with WalletConnect
```csharp
await Account.Sync(Provider); //always sync account first (to have the latest nonce)
var receiver = "RECEIVER_ADDRESS";

var transaction = EGLDTransactionRequest.EGLDTransfer(
                  NetworkConfig,
                  Account,
                  Address.FromBech32(receiver),
                  ESDTAmount.EGLD("1.5"),
                  $"hello");

try
{
    var transactionRequestDto = await WalletConnect.Sign(transaction);
    var response = await Provider.SendTransaction(transactionRequestDto);
    //Transaction was sent to network
}
catch (Exception ex)
{
    //Exception occured
}
```

9.2 Create multiple transactions the sign them with Wallet Connect
```csharp
await Account.Sync(Provider); //always sync account first (to have the latest nonce)
var receiver = "RECEIVER_ADDRESS";

var transaction1 = EGLDTransactionRequest.EGLDTransfer(
    NetworkConfig,
    Account,
    Address.FromBech32(receiver),
    ESDTAmount.EGLD("0.1"),
    "tx 1");
Account.IncrementNonce();

var transaction2 = EGLDTransactionRequest.EGLDTransfer(
    NetworkConfig,
    Account,
    Address.FromBech32(receiver),
    ESDTAmount.EGLD("0.2"),
    "tx 2");
Account.IncrementNonce();

var transaction3 = EGLDTransactionRequest.EGLDTransfer(
    NetworkConfig,
    Account,
    Address.FromBech32(receiver),
    ESDTAmount.EGLD("0.3"),
    "tx 3");
Account.IncrementNonce();

var transactions = new[] { transaction1, transaction2, transaction3 };
try
{
    var transactionsRequestDto = await WalletConnect.MultiSign(transactions);
    var response = await Provider.SendTransactions(transactionsRequestDto);
    //Transactions were sent to network
}
catch (Exception ex)
{
    //Exception occured
}
```

## Basic usage example
A Windows application example can be found [here](https://github.com/RemarkableTools/Mx.NET.SDK.WalletProviders/tree/dev/tests/WinFormsV2).