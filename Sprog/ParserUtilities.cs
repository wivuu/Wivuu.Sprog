using System;

namespace Wivuu.Sprog
{
    public static class Utilities
    {
        /// <summary>
        /// Returns a negated predicate
        /// </summary>
        /// <param name="value">Input predicate</param>
        /// <returns>The negation of the input pattern</returns>
        public static Predicate Not(Predicate fun)
        {
            if (fun == null)
                throw new ArgumentNullException(nameof(fun));

            bool not(char c) => !fun(c);
            return not;
        }
    }
}
