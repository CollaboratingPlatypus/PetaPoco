using System;
using static PetaPoco.Utilities.SqlClauseScanner;

namespace PetaPoco.Utilities
{
    /// <summary>
    /// Identifies a SQL clause that <see cref="SqlClauseScanner"/> can locate in a statement.
    /// </summary>
    /// <remarks>
    /// <para>A clause is described by the pair of keywords that introduces it and, optionally, the vocabulary of the statement level
    /// clauses that may follow it. The scanner itself knows nothing about any particular clause.</para>
    /// </remarks>
    internal sealed class SqlClauseKind
    {
        /// <summary>
        /// The <c>ORDER BY</c> clause, which is followed by the row limiting and locking clauses recognised by
        /// <see cref="IsOrderByTrailingClauseAt"/>.
        /// </summary>
        public static readonly SqlClauseKind OrderBy = new SqlClauseKind("ORDER", "BY", IsOrderByTrailingClauseAt);

        /// <summary>
        /// The <c>GROUP BY</c> clause. Nothing consumes the extent of this clause, so it declares no trailing clause vocabulary.
        /// </summary>
        public static readonly SqlClauseKind GroupBy = new SqlClauseKind("GROUP", "BY", null);

        private readonly Func<string, int, bool> _isTrailingClauseAt;

        private SqlClauseKind(string keyword, string secondKeyword, Func<string, int, bool> isTrailingClauseAt)
        {
            Keyword = keyword;
            SecondKeyword = secondKeyword;
            _isTrailingClauseAt = isTrailingClauseAt;
        }

        /// <summary>
        /// Gets the keyword that introduces the clause, for example <c>ORDER</c>.
        /// </summary>
        public string Keyword { get; }

        /// <summary>
        /// Gets the keyword that completes the clause, for example <c>BY</c>.
        /// </summary>
        public string SecondKeyword { get; }

        /// <summary>
        /// Determines whether a statement level clause that may follow this clause starts at the given index.
        /// </summary>
        /// <param name="sql">The SQL statement being scanned.</param>
        /// <param name="index">The index to test.</param>
        /// <returns><see langword="true"/> when a trailing clause starts at <paramref name="index"/>; otherwise, <see langword="false"/>.</returns>
        public bool IsTrailingClauseAt(string sql, int index)
            => _isTrailingClauseAt != null && _isTrailingClauseAt(sql, index);

        /// <summary>
        /// Determines whether one of the clauses that may follow an <c>ORDER BY</c> clause starts at the given index.
        /// </summary>
        /// <remarks>
        /// <para>The clauses recognised are the ones that may follow an <c>ORDER BY</c> clause in the supported dialects:
        /// <c>LIMIT</c>, <c>OFFSET</c>, <c>FETCH</c>, <c>FOR UPDATE</c> / <c>FOR SHARE</c> and <c>OPTION</c>. A keyword only counts
        /// when it is followed by the construct that the clause requires, so that a column that happens to be named after such a
        /// keyword (for example <c>ORDER BY limit</c> on SQL Server) is not mistaken for a clause.</para>
        /// </remarks>
        private static bool IsOrderByTrailingClauseAt(string sql, int index)
        {
            // A qualified reference such as "t.limit" is a column reference, not a statement clause.
            if (index > 0 && sql[index - 1] == '.')
                return false;

            if (IsKeywordAt(sql, index, "LIMIT"))
            {
                var next = SkipWhitespace(sql, index + "LIMIT".Length);
                return IsNumberOrParameterAt(sql, next) || IsKeywordAt(sql, next, "ALL");
            }

            if (IsKeywordAt(sql, index, "OFFSET"))
                return IsNumberOrParameterAt(sql, SkipWhitespace(sql, index + "OFFSET".Length));

            if (IsKeywordAt(sql, index, "FETCH"))
            {
                var next = SkipWhitespace(sql, index + "FETCH".Length);
                return IsKeywordAt(sql, next, "FIRST") || IsKeywordAt(sql, next, "NEXT");
            }

            if (IsKeywordAt(sql, index, "FOR"))
            {
                var next = SkipWhitespace(sql, index + "FOR".Length);
                return IsKeywordAt(sql, next, "UPDATE") || IsKeywordAt(sql, next, "SHARE")
                    || IsKeywordAt(sql, next, "NO") || IsKeywordAt(sql, next, "KEY");
            }

            if (IsKeywordAt(sql, index, "OPTION"))
            {
                var next = SkipWhitespace(sql, index + "OPTION".Length);
                return next < sql.Length && sql[next] == '(';
            }

            return false;
        }
    }

