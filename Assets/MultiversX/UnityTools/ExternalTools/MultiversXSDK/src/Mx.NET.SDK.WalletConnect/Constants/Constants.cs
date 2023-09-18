namespace Mx.NET.SDK.WalletConnect.Constants
{
    public class Operations
    {
        public const string SIGN_TRANSACTION = "mvx_signTransaction";
        public const string SIGN_TRANSACTIONS = "mvx_signTransactions";
        public const string SIGN_MESSAGE = "mvx_signMessage";
        public const string SIGN_LOGIN_TOKEN = "mvx_signLoginToken";
        public const string SIGN_NATIVE_AUTH_TOKEN = "mvx_signNativeAuthToken";
        public const string CANCEL_ACTION = "mvx_cancelAction";
    }

    public static class Events
    {
        public const string SESSION_UPDATE = "session_update";
        public const string SESSION_EVENT = "session_event";
        public const string SESSION_DELETE = "session_delete";
        public const string SESSION_EXPIRE = "session_expire";
        public const string PAIRING_DELETE = "pairing_delete";
        public const string PAIRING_EXPIRE = "pairing_expire";
    }
}
