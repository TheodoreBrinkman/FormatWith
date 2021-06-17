﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FormatWith.Internal
{
    internal static class StringBuilderExtensions
    {
        public static void AppendWithEscapedBrackets(
                this StringBuilder stringBuilder,
                ReadOnlySpan<char> value,
                int startIndex,
                int count,
                char openBraceChar = '{',
                char closeBraceChar = '}')
        {
            int currentTokenStart = startIndex;
            for (int i = startIndex; i < startIndex + count; i++)
            {
                if (value[i] == openBraceChar)
                {
                    stringBuilder.Append(value.Slice(currentTokenStart, i - currentTokenStart));
                    stringBuilder.Append(openBraceChar);
                    currentTokenStart = i;
                }
                else if (value[i] == closeBraceChar)
                {
                    stringBuilder.Append(value.Slice(currentTokenStart, i - currentTokenStart));
                    stringBuilder.Append(closeBraceChar);
                    currentTokenStart = i;
                }
            }

            // add the final section
            stringBuilder.Append(value.Slice(currentTokenStart, (startIndex + count) - currentTokenStart));
        }
        
        
#if NETSTANDARD2_0
        /// <summary>
        /// Appends a Span to a StringBuilder.
        ///
        /// This is only needed in the netstandard2 implementation as under netstandard2.1
        /// there is a native method for this.
        /// </summary>
        /// <param name="builder">The builder to which the result should be appended.</param>
        /// <param name="value">The span to append</param>
        public static void Append(this StringBuilder builder, ReadOnlySpan<char> value)
        {
            builder.Append(value.ToArray());
        }
#endif
    }
}
