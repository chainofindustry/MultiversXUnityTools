using UnityEngine.UI;
using System.Numerics;
using Mx.NET.SDK.Core.Domain.Values;
using Mx.NET.SDK.Domain.Data.Transaction;

namespace MultiversX.UnityTools.Examples
{
    public class SCScreen : GenericUIScreen
    {
        public InputField scAddress;
        public InputField method;
        public InputField param;
        public InputField gasInput;
        public Text scResultText;

        private string txHash;
        private string defaultScAddress = "erd1qqqqqqqqqqqqqpgqmm2m825y2t9nya0yqeg3nqlh2q50e7pd0eqq98uw2e";
        private string defaultFuncName = "add";
        private string defaultGas = "1500000";
        private int valueToAdd = 10;


        public override void Init(params object[] args)
        {
            //set default values inside input fields
            base.Init(args);
            scAddress.text = defaultScAddress;
            method.text = defaultFuncName;
            gasInput.text = defaultGas;
            param.text = valueToAdd.ToString();
        }


        //linked to the back button
        public void SCBack()
        {
            DemoScript.Instance.LoadScreen(Screens.Connected);
        }


        //linked to execute SC query button
        public void ExecuteQuery()
        {
            //call the method from scAddress with parameters
            API.MakeSCQuery<NumericValue>(scAddress.text, method.text, QueryComplete, TypeValue.BigUintTypeValue);
        }


        //linked to a button to execute the SC call 
        public void ExecuteCall()
        {
            //call the method from scAddress with param
            BigInteger nr = int.Parse(param.text);
            long gas = long.Parse(gasInput.text);
            API.CallSCMethod(scAddress.text, method.text, gas, CallStatus, NumericValue.BigIntValue(nr));
        }


        /// <summary>
        /// Triggered when Smart contract query is complete
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        /// <param name="data">deserialized returned data</param>
        private void QueryComplete(CompleteCallback<NumericValue> result)
        {
            if (result.status == OperationStatus.Success)
            {
                scResultText.text = "Current sum: " + result.data;
            }
            else
            {
                scResultText.text = result.errorMessage;
            }
        }


        /// <summary>
        /// Listener for the call response
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        private void CallStatus(CompleteCallback<string[]> result)
        {

            if (result.status == OperationStatus.Success)
            {
                scResultText.text = $"Pending TX: {result.data[0]}";
                API.CheckTransactionStatus(result.data, SCTransactionListener, 1);
            }
            else
            {
                //do something
                scResultText.text = $"Transaction status: {result.status}. Message: {result.errorMessage}";
            }
        }


        /// <summary>
        /// Listener for the SC transaction status 
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        private void SCTransactionListener(CompleteCallback<Transaction[]> result)
        {
            scResultText.text = "";
            if (result.status == OperationStatus.Error)
            {
                scResultText.text =  result.errorMessage;
            }

            for (int i = 0; i < result.data.Length; i++)
            {
                scResultText.text += $"\nTx: {result.data[i].TxHash} : {result.data[i].Status} {result.data[i].GetLogs()}";
            }

            if (result.status == OperationStatus.Success)
            {
                API.RefreshAccount();
            }
        }
    }
}
