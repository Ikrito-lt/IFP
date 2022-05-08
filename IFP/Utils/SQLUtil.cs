namespace IFP.Utils
{
    /// <summary>
    /// ToDo: replace it with something more better
    /// </summary>
    static class SQLUtil
    {
        /// <summary>
        /// returns SQL safe string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SQLSafeString(string input)
        {
            input = input.Replace("\\", $"");
            input = input.Replace("\'", $"\\'");
            input = input.Replace("\"", $"\\\"");
            return input;
        }
    }
}
