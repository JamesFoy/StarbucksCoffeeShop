using System;
using Object = UnityEngine.Object;

namespace AssetUsageDetectorNamespace
{
    [Serializable]
    public class SubAssetToSearch
    {
        public Object subAsset;
        public bool shouldSearch;

        public SubAssetToSearch( Object subAsset, bool shouldSearch )
        {
            this.subAsset = subAsset;
            this.shouldSearch = shouldSearch;
        }
    }
}