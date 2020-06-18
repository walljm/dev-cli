namespace ITPIE.CLI
{
    public static class Extensions
    {
        /// <summary>
        /// Changes a "CamelCasedString" to "Camel Cased String"
        /// </summary>
        /// <param name="camelCasedString"></param>
        /// <returns></returns>
        public static string SpaceByCamelCase(this string camelCasedString)
        {
            return System.Text.RegularExpressions.Regex.Replace(camelCasedString,
                "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
        }
    }
}
