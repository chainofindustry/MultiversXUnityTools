using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenMetadata
{
    public string identifier { get; set; }
    public string name { get; set; }
    public string ticker { get; set; }
    public string owner { get; set; }
    public int decimals { get; set; }
    public bool isPaused { get; set; }
    public int transactions { get; set; }
    public int accounts { get; set; }
    public bool canUpgrade { get; set; }
    public bool canMint { get; set; }
    public bool canBurn { get; set; }
    public bool canChangeOwner { get; set; }
    public bool canPause { get; set; }
    public bool canFreeze { get; set; }
    public bool canWipe { get; set; }
    public string balance { get; set; }
    public Assets assets { get; set; }
    public float price { get; set; }
    public float marketCap { get; set; }
    public string supply { get; set; }
    public string circulatingSupply { get; set; }
    public float valueUsd { get; set; }


    public class Assets
    {
        public string website { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string pngUrl { get; set; }
        public string svgUrl { get; set; }
        public string ledgerSignature { get; set; }
        public Social social { get; set; }
    }

    public class Social
    {
        public string blog { get; set; }
        public string twitter { get; set; }
        public string whitepaper { get; set; }
        public string coinmarketcap { get; set; }
        public string coingecko { get; set; }
    }
}

