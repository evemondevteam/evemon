using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Extensions;
using EVEMon.Common.Service;
using YamlDotNet.RepresentationModel;

namespace EVEMon.Common.Models.Extended
{
    /// <summary>
    /// A class for the internal parser of the EVE notification text.
    /// </summary>
    /// <remarks>
    /// This class is called when the fetching of the external parser fails for any reason.
    /// </remarks>
    internal class InternalEveNotificationTextParser : EveNotificationTextParser
    {
        /// <summary>
        /// Parses the notification text.
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="pair">The pair.</param>
        /// <param name="parsedDict">The parsed dictionary.</param>
        public override void Parse(EveNotification notification, KeyValuePair<YamlNode, YamlNode> pair,
            IDictionary<string, string> parsedDict)
        {
            string key = pair.Key.ToString(), value = pair.Value.ToString();
            switch (key.ToUpperInvariant())
            {
            case "CHARID":
            case "SENDERCHARID":
            case "RECEIVERCHARID":
            case "OWNERID":
            case "LOCATIONOWNERID":
            case "DESTROYERID":
            case "INVOKINGCHARID":
            case "PODKILLERID":
            case "NEWCEOID":
            case "OLDCEOID":
            case "CORPID":
            case "VICTIMID":
            case "DECLAREDBYID":
            case "AGAINSTID":
                parsedDict[key] = EveIDToName.GetIDToName(long.Parse(value));
                break;
            case "CLONESTATIONID":
            case "CORPSTATIONID":
            case "LOCATIONID":
                parsedDict[key] = EveIDToStation.GetIDToStation(long.Parse(value))?.Name ??
                    EveMonConstants.UnknownText;
                break;
            case "SOLARSYSTEMID":
                parsedDict[key] = StaticGeography.GetSolarSystemName(int.Parse(value));
                break;
            case "SHIPTYPEID":
            case "TYPEID":
            case "STRUCTURETYPEID":
            case "VICTIMSHIPTYPEID":
                parsedDict[key] = StaticItems.GetItemName(int.Parse(value));
                break;
            case "MEDALID":
                var medal = notification.CCPCharacter.CharacterMedals.FirstOrDefault(x =>
                    (x.ID.ToString() == value));

                parsedDict[key] = medal?.Title ?? EveMonConstants.UnknownText;
                parsedDict.Add("medalDescription", medal?.Description ??
                    EveMonConstants.UnknownText);
                break;
            case "ENDDATE":
            case "STARTDATE":
            case "DECLOAKTIME":
                parsedDict[key] = string.Format(CultureConstants.InvariantCulture,
                    "{0:dddd, MMMM d, yyyy HH:mm} (EVE Time)", long.Parse(value).
                    WinTimeStampToDateTime());
                break;
            case "NOTIFICATION_CREATED":
                parsedDict[key] = string.Format(CultureConstants.InvariantCulture,
                    "{0:dddd, MMMM d, yyyy} (EVE Time)", long.Parse(value).
                    WinTimeStampToDateTime());
                break;
            case "CAMPAIGNEVENTTYPE":
                switch (value)
                {
                case "1":
                    parsedDict[key] = "Territorial Claim Unit";
                    break;
                case "2":
                    parsedDict[key] = "Infrastructure Hub";
                    break;
                case "3":
                    parsedDict[key] = "Station";
                    break;
                default:
                    parsedDict[key] = EveMonConstants.UnknownText;
                    break;
                }
                break;
            case "TYPEIDS":
                YamlSequenceNode typeIDs = pair.Value as YamlSequenceNode;
                if (typeIDs == null)
                    break;
                switch (notification.TypeID)
                {
                    case 56:
                    case 57:
                    {
                        if (!typeIDs.Any())
                            parsedDict[key] = "None were in the clone";
                        else
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (var typeID in typeIDs)
                            {
                                int type = 0;
                                int.TryParse(typeID.ToString(), out type);
                                sb.AppendLine().AppendLine($"Type: {StaticItems.GetItemName(type)}");
                            }
                            parsedDict[key] = sb.ToString();
                        }
                    }
                        break;
                }
                break;
            case "ISHOUSEWARMINGGIFT":
                if (Convert.ToBoolean(pair.Value) && notification.TypeID == 34)
                    // Tritanium
                    parsedDict[key] = StaticItems.GetItemName(notification.TypeID);
                break;
            case "LEVEL":
                parsedDict[key] = $"{Standing.Status(double.Parse(value))} Standing";
                break;
            }
        }
    }
}
