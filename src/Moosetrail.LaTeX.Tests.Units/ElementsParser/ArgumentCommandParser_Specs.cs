﻿using System.Collections.Generic;
using System.Linq;
using Moosetrail.LaTeX.Elements;
using Moosetrail.LaTeX.ElementsParser;
using Moosetrail.LaTeX.Exceptions;
using Moosetrail.LaTeX.Helpers;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;

namespace Moosetrail.LaTeX.Tests.Units.ElementsParser
{
    [TestFixture]
    public class ArgumentCommandParser_Specs
    {
        private ArgumentCommandParser SUT;

        [SetUp]
        public void Setup()
        {
            SUT = new ArgumentCommandParser();
        }

        [TearDown]
        public void Teardown()
        {
            SUT = null;
        }

        [Test]
        public void should_be_LaTeXElementParser_for_Element()
        {
            Assert.IsInstanceOf<LaTexElementParser<ArgumentCommand>>(SUT);
        }
        [Test]
        public void should_be_LaTeXElementParser()
        {
            Assert.IsInstanceOf<LaTeXElementParser>(SUT);
        }

        #region CodeIndicators

        [Test]
        [TestCaseSource(nameof(allCommands))]
        public void codeIndicators_should_contain_singleslash_command(ArgumentCommandType argumentCommandType)
        {
            CollectionAssert.Contains(((LaTexElementParser<ArgumentCommand>)SUT).CodeIndicators, @"\\" + argumentCommandType);
        }

        [Test]
        [TestCaseSource(nameof(allCommands))]
        public void codeIndicators_should_contain_coded_slash(ArgumentCommandType argumentCommandType)
        {
            CollectionAssert.Contains(((LaTexElementParser<ArgumentCommand>)SUT).CodeIndicators, @"\\\\" + argumentCommandType);
        }

        [Test]
        [TestCaseSource(nameof(allCommands))]
        public void codeIndicators_should_contain_command_without_begin_escaped(ArgumentCommandType argumentCommandType)
        {
            CollectionAssert.Contains(((LaTexElementParser<ArgumentCommand>)SUT).CodeIndicators, "\\\\" + argumentCommandType);
        }

        #endregion CodeIndicators

        #region ParseCode

        [Test]
        public void parseCode_should_find_the_commandtype()
        {
            // Given
            var code = @"\documentclass{article}";

            // When
            var result = SUT.ParseCode(code);

            // Then
            Assert.AreEqual(ArgumentCommandType.documentclass, result.Item1.Type);
        }

        [Test]
        public void parseCode_should_find_the_commandtype_when_doubleSlash()
        {
            // Given
            var code = @"\\documentclass{article}";

            // When
            var result = SUT.ParseCode(code);

            // Then
            Assert.AreEqual(ArgumentCommandType.documentclass, result.Item1.Type);
        }

        [Test]
        public void parseCode_should_find_the_commandtype_when_unescaped_doubleSlash()
        {
            // Given
            var code = "\\\\documentclass{article}";

            // When
            var result = SUT.ParseCode(code);

            // Then
            Assert.AreEqual(ArgumentCommandType.documentclass, result.Item1.Type);
        }

        [Test]
        public void parseCode_should_throw_if_commandType_doesnt_exist()
        {
            // Given 
            var code = @"\unkowncommand{argument} Lorem ipsum dolor sit amet, assentior interpretaris eam ut, alii accommodare in usu, an cum tritani diceret qualisque. Mei eu ipsum timeam periculis, no per amet delenit inermis. Ius veri scribentur persequeris te. Per mutat augue ad, ei vide ullum appetere nam, ei pro everti reprehendunt. Qui purto molestie at, vis id feugait mnesarchum, stet erant deleniti est an.";

            // Then
            var ex = Assert.Throws<LaTeXParseCommandException>(() => SUT.ParseCode(code));
            Assert.AreEqual(@"\unkowncommand{argument}", ex.FailingCode);
            Assert.AreEqual(@"\unkowncommand{argument} Lorem ipsum dolor sit amet, assentior interpretaris eam ut, alii accommodar...", ex.InArea);
            Assert.AreEqual("Didn't recognize the command unkowncommand", ex.Message);
        }

        [Test]
        public void parseCode_should_find_first_requrired_argument()
        {
            // Given
            var code = @"\documentclass{article}";

            // When
            var result = SUT.ParseCode(code);

            // Then
            Assert.AreEqual("article", result.Item1.RequriredArguments.ElementAt(0));
        }

