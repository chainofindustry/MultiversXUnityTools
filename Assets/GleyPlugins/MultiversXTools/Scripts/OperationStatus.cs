namespace MultiversXUnityTools
{
    /// <summary>
    /// Each callback can be in one of these states
    /// </summary>
    public enum OperationStatus
    {
        InProgress,//still processing
        Error,//is done but with error
        Success//is done successfully 
    }
}
