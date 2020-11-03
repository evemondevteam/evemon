using System;
using EVEMon.Common.Enumerations.UISettings;

namespace EVEMon.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class UpdateAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="defaultPeriod">Default length of time between updates.</param>
        /// <param name="minimum">Minimum length of time between updates.</param>
        public UpdateAttribute(UpdatePeriod defaultPeriod, UpdatePeriod minimum)
        {
            CreateUpdateAttribute(defaultPeriod, minimum, UpdatePeriod.Week);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="defaultPeriod">Default length of time between updates.</param>
        /// <param name="minimum">Minimum length of time between updates.</param>
        /// <param name="maximum">Maximum length of time between updates.</param>
        public UpdateAttribute(UpdatePeriod defaultPeriod, UpdatePeriod minimum, UpdatePeriod maximum)
        {
            CreateUpdateAttribute(defaultPeriod, minimum, maximum);
        }

        /// <summary>
        /// Constructor helper method.
        /// </summary>
        /// <param name="defaultPeriod">Default length of time between updates.</param>
        /// <param name="minimum">Minimum length of time between updates.</param>
        /// <param name="maximum">Maximum length of time between updates.</param>
        private void CreateUpdateAttribute(UpdatePeriod defaultPeriod, UpdatePeriod minimum, UpdatePeriod maximum)
        {
            DefaultPeriod = defaultPeriod;
            Minimum = minimum;
            Maximum = maximum;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the default update period.
        /// </summary>
        /// <value>
        /// The default period.
        /// </value>
        public UpdatePeriod DefaultPeriod { get; private set; }

        /// <summary>
        /// Gets the minimum update period.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public UpdatePeriod Minimum { get; private set; }

        /// <summary>
        /// Gets the maximum update period.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public UpdatePeriod Maximum { get; private set; }

        #endregion
    }
}
