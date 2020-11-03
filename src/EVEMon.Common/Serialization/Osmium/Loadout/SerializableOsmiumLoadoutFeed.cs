using System;
using EVEMon.Common.Constants;

namespace EVEMon.Common.Serialization.Osmium.Loadout
{
    public sealed class SerializableOsmiumLoadoutFeed
    {
        public long ID => long.Parse(Uri.Replace($"{NetworkConstants.OsmiumBaseUrl}/loadout/", string.Empty),
            CultureConstants.InvariantCulture);

        public string Uri { get; set; }

        public string Name { get; set; }

        public int ShipTypeID { get; set; }

        public string ShipTypeName { get; set; }

        public SerializableOsmiumLoadoutAuthor Author { get; set; }

        public long CreationDate { get; set; }

        public string RawDescription { get; set; }

        public int UpVotes { get; set; }

        public int DownVotes { get; set; }

        public int Rating => UpVotes - DownVotes;
    }
}