    /// <summary>
    /// Describes the structure of an SQL statement, as identified by <see cref="SqlClauseScanner.Analyze"/>.
    /// </summary>
    /// <remarks>
    /// <para>Every index is a position in the statement that was analysed. The outermost statement is the one that follows the
    /// common table expression list, when the statement declares one. Nested statements, string literals, quoted identifiers and
    /// comments are never mistaken for the clauses of the outermost statement.</para>
    /// </remarks>
    internal struct SqlAnalysis
    {
        /// <summary>
        /// Gets or sets a value indicating whether an outermost <c>SELECT</c> statement that selects from a top level <c>FROM</c>
        /// clause was found.
        /// </summary>
        public bool Success;

        /// <summary>
        /// Gets or sets the common table expression list that precedes the outermost statement, or <see langword="null"/> when the
        /// statement declares no common table expressions.
        /// </summary>
        /// <remarks>
        /// The text is ready to be prepended to a statement, so that a statement which is derived from the analysed statement can
        /// keep the common table expressions that the outermost statement refers to.
        /// </remarks>
        public string Cte;

        /// <summary>
        /// Gets or sets the index of the <c>SELECT</c> keyword of the outermost statement.
        /// </summary>
        public int SelectIndex;

        /// <summary>
        /// Gets or sets the index of the first character of the outermost select list, which is the first character after the
        /// <c>SELECT</c> keyword and the whitespace that follows it.
        /// </summary>
        public int SelectListStart;

        /// <summary>
        /// Gets or sets the index of the <c>FROM</c> keyword that terminates the outermost select list.
        /// </summary>
        public int SelectListEnd;

        /// <summary>
        /// Gets or sets a value indicating whether the outermost statement selects distinct rows.
        /// </summary>
        public bool IsDistinct;

        /// <summary>
        /// Gets or sets the index of the <c>GROUP BY</c> clause of the outermost statement, or <c>-1</c> when it has none.
        /// </summary>
        public int GroupByIndex;

        /// <summary>
        /// Gets or sets the index of the <c>ORDER BY</c> clause of the outermost statement, or <c>-1</c> when it has none.
        /// </summary>
        public int OrderByIndex;

        /// <summary>
        /// Gets or sets the index one past the ordering expression list of the outermost <c>ORDER BY</c> clause, or <c>-1</c> when
        /// the statement has no <c>ORDER BY</c> clause or the clause has no ordering expression.
        /// </summary>
        public int OrderByEnd;

        /// <summary>
        /// Gets or sets the index of the statement terminator, or the length of the statement when it is not terminated.
        /// </summary>
        public int StatementEnd;
    }

