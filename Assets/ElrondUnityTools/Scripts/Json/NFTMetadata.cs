namespace ElrondUnityTools
{
    //automatically generated from response JSON
    public class NFTMetadata
    {
        public string identifier { get; set; }
        public string collection { get; set; }
        public string attributes { get; set; }
        public ulong nonce { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string creator { get; set; }
        public float royalties { get; set; }
        public string[] uris { get; set; }
        public string url { get; set; }
        public Medium[] media { get; set; }
        public bool isWhitelistedStorage { get; set; }
        public string[] tags { get; set; }
        public Metadata metadata { get; set; }
        public string ticker { get; set; }
        public object score { get; set; }
        public int rank { get; set; }
        public string balance { get; set; }
        public bool isNsfw { get; set; }
        public Assets assets { get; set; }
        public int decimals { get; set; }
        public float price { get; set; }
        public float valueUsd { get; set; }
        public Scaminfo scamInfo { get; set; }
        public Unlockschedule[] unlockSchedule { get; set; }
    }

    public class Metadata
    {
        public object id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public Attribute[] attributes { get; set; }
        public object image { get; set; }
        public string dna { get; set; }
        public long date { get; set; }
        public string[] tags { get; set; }
        public string edition { get; set; }
        public string fileName { get; set; }
        public string fileType { get; set; }
        public Rarityinfo rarityinfo { get; set; }
        public string compiler { get; set; }
        public Rarity rarity { get; set; }
    }

    public class Rarityinfo
    {
        public float averageRarity { get; set; }
        public int propertyCount { get; set; }
        public int collectionSize { get; set; }
    }

    public class Rarity
    {
        public float avgRarity { get; set; }
        public float statRarity { get; set; }
        public float rarityScore { get; set; }
        public float rarityScoreNormed { get; set; }
        public int usedTraitsCount { get; set; }
    }

    public class Attribute
    {
        public string value { get; set; }
        public string trait_type { get; set; }
        public int attributeRarity { get; set; }
        public int attributeOccurance { get; set; }
    }

    public class Assets
    {
        public string website { get; set; }
        public string description { get; set; }
        public Social social { get; set; }
        public string status { get; set; }
        public string pngUrl { get; set; }
        public string svgUrl { get; set; }
    }

    public class Social
    {
        public string blog { get; set; }
        public string twitter { get; set; }
        public string whitepaper { get; set; }
        public string telegram { get; set; }
        public string discord { get; set; }
        public string email { get; set; }
    }

    public class Scaminfo
    {
        public string type { get; set; }
        public string info { get; set; }
    }

    public class Medium
    {
        public string url { get; set; }
        public string originalUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string fileType { get; set; }
        public int fileSize { get; set; }
    }

    public class Unlockschedule
    {
        public int remainingEpochs { get; set; }
        public float percent { get; set; }
    }
}
