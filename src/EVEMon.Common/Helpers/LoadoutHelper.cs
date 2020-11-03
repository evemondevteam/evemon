using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Interfaces;
using EVEMon.Common.Serialization.FittingClf;
using EVEMon.Common.Serialization.FittingXml;

namespace EVEMon.Common.Helpers
{
    public static class LoadoutHelper
    {
        /// <summary>
        /// Gets the ordered slot names.
        /// </summary>
        /// <value>
        /// The ordered slot names.
        /// </value>
        public static string[] OrderedSlotNames => new[]
        {
            "High Slots", "Med Slots", "Low Slots",
            "Rig Slots", "Subsystem Slots", "Ammunition & Charges",
            "Drones", "Unknown"
        };

        /// <summary>
        /// Determines whether the specified text is a loadout.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static bool IsLoadout(string text, out LoadoutFormat format)
        {
            if (IsEFTFormat(text))
            {
                format = LoadoutFormat.EFT;
                return true;
            }

            if (IsXMLFormat(text))
            {
                format = LoadoutFormat.XML;
                return true;
            }

            if (IsDNAFormat(text))
            {
                format = LoadoutFormat.DNA;
                return true;
            }

            if (IsCLFFormat(text))
            {
                format = LoadoutFormat.CLF;
                return true;
            }

            format = LoadoutFormat.None;
            return false;
        }

        /// <summary>
        /// Determines whether the loadout is in EFT format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the loadout is in EFT format; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsEFTFormat(string text)
        {
            string[] lines = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Nothing to evaluate
            if (lines.Length == 0)
                return false;

            // Error on first line ?
            string line = lines.First();
            if (string.IsNullOrEmpty(line) || !line.StartsWith("[", StringComparison.CurrentCulture) || !line.Contains(","))
                return false;

            // Retrieve the ship
            int commaIndex = line.IndexOf(',');
            string shipTypeName = line.Substring(1, commaIndex - 1);

            return StaticItems.ShipsMarketGroup.AllItems.Any(x => x.Name == shipTypeName);
        }

        /// <summary>
        /// Determines whether the loadout is in XML format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the loadout is in XML format; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsXMLFormat(string text)
        {
            XmlRootAttribute xmlRoot = new SerializableXmlFittings().GetType().GetCustomAttributes(
                typeof(XmlRootAttribute), false).Cast<XmlRootAttribute>().FirstOrDefault();

            if (xmlRoot == null)
                return false;

            using (TextReader reader = new StringReader(text))
            {
                if (Util.GetXmlRootElement(reader) != xmlRoot.ElementName)
                    return false;
            }

            SerializableXmlFittings fittings = Util.DeserializeXmlFromString<SerializableXmlFittings>(text);
            return StaticItems.ShipsMarketGroup.AllItems.Any(x => x.Name == fittings.Fitting.ShipType.Name);
        }

        /// <summary>
        /// Determines whether the loadout is in DNA format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the loadout is in DNA format; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsDNAFormat(string text)
        {
            string[] lines = text.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Nothing to evaluate
            if (lines.Length == 0)
                return false;

            // Error on first line ?
            int shipID;
            string line = lines.First();
            if (string.IsNullOrEmpty(line) || !line.TryParseInv(out shipID))
                return false;

            return StaticItems.ShipsMarketGroup.AllItems.Any(x => x.ID == shipID);
        }