    /// <summary>
    /// Locates SQL clauses in a statement, accounting for nested parentheses, string literals, quoted identifiers and comments.
    /// </summary>
    /// <remarks>
    /// <para>The scanner knows nothing about any particular clause; a clause is described by a <see cref="SqlClauseKind"/>. It
    /// replaces the right-to-left regular expressions that used to locate the clauses of a paging statement, which backtracked
    /// catastrophically on large statements that do not contain the clause being searched for.</para>
    /// </remarks>
    internal static class SqlClauseScanner
    {
        /// <summary>
        /// Analyses a statement, locating the outermost <c>SELECT</c> statement and the clauses that are relevant to paging it.
        /// </summary>
        /// <remarks>
        /// <para>The clauses are located in a single pass, and no dialect is assumed: a clause is recognised by its keywords, and
        /// the vocabulary of the statement level clauses that may follow an <c>ORDER BY</c> clause is declared by
        /// <see cref="SqlClauseKind.OrderBy"/>. Nested statements, string literals, quoted identifiers and comments are skipped, and
        /// the common table expression list that may precede the outermost statement is identified, so that a statement which
        /// declares common table expressions is analysed like any other.</para>
        /// </remarks>
        /// <param name="sql">The SQL statement to analyse.</param>
        /// <returns>The structure of the statement.</returns>
        public static SqlAnalysis Analyze(string sql)
        {
            var analysis = new SqlAnalysis
            {
                Cte = null,
                SelectIndex = -1,
                SelectListStart = -1,
                SelectListEnd = -1,
                IsDistinct = false,
                GroupByIndex = -1,
                OrderByIndex = -1,
                OrderByEnd = -1,
                StatementEnd = sql?.Length ?? 0,
                Success = false,
            };

            if (string.IsNullOrEmpty(sql))
                return analysis;

            var index = SkipStatementNoise(sql, 0);

            // A statement may declare common table expressions, in which case the outermost statement follows them.
            if (IsKeywordAt(sql, index, "WITH"))
            {
                var cteEnd = FindCteListEnd(sql, index);
                if (cteEnd < 0)
                    return analysis;

                cteEnd = SkipStatementNoise(sql, cteEnd);
                analysis.Cte = sql.Substring(index, cteEnd - index);
                index = cteEnd;
            }

            index = SkipStatementNoise(sql, index);
            if (!IsKeywordAt(sql, index, "SELECT"))
                return analysis;

            analysis.SelectIndex = index;

            var selectListStart = SkipWhitespace(sql, index + "SELECT".Length);
            analysis.SelectListStart = selectListStart;
            analysis.IsDistinct = IsKeywordAt(sql, selectListStart, "DISTINCT");

            var parenthesesDepth = 0;
            var orderByExpressionStart = -1;
            var orderByScanned = false;

            for (index = selectListStart; index < sql.Length;)
            {
                // Once the ORDER BY clause has been located the scan is also looking for the end of its ordering expression
                // list, which is the first statement terminator, comment or statement level clause that may follow it. Ordering
                // expressions may contain parentheses, so only the outermost level is considered.
                if (orderByExpressionStart >= 0 && !orderByScanned && parenthesesDepth == 0 && index >= orderByExpressionStart
                    && IsScanStopAt(sql, index, SqlClauseKind.OrderBy))
                {
                    orderByScanned = true;
                    analysis.OrderByEnd = index;
                }

                var quoted = SkipQuotedSection(sql, index);
                if (quoted > index)
                {
                    index = quoted;
                    continue;
                }

                if (IsCommentStart(sql, index))
                {
                    index = SkipComment(sql, index);
                    continue;
                }

                var ch = sql[index];

                if (ch == '(')
                {
                    parenthesesDepth++;
                }
                else if (ch == ')')
                {
                    if (parenthesesDepth > 0)
                        parenthesesDepth--;
                }
                else if (parenthesesDepth == 0)
                {
                    if (ch == ';')
                    {
                        analysis.StatementEnd = index;
                        break;
                    }

                    if (analysis.SelectListEnd < 0 && IsKeywordAt(sql, index, "FROM"))
                    {
                        analysis.SelectListEnd = index;
                    }
                    else if (IsClauseKeywordAt(sql, index, SqlClauseKind.GroupBy))
                    {
                        analysis.GroupByIndex = index;
                    }
                    else if (IsClauseKeywordAt(sql, index, SqlClauseKind.OrderBy))
                    {
                        var expressionStart = FindClauseExpressionStart(sql, index, SqlClauseKind.OrderBy);
                        if (expressionStart >= 0)
                        {
                            analysis.OrderByIndex = index;
                            analysis.OrderByEnd = -1;
                            orderByExpressionStart = expressionStart;
                            orderByScanned = false;
                        }
                    }
                }

                index++;
            }

            if (analysis.OrderByIndex >= 0)
            {
                if (!orderByScanned)
                    analysis.OrderByEnd = sql.Length;

                // A statement level clause immediately after the keywords means the clause has no ordering expression.
                if (analysis.OrderByEnd == orderByExpressionStart)
                    analysis.OrderByEnd = -1;
            }

            analysis.Success = analysis.SelectListEnd >= 0;
            return analysis;
        }

        /// <summary>
        /// Determines whether the clause introduced by the keywords of <paramref name="clause"/> starts at the given index.
        /// </summary>
        /// <remarks>
        /// <para>The keywords are separated by whitespace, and the second keyword may be followed by the end of the statement
        /// rather than by an expression.</para>
        /// </remarks>
        private static bool IsClauseKeywordAt(string sql, int index, SqlClauseKind clause)
        {
            if (!IsKeywordAt(sql, index, clause.Keyword))
                return false;

            return IsKeywordAt(sql, SkipWhitespace(sql, index + clause.Keyword.Length), clause.SecondKeyword, true);
        }

        /// <summary>
        /// Returns the index of the first character of the expression that the clause starting at <paramref name="clauseIndex"/>
        /// introduces, or <c>-1</c> when the clause has no expression.
        /// </summary>
        private static int FindClauseExpressionStart(string sql, int clauseIndex, SqlClauseKind clause)
        {
            // Mirror the "\bORDER\s+BY\s+" prefix: whitespace is mandatory on both sides of the keyword pair.
            var afterKeyword = clauseIndex + clause.Keyword.Length;
            var secondKeyword = SkipWhitespace(sql, afterKeyword);
            if (secondKeyword == afterKeyword)
                return -1;

            var expressionStart = SkipWhitespace(sql, secondKeyword + clause.SecondKeyword.Length);
            return expressionStart == secondKeyword + clause.SecondKeyword.Length ? -1 : expressionStart;
        }

