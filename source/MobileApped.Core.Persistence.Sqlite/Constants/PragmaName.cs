using System;
using System.Linq;
using System.Runtime.Serialization;

namespace MobileApped.Core.Persistence.Sqlite.Constants
{
    /// <summary>
    /// <see cref="https://www.tutorialspoint.com/sqlite/sqlite_pragma.htm"/>
    /// </summary>
    public enum PragmaName
    {
        [EnumMember(Value = "pragma_name")] Name,
        [EnumMember(Value = "auto_vacuum")] AutoVacuum,
        [EnumMember(Value = "cache_size")] CacheSize,
        [EnumMember(Value = "case_sensitive_like")] CaseSensitiveLike,
        [EnumMember(Value = "count_changes")] CountChanges,

        /// <summary>
        /// List all attached databases.
        /// </summary>
        [EnumMember(Value = "database_list")] DatabaseList,

        /// <summary>
        /// Controls how strings are encoded and stored in a database file.
        /// </summary>
        [EnumMember(Value = "encoding")] Encoding,
        
        /// <summary>
        /// Returns a single integer indicating how many database pages are currently marked as free and available.
        /// </summary>
        [EnumMember(Value = "freelist_count")] FreeListCount,

        /// <summary>
        /// Lists all of the indexes associated with a table.
        /// </summary>
        [EnumMember(Value = "index_list")] IndexList,

        /// <summary>
        /// Gets or sets the journal mode which controls how the journal file is stored and processed
        ///</summary>
        /// <para>1: Delete</para>
        /// <para>2: Truncate</para>
        /// <para>3: Persist</para>
        /// <para>4: Memory</para>
        /// <para>5: Off</para>
        [EnumMember(Value = "journal_mode")] JournalMode,
    }

    public static class PragmaNameExtensions
    {
        public static string GetValue(this PragmaName pragma)
        {
            var enumType = typeof(PragmaName);
            var member = enumType.GetMember(Enum.GetName(typeof(PragmaName), pragma))?.FirstOrDefault();

            EnumMemberAttribute attribute = member.GetCustomAttributes(
                    typeof(EnumMemberAttribute), true).FirstOrDefault()
                    as EnumMemberAttribute;
            return attribute?.Value;
        }
    }
}
