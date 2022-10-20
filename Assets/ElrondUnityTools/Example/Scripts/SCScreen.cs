using System;
using UnityEngine;
using UnityEngine.UI;
using System.Numerics;
using Erdcsharp.Domain.Values;

namespace ElrondUnityExamples
{
    public class SCScreen : GenericUIScreen
    {
        public InputField scAddress;
        public InputField method;
        public InputField param;
        public InputField gasInput;
        public Text scResultText;
        private string txHash;
        string defaultScAddress = "erd1qqqqqqqqqqqqqpgqmm2m825y2t9nya0yqeg3nqlh2q50e7pd0eqq98uw2e";
        string defaultFuncName = "add";
        int valueToAdd = 10;
        string defaultGas = "1500000";


        public override void Init(params object[] args)
        {
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
            ElrondUnityTools.Manager.MakeSCQuery<NumericValue>(scAddress.text, method.text, QueryComplete, TypeValue.BigUintTypeValue);
        }

        //linked to a button to execute the SC call 
        public void ExecuteCall()
        {
            //call the method from scAddress with param
            BigInteger nr = int.Parse(param.text);
            long gas = long.Parse(gasInput.text);
            ElrondUnityTools.Manager.CallSCMethod(scAddress.text, method.text, gas, CallStatus, NumericValue.BigIntValue(nr));
        }

        /// <summary>
        /// Triggered when Smart contract query is complete
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        /// <param name="data">deserialized returned data</param>
        private void QueryComplete(ElrondUnityTools.OperationStatus operationStatus, string message, NumericValue data)
        {
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                scResultText.text = "Current sum: " + data;
            }
            else
            {
                scResultText.text = message;
            }
        }


        /// <summary>
        /// Listener for the call response
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        private void CallStatus(ElrondUnityTools.OperationStatus operationStatus, string message)
        {
            scResultText.text = operationStatus + " " + message;
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                txHash = message;
                Debug.Log("Tx Hash: " + txHash);
                ElrondUnityTools.Manager.CheckTransactionStatus(txHash, SCTransactionListener);
            }
            if (operationStatus == ElrondUnityTools.OperationStatus.Error)
            {
                //do something
            }
        }

        /// <summary>
        /// Listener for the SC transaction status 
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        private void SCTransactionListener(ElrondUnityTools.OperationStatus operationStatus, string message)
        {
            scResultText.text = operationStatus + " " + message;
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                ElrondUnityTools.Manager.RefreshAccount();
            }
        }
    }
}