        /// <summary>
        /// Returns the index of the first token of the statement, skipping leading whitespace, comments and empty statements.
        /// </summary>
        private static int SkipStatementNoise(string sql, int index)
        {
            while (index < sql.Length)
            {
                var whitespace = SkipWhitespace(sql, index);
                if (whitespace > index)
                {
                    index = whitespace;
                    continue;
                }

                var comment = SkipComment(sql, index);
                if (comment > index)
                {
                    index = comment;
                    continue;
                }

                if (sql[index] != ';')
                    break;

                index++;
            }

            return index;
        }

        /// <summary>
        /// Returns the index of the first token that follows the common table expression list that starts at the <c>WITH</c>
        /// keyword at <paramref name="withIndex"/>, or <c>-1</c> when the list is malformed.
        /// </summary>
        /// <remarks>
        /// <para>A common table expression is a name, an optional column list, the <c>AS</c> keyword and a parenthesised statement.
        /// The <c>RECURSIVE</c> modifier that may follow the <c>WITH</c> keyword is ignored, and the list may contain any number of
        /// expressions separated by commas.</para>
        /// </remarks>
        private static int FindCteListEnd(string sql, int withIndex)
        {
            var index = SkipWhitespace(sql, withIndex + "WITH".Length);

            if (IsKeywordAt(sql, index, "RECURSIVE"))
                index = SkipWhitespace(sql, index + "RECURSIVE".Length);

            while (true)
            {
                index = SkipCteName(sql, index);
                if (index < 0)
                    return -1;

                index = SkipWhitespace(sql, index);

                // The column list of the common table expression is optional.
                if (index < sql.Length && sql[index] == '(')
                {
                    index = SkipParenthesisedSection(sql, index);
                    if (index < 0)
                        return -1;

                    index = SkipWhitespace(sql, index);
                }

                if (!IsKeywordAt(sql, index, "AS"))
                    return -1;

                index = SkipWhitespace(sql, index + "AS".Length);
                if (index >= sql.Length || sql[index] != '(')
                    return -1;

                index = SkipParenthesisedSection(sql, index);
                if (index < 0)
                    return -1;

                index = SkipWhitespace(sql, index);

                if (index >= sql.Length || sql[index] != ',')
                    return index;

                index = SkipWhitespace(sql, index + 1);
            }
        }

        /// <summary>
        /// Returns the index one past the name of a common table expression that starts at <paramref name="index"/>, or <c>-1</c>
        /// when no name starts there.
        /// </summary>
        private static int SkipCteName(string sql, int index)
        {
            var quoted = SkipQuotedSection(sql, index);
            if (quoted > index)
                return quoted;

            if (index >= sql.Length || !IsIdentifierStart(sql[index]))
                return -1;

            index++;

            while (index < sql.Length && IsIdentifierPart(sql[index]))
                index++;

            return index;
        }

        /// <summary>
        /// Returns the index one past the parenthesised section that starts at the opening parenthesis at
        /// <paramref name="openIndex"/>, or <c>-1</c> when the section is not terminated.
        /// </summary>
        private static int SkipParenthesisedSection(string sql, int openIndex)
        {
            var parenthesesDepth = 1;

            for (var i = openIndex + 1; i < sql.Length;)
            {
                var quoted = SkipQuotedSection(sql, i);
                if (quoted > i)
                {
                    i = quoted;
                    continue;
                }

                if (IsCommentStart(sql, i))
                {
                    i = SkipComment(sql, i);
                    continue;
                }

                if (sql[i] == '(')
                {
                    parenthesesDepth++;
                }
                else if (sql[i] == ')')
                {
                    parenthesesDepth--;
                    if (parenthesesDepth == 0)
                        return i + 1;
                }

                i++;
            }

            return -1;
        }

        /// <summary>
        /// Determines whether the clause that is being scanned ends at the given index. A clause ends at the statement terminator,
        /// at a comment, or at one of the statement level clauses that may follow it.
        /// </summary>
        private static bool IsScanStopAt(string sql, int index, SqlClauseKind clause)
        {
            if (sql[index] == ';')
                return true;

            return IsCommentStart(sql, index) || clause.IsTrailingClauseAt(sql, index);
        }