        [Test]
        public void parseCode_should_throw_if_first_requrired_argument_isnt_known()
        {
            // Given
            var code = @"\documentclass{unkown} Lorem ipsum dolor sit amet, assentior interpretaris eam ut, alii accommodare in usu, an cum tritani diceret qualisque. Mei eu ipsum timeam periculis, no per amet delenit inermis. Ius veri scribentur persequeris te. Per mutat augue ad, ei vide ullum appetere nam, ei pro everti reprehendunt. Qui purto molestie at, vis id feugait mnesarchum, stet erant deleniti est an.";

            // Then
            var ex = Assert.Throws<LaTeXParseCommandException>(() => SUT.ParseCode(code));
            Assert.AreEqual(@"\documentclass{unkown}", ex.FailingCode);
            Assert.AreEqual("The requrired argument 'unkown' isn't a known argument for the command 'documentclass'", ex.Message);
        }

        [Test]
        public void parseCode_should_throw_if_optional_argument_isnt_known()
        {
            // Given
            var code = @"\documentclass[unkown]{article} Lorem ipsum dolor sit amet, assentior interpretaris eam ut, alii accommodare in usu, an cum tritani diceret qualisque. Mei eu ipsum timeam periculis, no per amet delenit inermis. Ius veri scribentur persequeris te. Per mutat augue ad, ei vide ullum appetere nam, ei pro everti reprehendunt. Qui purto molestie at, vis id feugait mnesarchum, stet erant deleniti est an.";

            // Then
            var ex = Assert.Throws<LaTeXParseCommandException>(() => SUT.ParseCode(code));
            Assert.AreEqual(@"\documentclass[unkown]{article}", ex.FailingCode);
            Assert.AreEqual("The optional argument 'unkown' isn't a known argument for the command 'documentclass'", ex.Message);
        }

        [Test]
        public void parseCode_should_find_optional_argument_that_is_a_pattern()
        {
            // Given
            var code = @"\documentclass[10pt]{article}";

            // When
            var result = SUT.ParseCode(code);

            // Then
            Assert.AreEqual("10pt", result.Item1.OptionalArguments.ElementAt(0));
        }

        [Test]
        public void parsseCode_should_throw_if_optional_argument_isnt_allowed_for_a_requrired_argument_thats_set()
        {
            // Given
            var code = @"\documentclass[10pt]{slides}";

            // Then
            var ex = Assert.Throws<InvalidLatexCodeException>(() => SUT.ParseCode(code));
            Assert.AreEqual("The command 'documentclass' with the requrired argument/s 'slides' can't have the optional argument of '10pt'", ex.Message);
        }

        [Test]
        public void parsseCode_should_throw_if_multiple_optional_argument_dont_go_togheter()
        {
            // Given
            var code = @"\documentclass[draft,final]{article}";

            // Then
            var ex = Assert.Throws<InvalidLatexCodeException>(() => SUT.ParseCode(code));
            Assert.AreEqual("The command 'documentclass' can't have the both the optional arguments of 'draft' and 'final'", ex.Message);
        }

        [Test]
        public void parseCode_should_throw_if_optional_arguument_is_given_for_requrired_argument_not_in_allowed_list()
        {
            // Given
            var code = @"\documentclass[clock]{article}";

            // Then
            var ex = Assert.Throws<InvalidLatexCodeException>(() => SUT.ParseCode(code));
            Assert.AreEqual("The command 'documentclass' with the requrired argument/s 'article' can't have the optional argument of 'clock'", ex.Message);
        }

        [Test]
        public void parseCode_should_allow_any_requrired_argument_if_none_specified_in_rules()
        {
            // Given
            var code = @"\usepackage{myPackage}";

            // When
            var result = SUT.ParseCode(code);

            // Then
            CollectionAssert.Contains(result.Item1.RequriredArguments, "myPackage");
        }

        [Test]
        public void parseCode_should_allow_any_optional_argument_if_none_specified_in_rules()
        {
            // Given
            var code = @"\usepackage[myOption]{myPackage}";

            // When
            var result = SUT.ParseCode(code);

            // Then
            CollectionAssert.Contains(result.Item1.OptionalArguments, "myOption");
        }

        [Test]
        public void parseCode_should_return_code_with_the_parsed_code_removed()
        {
            // Given
            var code = @"\documentclass[10pt]{article}" +
                       @"\usepackage[myOption]{myPackage}";

            // When
            var result = SUT.ParseCode(code);

            // Then
            Assert.AreEqual(@"\usepackage[myOption]{myPackage}", result.Item2);
        }

        #endregion ParseCode

        #region TestHelpers

        private static IEnumerable<ArgumentCommandType> allCommands()
        {
            return EnumUtil.GetValues<ArgumentCommandType>();
        }

        #endregion TestHelpers
    }
}