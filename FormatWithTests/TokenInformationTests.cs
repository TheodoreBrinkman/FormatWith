using Xunit;
using FormatWith;
using Xunit.Sdk;

namespace FormatWithTests
{
    public class TokenInformationTests
    {

        #region Test Values

        private static string testTokenKey = "token";
        private static int? testLeftPad = 15;
        private static int? testRightPad = -15;
        private static string testFormat = "yyyy-MM-dd";
        private static System.DateTime testDate = new System.DateTime(2024, 8, 29);

        public class Parse_TestData : TheoryData<string, string, int?, string>
        {
            public Parse_TestData()
            {
            }
        };

        #endregion

        #region BuildString

        [Fact]
        public void BuildString_TokenOnly()
        {
            string expected = "token";

            string result = TokenInformation.BuildString(testTokenKey);
            Assert.Equal(result, expected);
        }

        [Fact]
        public void BuildString_TokenAndLeftPad()
        {
            string expected = "token,15";

            string result = TokenInformation.BuildString(testTokenKey, testLeftPad);
            Assert.Equal(result, expected);
        }

        [Fact]
        public void BuildString_TokenAndRightPad()
        {
            string expected = "token,-15";

            string result = TokenInformation.BuildString(testTokenKey, testRightPad);
            Assert.Equal(result, expected);
        }

        [Fact]
        public void BuildString_TokenAndFormat()
        {
            string expected = "token:yyyy-MM-dd";

            string result = TokenInformation.BuildString(testTokenKey, testFormat);
            Assert.Equal(result, expected);
        }

        [Fact]
        public void BuildString_TokenLeftPadAndFormat()
        {
            string expected = "token,15:yyyy-MM-dd";

            string result = TokenInformation.BuildString(testTokenKey, testLeftPad, testFormat);
            Assert.Equal(result, expected);
        }

        [Fact]
        public void BuildString_TokenAndRightPadAndFormat()
        {
            string expected = "token,-15:yyyy-MM-dd";

            string result = TokenInformation.BuildString(testTokenKey, testRightPad, testFormat);
            Assert.Equal(result, expected);
        }

        #endregion

        #region Parse

        [Theory]
        [InlineData("token", "token", null, null, "")]
        [InlineData("token,15", "token", "15", null, ",15")]
        [InlineData("token,-15", "token", "-15", null, ",-15")]
        [InlineData("token:yyyy-MM-dd", "token", null, "yyyy-MM-dd", ":yyyy-MM-dd")]
        [InlineData("token,15:yyyy-MM-dd", "token", "15", "yyyy-MM-dd", ",15:yyyy-MM-dd")]
        [InlineData("token,-15:yyyy-MM-dd", "token", "-15", "yyyy-MM-dd", ",-15:yyyy-MM-dd")]
        public void Parse(string rawToken, string expectedTokenKey, string expectedAlignment, string expectedFormat, string expectedFormatString)
        {
            TokenInformation result = new TokenInformation(rawToken);

            Assert.Equal(result.RawToken, rawToken);
            Assert.Equal(result.TokenKey, expectedTokenKey);
            if (null == expectedAlignment)
            {
                Assert.Null(result.Alignment);
            }
            else
            {
                Assert.Equal(result.Alignment, expectedAlignment);
            }
            if (null == expectedFormat)
            {
                Assert.Null(result.Format);
            }
            else
            {
                Assert.Equal(result.Format, expectedFormat);
            }
            Assert.Equal(result.FormatString, expectedFormatString);
        }

        #endregion

        #region FormatWith

        [Fact]
        public void FormatWith_DateTest_FormatOnly()
        {
            System.Collections.Generic.Dictionary<string, object> replacmentValues = new System.Collections.Generic.Dictionary<string, object>();
            replacmentValues.Add(testTokenKey, testDate);

            string expectedResult = "'2024-08-29'";
            string result = "'{token:yyyy-MM-dd}'".FormatWith(replacmentValues);

            Assert.Equal(result, expectedResult);
        }

        [Fact]
        public void FormatWith_DateTest_FormatAndLeftPad()
        {
            System.Collections.Generic.Dictionary<string, object> replacmentValues = new System.Collections.Generic.Dictionary<string, object>();
            replacmentValues.Add(testTokenKey, testDate);

            string expectedResult = "'     2024-08-29'";
            string result = "'{token,15:yyyy-MM-dd}'".FormatWith(replacmentValues);

            Assert.Equal(result, expectedResult);
        }

        [Fact]
        public void FormatWith_DateTest_FormatAndRightPad()
        {
            System.Collections.Generic.Dictionary<string, object> replacmentValues = new System.Collections.Generic.Dictionary<string, object>();
            replacmentValues.Add(testTokenKey, testDate);

            string expectedResult = "'2024-08-29     '";
            string result = "'{token,-15:yyyy-MM-dd}'".FormatWith(replacmentValues);

            Assert.Equal(result, expectedResult);
        }

        #endregion

    }   // end class
}   // end namespace