        /// <summary>
        /// Determines whether whether the loadout is in CLF format.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the loadout is in CLF format; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsCLFFormat(string text)
            => text.Length != 0 && text.StartsWith("{\"clf-version\":", StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Deserializes an EFT loadout text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">text</exception>
        public static ILoadoutInfo DeserializeEftFormat(string text)
        {
            text.ThrowIfNull(nameof(text));

            string[] lines = text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            ILoadoutInfo loadoutInfo = new LoadoutInfo();

            // Nothing to evaluate
            if (lines.Length == 0)
                return loadoutInfo;

            var listOfItems = new List<Item>();
            Loadout loadout = null;

            foreach (string line in lines.Where(line => !string.IsNullOrEmpty(line) &&
                !line.Contains("empty") && !line.Contains("slot")))
            {
                // Retrieve the ship
                if (line == lines.First())
                {
                    // Retrieve the loadout name
                    int commaIndex = line.IndexOf(',');
                    loadoutInfo.Ship = StaticItems.GetItemByName(line.Substring(1,
                        commaIndex - 1));
                    if (loadoutInfo.Ship == null)
                        return loadoutInfo;
                    loadout = new Loadout(line.Substring(commaIndex + 1, line.Length -
                        commaIndex - 2).Trim(), string.Empty);
                    continue;
                }

                // Retrieve the item (might be a drone)
                int lastX = line.LastIndexOf(" x", StringComparison.CurrentCulture);
                int lastComma = line.LastIndexOf(',');
                string itemName = lastComma >= 0 ? line.Substring(0, lastComma) : (lastX >= 0 ?
                    line.Substring(0, lastX) : line);

                int quantity = lastX >= 0 ? int.Parse(line.Substring(lastX + 2, line.Length -
                    (lastX + 2))) : 1;

                Item item = StaticItems.GetItemByName(itemName) ?? Item.UnknownItem;

                for (int i = 0; i < Math.Min(quantity, 100); i++)
                {
                    listOfItems.Add(item);
                }

                // Retrieve the charge
                string chargeName = lastComma >= 0 ? line.Substring(lastComma + 1).Trim() :
                    null;

                if (string.IsNullOrEmpty(chargeName))
                    continue;

                Item charge = StaticItems.GetItemByName(chargeName) ?? Item.UnknownItem;

                listOfItems.Add(charge);
            }

            if (loadout == null)
                return loadoutInfo;

            loadout.Items = listOfItems;
            loadoutInfo.Loadouts.Add(loadout);

            return loadoutInfo;
        }

        /// <summary>
        /// Deserializes an XML loadout text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static ILoadoutInfo DeserializeXmlFormat(string text)
        {
            SerializableXmlFittings fittings = Util.DeserializeXmlFromString<SerializableXmlFittings>(text);

            ILoadoutInfo loadoutInfo = new LoadoutInfo();

            // Nothing to evaluate
            if (fittings == null)
                return loadoutInfo;

            // Retrieve the ship
            loadoutInfo.Ship = StaticItems.GetItemByName(fittings.Fitting.ShipType.Name);

            if (loadoutInfo.Ship == null)
                return loadoutInfo;

            // Special case to avoid displaying gzCLF block from Osmium
            if (fittings.Fitting.Description.Text.StartsWith("BEGIN gzCLF BLOCK", StringComparison.InvariantCultureIgnoreCase))
                fittings.Fitting.Description.Text = string.Empty;

            Loadout loadout = new Loadout(fittings.Fitting.Name, fittings.Fitting.Description.Text);

            IEnumerable<Item> listOfItems = fittings.Fitting.FittingHardware
                .Where(hardware => hardware != null && hardware.Item != null && hardware.Slot != "drone bay")
                .Select(hardware => hardware.Item);

            IEnumerable<SerializableXmlFittingHardware> listOfXmlDrones = fittings.Fitting.FittingHardware
                .Where(hardware => hardware != null &&
                                   hardware.Item != null &&
                                   hardware.Slot == "drone bay");

            var listOfDrones = new List<Item>();
            foreach (SerializableXmlFittingHardware drone in listOfXmlDrones)
            {
                for (int i = 0; i < drone.Quantity; i++)
                {
                    listOfDrones.Add(drone.Item);
                }
            }

            loadout.Items = listOfItems.Concat(listOfDrones);
            loadoutInfo.Loadouts.Add(loadout);

            return loadoutInfo;
        }

        /// <summary>
        /// Deserializes a DNA loadout text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static ILoadoutInfo DeserializeDnaFormat(string text)
        {
            string[] lines = text.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            ILoadoutInfo loadoutInfo = new LoadoutInfo();

            // Nothing to evaluate
            if (lines.Length == 0)
                return loadoutInfo;

            var listOfItems = new List<Item>();
            Loadout loadout = null;

            foreach (string line in lines.Where(line => !string.IsNullOrEmpty(line)))
            {
                // Retrieve the ship
                if (line == lines.First())
                {
                    int shipID;
                    if (line.TryParseInv(out shipID))
                    {
                        loadoutInfo.Ship = StaticItems.GetItemByID(shipID);
                        if (loadoutInfo.Ship == null)
                            return loadoutInfo;
                        loadout = new Loadout(loadoutInfo.Ship.Name, string.Empty);
                        continue;
                    }
                }

                // Retrieve the item
                int itemID;
                Item item = line.Substring(0, line.LastIndexOf(';')).TryParseInv(out itemID) ?
                    (StaticItems.GetItemByID(itemID) ?? Item.UnknownItem) : Item.UnknownItem;

                // Retrieve the quantity
                int quantity;
                line.Substring(line.LastIndexOf(';') + 1).TryParseInv(out quantity);

                // Trim excess ammo & charges, no need to display more than the max number of modules
                if (item.MarketGroup.BelongsIn(DBConstants.AmmosAndChargesMarketGroupID) && quantity > 8)
                    quantity = 1;

                for (int i = 0; i < quantity; i++)
                {
                    listOfItems.Add(item);
                }
            }

            if (loadout == null)
                return loadoutInfo;

            loadout.Items = listOfItems;
            loadoutInfo.Loadouts.Add(loadout);

            return loadoutInfo;
        }

        /// <summary>
        /// Deserializes a CLF loadout text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static ILoadoutInfo DeserializeClfFormat(string text)
        {
            ILoadoutInfo loadoutInfo = new LoadoutInfo();

            // Nothing to evaluate
            if (text.Length == 0)
                return loadoutInfo;

            SerializableClfFitting clfFitting = Util.DeserializeJson<SerializableClfFitting>(text);

            // Nothing to evaluate
            if (clfFitting == null)
                return loadoutInfo;

            // Retrieve the ship
            loadoutInfo.Ship = clfFitting.Ship.Item;

            if (loadoutInfo.Ship == null)
                return loadoutInfo;

            Loadout loadout = new Loadout(clfFitting.MetaData.Title, clfFitting.MetaData.Description);

            IEnumerable<Item> listOfItems = clfFitting.Presets.SelectMany(x => x.Modules)
                .Where(module => module != null && module.Item != null)
                .Select(module => module.Item);

            IEnumerable<Item> listOfCharges = clfFitting.Presets.SelectMany(x => x.Modules)
                .SelectMany(module => module.Charges)
                .Where(module => module != null && module.Item != null)
                .Select(module => module.Item);

            IEnumerable<SerializableClfFittingDroneType> listOfClfDrones = clfFitting.Drones.SelectMany(x => x.InBay)
                .Concat(clfFitting.Drones.SelectMany(x => x.InSpace))
                .Where(drone => drone != null && drone.Item != null)
                .Select(drone => drone);

            var listOfDrones = new List<Item>();
            foreach (SerializableClfFittingDroneType clfDrone in listOfClfDrones)
            {
                for (int i = 0; i < clfDrone.Quantity; i++)
                {
                    listOfDrones.Add(clfDrone.Item);
                }
            }

            loadout.Items = listOfItems.Concat(listOfCharges).Concat(listOfDrones);
            loadoutInfo.Loadouts.Add(loadout);

            return loadoutInfo;
        }

        /// <summary>
        /// Gets the slot by item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static string GetSlotByItem(Item item)
        {
            switch (item.FittingSlot)
            {
                // High Slot
                case ItemSlot.High:
                    return OrderedSlotNames[0];
                // Medium Slot
                case ItemSlot.Medium:
                    return OrderedSlotNames[1];
                // Low Slot
                case ItemSlot.Low:
                    return OrderedSlotNames[2];
            }

            // Rig Slot
            if (item.MarketGroup.BelongsIn(DBConstants.ShipModificationsMarketGroupID))
                return OrderedSlotNames[3];

            // Subsystems
            if (item.MarketGroup.BelongsIn(DBConstants.SubsystemsMarketGroupID))
                return OrderedSlotNames[4];

            // Ammunition & Charges
            if (item.MarketGroup.BelongsIn(DBConstants.AmmosAndChargesMarketGroupID))
                return OrderedSlotNames[5];

            // Drones
            if (item.MarketGroup.BelongsIn(DBConstants.DronesMarketGroupID))
                return OrderedSlotNames[6];

            return OrderedSlotNames[7];
        }
    }
}
