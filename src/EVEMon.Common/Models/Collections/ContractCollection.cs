using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Models.Collections
{
    public class ContractCollection : ReadonlyCollection<Contract>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal ContractCollection(CCPCharacter character)
        {
            m_character = character;
        }


        #region Importation/Exportation Methods

        /// <summary>
        /// Imports an enumeration of serialization objects.
        /// </summary>
        /// <param name="src"></param>
        internal void Import(IEnumerable<SerializableContract> src)
        {
            Items.Clear();
            foreach (SerializableContract srcContract in src)
            {
                Items.Add(new Contract(m_character, srcContract));
            }
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The enumeration of serializable contracts from the API.</param>
        /// <param name="endedContracts">The ended contracts.</param>
        internal void Import(IEnumerable<SerializableContractListItem> src,
            ICollection<Contract> endedContracts)
        {
            // Mark all contracts for deletion 
            // If they are found again on the API feed, they won't be deleted
            // and those set as ignored will be left as ignored
            foreach (Contract contract in Items)
                contract.MarkedForDeletion = true;
            // Import the contracts from the API, excluding the expired assigned ones
            var newContracts = new LinkedList<Contract>();
            DateTime now = DateTime.UtcNow;
            foreach (var contract in src)
            {
                var status = contract.Status;
                // For contracts issued to/by us, or finished, or outstanding and unexpired
                if (contract.IssuerID == m_character.CharacterID || status ==
                    CCPContractStatus.Completed.ToString() || status == CCPContractStatus.
                    CompletedByContractor.ToString() || status == CCPContractStatus.
                    CompletedByIssuer.ToString() || (status == CCPContractStatus.Outstanding.
                    ToString() && contract.DateExpired >= now) || contract.AcceptorID ==
                    m_character.CharacterID)
                {
                    // Exclude contracts which expired or were completed too long ago
                    var limit = contract.DateExpired.AddDays(Contract.MaxEndedDays);
                    if ((limit >= now || status == CCPContractStatus.Outstanding.ToString()) &&
                            !Items.Any(x => x.TryImport(contract, endedContracts)))
                        // Exclude contracts which matched an existing contract
                        newContracts.AddLast(new Contract(m_character, contract));
                }
            }
            // Add the new contracts that need attention to be notified to the user
            endedContracts.AddRange(newContracts.Where(newContract => newContract.NeedsAttention));
            // Add the items that are no longer marked for deletion
            newContracts.AddRange(Items.Where(x => !x.MarkedForDeletion));
            Items.Clear();
            Items.AddRange(newContracts);
        }

        /// <summary>
        /// Exports only the character issued contracts to a serialization object for the settings file.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Used to export only the corporation contracts issued by a character.</remarks>
        internal IEnumerable<SerializableContract> ExportOnlyIssuedByCharacter()
            => Items.Where(contract => contract.IssuerID == m_character.CharacterID).Select(contract => contract.Export());

        /// <summary>
        /// Exports the contracts to a serialization object for the settings file.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<SerializableContract> Export() => Items.Select(contract => contract.Export());

        #endregion
    }
}
