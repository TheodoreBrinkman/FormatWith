namespace FormatWith
{

    /// <summary>
    /// Encapsulates the token-parsing logic and values, ensuring consistent behavior and allowing them to be passed to handlers more concisely.
    /// </summary>
    public class TokenInformation
    {

        #region Static & Const

        ///// <summary>
        ///// A regular expression to parse the token
        ///// </summary>
        //private static readonly System.Text.RegularExpressions.Regex _parseRegEx = new System.Text.RegularExpressions.Regex(@"(?<token>([^,;:\s]+))(,(?<alignment>-?\d+))?(:(?<format>.*))?", System.Text.RegularExpressions.RegexOptions.Compiled);

        #endregion

        #region Properties

        /// <summary>
        /// The raw token parsed.
        /// </summary>
        public string RawToken { get; private set; }

        /// <summary>
        /// The token key.
        /// </summary>
        public string TokenKey { get; private set; }

        /// <summary>
        /// The alignment indicator
        /// </summary>
        public string Alignment { get; private set; }

        /// <summary>
        /// The base format string. (no alignment)
        /// </summary>
        public string Format { get; private set; }

        private string _formatString = null;
        /// <summary>
        /// The full composite formatting string. [,align][:format]
        /// </summary>
        public string FormatString
        {
            get
            {
                if (null == this._formatString)
                {
                    this._formatString = string.Concat((!string.IsNullOrEmpty(this.Alignment) ? $",{this.Alignment}" : string.Empty), (!string.IsNullOrEmpty(this.Format) ? $":{this.Format}" : string.Empty));
                }
                return (this._formatString);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Parses token information from a raw token string value.
        /// </summary>
        /// <param name="rawToken">The raw token string.</param>
        /// <remarks>IndexOf/Substring version of parser based on existing code</remarks>
        public TokenInformation(string rawToken)
        {
            this.RawToken = rawToken;
            this.Format = null;
            this.Alignment = null;

            // Parse out the format
            var separatorIdx = rawToken.IndexOf(":", System.StringComparison.Ordinal);
            if (separatorIdx > -1)
            {
                this.Format = rawToken.Substring(separatorIdx + 1);
                rawToken = rawToken.Substring(0, separatorIdx);
            }

            // Parse out alignment
            var alignmentIdx = rawToken.LastIndexOf(",", System.StringComparison.Ordinal);
            if (alignmentIdx > -1)
            {
                this.Alignment = rawToken.Substring(alignmentIdx + 1);
                rawToken = rawToken.Substring(0, alignmentIdx);
            }

            // Grab the token key
            this.TokenKey = rawToken;
        }

        ///// <summary>
        ///// Parses token information from a raw token string value.
        ///// </summary>
        ///// <param name="rawToken">The raw token string.</param>
        ///// <remarks>RegEx version of parser, for performance comparison, and in case someone else knows how to optimize it more</remarks>
        //public TokenInformation(string rawToken, bool ignoreThisParameterItIsHereForPerformanceComparisons)
        //{
        //	this.RawToken = rawToken;
        //	this.Alignment = null;
        //	this.Format = null;
        //	
        //	System.Text.RegularExpressions.Match results = _parseRegEx.Match(rawToken);
        //	if(results.Success)
        //	{
        //		this.TokenKey = results.Groups["token"].Value;
        //		this.Alignment = results.Groups["alignment"].Value;
        //		this.Format = results.Groups["format"].Value;
        //	}
        //}

        #endregion

        #region Methods

        private static string BuildString(string tokenKey, string alignment, string format)
        {
            string rawToken = string.Concat(tokenKey, (!string.IsNullOrEmpty(alignment) ? $",{alignment}" : string.Empty), (!string.IsNullOrEmpty(format) ? $":{format}" : string.Empty));
            return (rawToken);
        }

        /// <summary>
        /// Builds a raw token string given the desired tokenKey and format
        /// </summary>
        /// <param name="tokenKey">The key value for the token.</param>
        /// <param name="alignment">The left- or right-padding indicator desired.</param>
        /// <param name="format">The format string.</param>
        /// <returns></returns>
        public static string BuildString(string tokenKey, int? alignment, string format)
        {
            return (TokenInformation.BuildString(tokenKey, alignment.HasValue ? $"{alignment}" : string.Empty, format));
        }

        /// <summary>
        /// Builds a raw token string given the desired tokenKey and format
        /// </summary>
        /// <param name="tokenKey">The key value for the token.</param>
        /// <param name="format">The format string.</param>
        /// <returns></returns>
        public static string BuildString(string tokenKey, string format)
        {
            return (TokenInformation.BuildString(tokenKey, (int?)null, format));
        }

        /// <summary>
        /// Builds a raw token string given the desired tokenKey and format
        /// </summary>
        /// <param name="tokenKey">The key value for the token.</param>
        /// <param name="alignment">The left- or right-padding indicator desired.</param>
        /// <returns></returns>
        public static string BuildString(string tokenKey, int? alignment)
        {
            return (TokenInformation.BuildString(tokenKey, alignment, null));
        }

        /// <summary>
        /// Builds a raw token string given the desired tokenKey and format
        /// </summary>
        /// <param name="tokenKey">The key value for the token.</param>
        /// <returns></returns>
        public static string BuildString(string tokenKey)
        {
            return(TokenInformation.BuildString(tokenKey, (int?)null, null));
        }

        /// <summary>
        /// Builds a TokenInfo object given the desired tokenKey and format
        /// </summary>
        /// <param name="tokenKey">The key value for the token.</param>
        /// <param name="alignment">The left- or right-padding indicator desired.</param>
        /// <param name="format">The format string.</param>
        /// <returns></returns>
        public static TokenInformation Build(string tokenKey, int? alignment, string format)
        {
            return (new TokenInformation(TokenInformation.BuildString(tokenKey, alignment, format)));
        }

        /// <summary>
        /// Builds a TokenInfo object given the desired tokenKey and format
        /// </summary>
        /// <param name="tokenKey">The key value for the token.</param>
        /// <param name="format">The format string.</param>
        /// <returns></returns>
        public static TokenInformation Build(string tokenKey, string format)
        {
            return (TokenInformation.Build(tokenKey, null, format));
        }

        #endregion

    }   // end class
}   // end namespace
