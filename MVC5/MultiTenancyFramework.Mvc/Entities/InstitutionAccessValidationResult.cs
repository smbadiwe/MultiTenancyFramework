namespace MultiTenancyFramework.Entities
{
    public class InstitutionAccessValidationResult
    {
        public bool AllowAccess { get; set; }

        /// <summary>
        /// if <seealso cref="AllowAccess"/> is false, the reason for the denial will usually be learnt from this property
        /// </summary>
        public string Remarks { get; set; }
    }
}
