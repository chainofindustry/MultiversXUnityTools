using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            ElrondUnityTools.Manager.MakeSCQuery(scAddress.text, method.text, new string[] { param.text }, QueryComplete);
        }

        //linked to a button to execute the SC call 
        public void ExecuteCall()
        {
            //call the method from scAddress with param
            int nr = int.Parse(param.text);
            long gas = long.Parse(gasInput.text);
            ElrondUnityTools.Manager.CallSCMethod(scAddress.text, method.text, gas, CallStatus, nr);
        }

        /// <summary>
        /// Triggered when Smart contract query is complete
        /// </summary>
        /// <param name="operationStatus">Completed, In progress or Error</param>
        /// <param name="message">additional message</param>
        /// <param name="data">deserialized returned data</param>
        private void QueryComplete(ElrondUnityTools.OperationStatus operationStatus, string message, ElrondUnityTools.SCData data)
        {
            if (operationStatus == ElrondUnityTools.OperationStatus.Complete)
            {
                scResultText.text = "Raw data: \n" + Newtonsoft.Json.JsonConvert.SerializeObject(data);
                if (data.returnData.Length > 0)
                { 
                    //the returned data is an array(I do not know at this point how to create a SC that returns an Array of data instead of a single element)
                    string encodedText = data.returnData[0];

                    //convert the received data to bytes
                    byte[] bytes = Convert.FromBase64String(encodedText);

                    //convert the bytes array so hex
                    string hexString = Erdcsharp.Domain.Helper.Converter.ToHexString(bytes);

                    //convert the hex string to your data type(int, float, string, etc) 
                    //in this case the return data is an int
                    var result = Convert.ToInt64(hexString, 16);

                    scResultText.text += "\n\n Current sum: " + result;
                }
                else
                {
                    Debug.LogError("No data returned, check the call");
                }
            }
            else
            {
                Debug.LogError(message);
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
                ElrondUnityTools.Manager.CheckTransactionStatus(txHash, SCTransactionListener, 1);
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
                if (message == "pending")
                {
                    ElrondUnityTools.Manager.CheckTransactionStatus(txHash, SCTransactionListener, 1);
                }
                else
                {
                    if (message == "success")
                    {
                        //do something 
                    }
                }
            }
        }
    }
}
