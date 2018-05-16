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
        /// <param name="cacheStyle">Cache style.</param>
        public UpdateAttribute(UpdatePeriod defaultPeriod, UpdatePeriod minimum, CacheStyle cacheStyle)
        {
            CreateUpdateAttribute(defaultPeriod, minimum, UpdatePeriod.Week, cacheStyle);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="defaultPeriod">Default length of time between updates.</param>
        /// <param name="minimum">Minimum length of time between updates.</param>
        /// <param name="maximum">Maximum length of time between updates.</param>
        /// <param name="cacheStyle">Cache style.</param>
        public UpdateAttribute(UpdatePeriod defaultPeriod, UpdatePeriod minimum, UpdatePeriod maximum, CacheStyle cacheStyle)
        {
            CreateUpdateAttribute(defaultPeriod, minimum, maximum, cacheStyle);
        }

        /// <summary>
        /// Constructor helper method.
        /// </summary>
        /// <param name="defaultPeriod">Default length of time between updates.</param>
        /// <param name="minimum">Minimum length of time between updates.</param>
        /// <param name="maximum">Maximum length of time between updates.</param>
        /// <param name="cacheStyle">Cache style.</param>
        private void CreateUpdateAttribute(UpdatePeriod defaultPeriod, UpdatePeriod minimum, UpdatePeriod maximum, CacheStyle cacheStyle)
        {
            DefaultPeriod = defaultPeriod;
            Minimum = minimum;
            Maximum = maximum;
            CacheStyle = cacheStyle;
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

        /// <summary>
        /// Gets the update period cache style. EVEMon does not actually use this attribute!
        /// </summary>
        /// <value>
        /// The cache style.
        /// </value>
        public CacheStyle CacheStyle { get; private set; }

        #endregion
    }
}
