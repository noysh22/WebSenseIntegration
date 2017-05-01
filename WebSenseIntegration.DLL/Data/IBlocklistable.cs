namespace Siemplify.Integrations.WebSense.Data
{
    public interface IBlocklistable
    {
        string BuildBlocklistEntry();

        /// <summary>
        /// Validate if a blacklist entry is acceptable or not
        /// Throws an exception if entry is invalid 
        /// </summary>
        void ValidateEntry();
    }
}