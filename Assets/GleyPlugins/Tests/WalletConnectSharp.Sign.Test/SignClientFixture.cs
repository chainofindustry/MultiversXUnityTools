using WalletConnectSharp.Sign;
using WalletConnectSharp.Sign.Test;

public class SignClientFixture : TwoClientsFixture<WalletConnectSignClient>
{
    protected override async void Init()
    {
        ClientA = await WalletConnectSignClient.Init(OptionsA);
        ClientB = await WalletConnectSignClient.Init(OptionsB);
    }
}
