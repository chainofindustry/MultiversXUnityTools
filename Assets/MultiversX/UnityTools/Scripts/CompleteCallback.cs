namespace MultiversX.UnityTools
{
    public class CompleteCallback<T>
    {
        public OperationStatus status;
        public string errorMessage;
        public T data;

        public CompleteCallback(OperationStatus status, string errorMessage, T data)
        { 
            this.status = status;
            this.errorMessage = errorMessage;
            this.data = data;
        }
    }
}