        /// <summary>
        /// Determines whether a keyword appears at the given index, on a keyword boundary.
        /// </summary>
        /// <param name="sql">The SQL statement being scanned.</param>
        /// <param name="index">The index to test.</param>
        /// <param name="keyword">The keyword to look for.</param>
        /// <param name="allowTrailingWhitespace">
        /// When <see langword="true"/>, the keyword may be followed by whitespace, the end of the statement, a statement terminator,
        /// a comma or a closing parenthesis instead of requiring a keyword boundary.
        /// </param>
        /// <returns><see langword="true"/> when the keyword appears at <paramref name="index"/>; otherwise, <see langword="false"/>.</returns>
        internal static bool IsKeywordAt(string sql, int index, string keyword, bool allowTrailingWhitespace = false)
        {
            if (index < 0 || index + keyword.Length > sql.Length)
                return false;

            if (index > 0 && IsIdentifierPart(sql[index - 1]))
                return false;

            if (string.Compare(sql, index, keyword, 0, keyword.Length, true) != 0)
                return false;

            var keywordEnd = index + keyword.Length;
            if (allowTrailingWhitespace)
            {
                return keywordEnd == sql.Length || char.IsWhiteSpace(sql[keywordEnd]) || sql[keywordEnd] == ';' || sql[keywordEnd] == ',' || sql[keywordEnd] == ')';
            }

            if (keywordEnd < sql.Length && IsIdentifierPart(sql[keywordEnd]))
                return false;

            return true;
        }

        /// <summary>
        /// Returns the index of the first character at or after <paramref name="index"/> that is not whitespace.
        /// </summary>
        internal static int SkipWhitespace(string sql, int index)
        {
            while (index < sql.Length && char.IsWhiteSpace(sql[index]))
            {
                index++;
            }

            return index;
        }

        /// <summary>
        /// Determines whether a number or a parameter marker starts at the given index.
        /// </summary>
        internal static bool IsNumberOrParameterAt(string sql, int index)
        {
            if (index >= sql.Length)
                return false;

            var ch = sql[index];
            return char.IsDigit(ch) || ch == '?' || ch == '@' || ch == ':' || ch == '$';
        }

        /// <summary>
        /// Returns the index one past the end of the quoted section that starts at <paramref name="index"/>, or
        /// <paramref name="index"/> when no quoted section starts there.
        /// </summary>
        private static int SkipQuotedSection(string sql, int index)
        {
            var ch = sql[index];

            if (ch == '\'')
            {
                for (var i = index + 1; i < sql.Length; i++)
                {
                    if (sql[i] != '\'')
                        continue;

                    // A doubled quote is an escaped quote, not the end of the literal.
                    if (i + 1 < sql.Length && sql[i + 1] == '\'')
                    {
                        i++;
                        continue;
                    }

                    return i + 1;
                }

                return sql.Length;
            }

            if (ch == '"' || ch == '`')
            {
                var end = sql.IndexOf(ch, index + 1);
                return end < 0 ? sql.Length : end + 1;
            }

            if (ch == '[')
            {
                var end = sql.IndexOf(']', index + 1);
                return end < 0 ? sql.Length : end + 1;
            }

            return index;
        }

        /// <summary>
        /// Returns the index one past the end of the comment that starts at <paramref name="index"/>, or
        /// <paramref name="index"/> when no comment starts there.
        /// </summary>
        private static int SkipComment(string sql, int index)
        {
            if (!IsCommentStart(sql, index))
                return index;

            if (sql[index] == '-')
            {
                var end = sql.IndexOf('\n', index + 2);
                return end < 0 ? sql.Length : end + 1;
            }

            var terminator = sql.IndexOf("*/", index + 2, StringComparison.Ordinal);
            return terminator < 0 ? sql.Length : terminator + 2;
        }

        /// <summary>
        /// Determines whether a comment starts at the given index.
        /// </summary>
        private static bool IsCommentStart(string sql, int index)
        {
            if (index + 1 >= sql.Length)
                return false;

            return (sql[index] == '-' && sql[index + 1] == '-') || (sql[index] == '/' && sql[index + 1] == '*');
        }

        private static bool IsIdentifierStart(char ch)
            => char.IsLetter(ch) || ch == '_';

        private static bool IsIdentifierPart(char ch)
            => char.IsLetterOrDigit(ch) || ch == '_' || ch == '$';
    }
}
