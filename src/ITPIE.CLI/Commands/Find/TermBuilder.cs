using System.Linq;

namespace ITPIE.CLI.Commands.Find
{
    public static class TermBuilder
    {
        public static Term Build(string name, int minLength, Term[] aliases = null)
        {
            return new Term
            {
                Name = name,
                Is = term =>
                {
                    return term != null && term.Length >= minLength
                    &&
                    (
                        name.StartsWith(term)
                        ||
                        (
                            aliases != null
                            &&
                            aliases.Any(o => o.Is(term))
                        )
                    );
                }
            };
        }
    }
}
