/* 
 * EVE Swagger Interface
 *
 * An OpenAPI for EVE Online
 *
 * OpenAPI spec version: 0.5.5
 * 
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using SwaggerDateConverter = IO.Swagger.Client.SwaggerDateConverter;

namespace IO.Swagger.Model
{
    /// <summary>
    /// 200 ok object
    /// </summary>
    [DataContract]
    public partial class GetAlliancesAllianceIdOk :  IEquatable<GetAlliancesAllianceIdOk>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAlliancesAllianceIdOk" /> class.
        /// </summary>
        [JsonConstructorAttribute]
        protected GetAlliancesAllianceIdOk() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAlliancesAllianceIdOk" /> class.
        /// </summary>
        /// <param name="AllianceName">the full name of the alliance (required).</param>
        /// <param name="DateFounded">date_founded string (required).</param>
        /// <param name="ExecutorCorp">the executor corporation ID, if this alliance is not closed.</param>
        /// <param name="Ticker">the short name of the alliance (required).</param>
        public GetAlliancesAllianceIdOk(string AllianceName = default(string), DateTime? DateFounded = default(DateTime?), int? ExecutorCorp = default(int?), string Ticker = default(string))
        {
            // to ensure "AllianceName" is required (not null)
            if (AllianceName == null)
            {
                throw new InvalidDataException("AllianceName is a required property for GetAlliancesAllianceIdOk and cannot be null");
            }
            else
            {
                this.AllianceName = AllianceName;
            }
            // to ensure "DateFounded" is required (not null)
            if (DateFounded == null)
            {
                throw new InvalidDataException("DateFounded is a required property for GetAlliancesAllianceIdOk and cannot be null");
            }
            else
            {
                this.DateFounded = DateFounded;
            }
            // to ensure "Ticker" is required (not null)
            if (Ticker == null)
            {
                throw new InvalidDataException("Ticker is a required property for GetAlliancesAllianceIdOk and cannot be null");
            }
            else
            {
                this.Ticker = Ticker;
            }
            this.ExecutorCorp = ExecutorCorp;
        }
        
        /// <summary>
        /// the full name of the alliance
        /// </summary>
        /// <value>the full name of the alliance</value>
        [DataMember(Name="alliance_name", EmitDefaultValue=false)]
        public string AllianceName { get; set; }

        /// <summary>
        /// date_founded string
        /// </summary>
        /// <value>date_founded string</value>
        [DataMember(Name="date_founded", EmitDefaultValue=false)]
        public DateTime? DateFounded { get; set; }

        /// <summary>
        /// the executor corporation ID, if this alliance is not closed
        /// </summary>
        /// <value>the executor corporation ID, if this alliance is not closed</value>
        [DataMember(Name="executor_corp", EmitDefaultValue=false)]
        public int? ExecutorCorp { get; set; }

        /// <summary>
        /// the short name of the alliance
        /// </summary>
        /// <value>the short name of the alliance</value>
        [DataMember(Name="ticker", EmitDefaultValue=false)]
        public string Ticker { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class GetAlliancesAllianceIdOk {\n");
            sb.Append("  AllianceName: ").Append(AllianceName).Append("\n");
            sb.Append("  DateFounded: ").Append(DateFounded).Append("\n");
            sb.Append("  ExecutorCorp: ").Append(ExecutorCorp).Append("\n");
            sb.Append("  Ticker: ").Append(Ticker).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as GetAlliancesAllianceIdOk);
        }

        /// <summary>
        /// Returns true if GetAlliancesAllianceIdOk instances are equal
        /// </summary>
        /// <param name="input">Instance of GetAlliancesAllianceIdOk to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(GetAlliancesAllianceIdOk input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.AllianceName == input.AllianceName ||
                    (this.AllianceName != null &&
                    this.AllianceName.Equals(input.AllianceName))
                ) && 
                (
                    this.DateFounded == input.DateFounded ||
                    (this.DateFounded != null &&
                    this.DateFounded.Equals(input.DateFounded))
                ) && 
                (
                    this.ExecutorCorp == input.ExecutorCorp ||
                    (this.ExecutorCorp != null &&
                    this.ExecutorCorp.Equals(input.ExecutorCorp))
                ) && 
                (
                    this.Ticker == input.Ticker ||
                    (this.Ticker != null &&
                    this.Ticker.Equals(input.Ticker))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.AllianceName != null)
                    hashCode = hashCode * 59 + this.AllianceName.GetHashCode();
                if (this.DateFounded != null)
                    hashCode = hashCode * 59 + this.DateFounded.GetHashCode();
                if (this.ExecutorCorp != null)
                    hashCode = hashCode * 59 + this.ExecutorCorp.GetHashCode();
                if (this.Ticker != null)
                    hashCode = hashCode * 59 + this.Ticker.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
