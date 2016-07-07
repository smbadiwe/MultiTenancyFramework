namespace MultiTenancyFramework
{
    public class TrailItem
    {
        public TrailItem(string propertyName, string valueBefore, string valueAfter)
        {
            Property = propertyName;
            Before = valueBefore;
            After = valueAfter;
        }

        public TrailItem()
            : this("", "", "")
        {
        }

        public TrailItem(string propertyName)
            : this(propertyName, "", "")
        {
        }

        /// <summary>
        /// The name of the property we're trailing
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// The value before the change
        /// </summary>
        public string Before { get; set; }

        /// <summary>
        /// The value after the change
        /// </summary>
        public string After { get; set; }

        /// <summary>
        /// Gets a value indicating hether or not the value of the property was modified
        /// </summary>
        public bool Changed { get { return Before != After; } }

        public override string ToString()
        {
            return $"Before: {Before}; After: {After}; Changed: {Changed}";
        }
    }

}
