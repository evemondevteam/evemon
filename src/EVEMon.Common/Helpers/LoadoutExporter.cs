using EVEMon.Common.Data;
using EVEMon.Common.Extensions;
using EVEMon.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EVEMon.Common.Helpers
{
    public static class LoadoutExporter
    {
        private static readonly string[] EMPTY_SLOTS =
        {
            "[empty high slot]", "[empty med slot]", "[empty low slot]", "[empty rig slot]", ""
        };
        private static readonly Regex NOT_DIGITS = new Regex(@"[^\d]");
        private static readonly int[] SLOT_ORDER = { 2, 1, 0, 3, 4 };

        /// <summary>
        /// Exports to clipboard.
        /// </summary>
        /// <param name="loadoutInfo">The loadout information.</param>
        /// <param name="loadout">The loadout.</param>
        public static void ExportToClipboard(ILoadoutInfo loadoutInfo, Loadout loadout)
        {
            Dictionary<string, List<string>> items = GetItemsBySlots(loadout.Items);
            ExtractProperties(loadoutInfo, items);
            string exportText = SerializeToEFTFormat(loadoutInfo, loadout, items);

            // Copy to clipboard
            try
            {
                Clipboard.Clear();
                Clipboard.SetText(exportText);
            }
            catch (ExternalException ex)
            {
                // There is a bug that results in an exception being
                // thrown when the clipboard is in use by another process
                ExceptionHandler.LogException(ex, true);
            }
        }

        /// <summary>
        /// Gets the items by slots.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        private static Dictionary<string, List<string>> GetItemsBySlots(IEnumerable<Item> items)
        {
            // AllGroups items by slots
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();

            foreach (IGrouping<string, Item> slotItems in items.GroupBy(LoadoutHelper.GetSlotByItem))
            {
                dictionary[slotItems.Key] = new List<string>();
                foreach (Item item in slotItems)
                {
                    dictionary[slotItems.Key].Add(item.Name);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Extracts the properties.
        /// </summary>
        /// <param name="loadoutInfo">The loadout information.</param>
        /// <param name="items">The items.</param>
        private static void ExtractProperties(ILoadoutInfo loadoutInfo, IDictionary<string, List<string>> items)
        {
            // Add "empty slot" mentions for every slot type
            foreach (EvePropertyValue prop in loadoutInfo.Ship.Properties.Where(prop => prop.Property != null))
            {
                for (int i = 0; i < 5; i++)
                {
                    string slotName = LoadoutHelper.OrderedSlotNames[i], empty;
                    // Fill slots by type
                    if (prop.Property.Name.Contains(slotName))
                    {
                        int slots;
                        NOT_DIGITS.Replace(prop.Value, string.Empty).TryParseInv(out slots);
                        empty = EMPTY_SLOTS[i];
                        while (items.ContainsKey(slotName) && items[slotName].Count < slots)
                            items[slotName].Add(empty);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Serializes to EFT format.
        /// </summary>
        /// <param name="loadoutInfo">The loadout information.</param>
        /// <param name="loadout">The loadout.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        private static string SerializeToEFTFormat(ILoadoutInfo loadoutInfo, Loadout loadout,
            IDictionary<string, List<string>> items)
        {
            string name;
            // Build the output format for EFT
            StringBuilder exportText = new StringBuilder();
            exportText.AppendLine($"[{loadoutInfo.Ship.Name}, {loadout.Name} (EVEMon)]");

            // Slots in order: Low, Medium, High, Rig, Subsystem
            foreach (int index in SLOT_ORDER)
            {
                name = LoadoutHelper.OrderedSlotNames[index];
                if (items.ContainsKey(name))
                {
                    // Same function as appending the joined string, but faster
                    foreach (string slotItem in items[name])
                        exportText.AppendLine(slotItem);
                    exportText.AppendLine();
                }
            }

            // Drones need quantity
            name = LoadoutHelper.OrderedSlotNames[6];
            if (items.ContainsKey(name))
                foreach (var itemName in items[name].GroupBy(itemName => itemName))
                    exportText.AppendLine($"{itemName.Key} x{itemName.Count()}");

            return exportText.ToString();
        }
    }
}

