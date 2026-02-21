using UI.EmployerPortal.Razor.SharedComponents.Inputs;

namespace Test.UI.EmployerPortal.Razor.SharedComponents.Inputs;

/// <summary>
/// Unit tests for the static methods of the <see cref="SSNInput"/> component:
/// <see cref="SSNInput.ValidateSSN"/>, <see cref="SSNInput.FormatSSN"/>,
/// <see cref="SSNInput.MaskSSN"/>, and <see cref="SSNInput.FindDuplicateSSNs"/>.
/// </summary>
public class SSNInputTests
{
    // =========================================================
    // ValidateSSN
    // =========================================================

    /// <summary>
    /// Verifies that <see cref="SSNInput.ValidateSSN"/> returns an invalid result
    /// with a "required" error message when the SSN is <see langword="null"/>,
    /// empty, or whitespace.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateSSN_WhenNullOrEmpty_ReturnsInvalidWithRequiredMessage(string? ssn)
    {
        // Arrange
        const string ExpectedError = "SSN is required";

        // Act
        var result = SSNInput.ValidateSSN(ssn);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(ExpectedError, result.ErrorMessage);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.ValidateSSN"/> uses the provided
    /// <c>fieldLabel</c> in the "required" error message when the SSN is <see langword="null"/>.
    /// </summary>
    [Fact]
    public void ValidateSSN_WhenNullOrEmpty_UsesCustomFieldLabel()
    {
        // Arrange
        const string CustomLabel = "Employee SSN";

        // Act
        var result = SSNInput.ValidateSSN(null, CustomLabel);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal($"{CustomLabel} is required", result.ErrorMessage);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.ValidateSSN"/> returns an invalid result
    /// with a format error message when the SSN contains fewer than 9 digits.
    /// </summary>
    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    [InlineData("1234567")]
    public void ValidateSSN_WhenFewerThanNineDigits_ReturnsInvalidWithFormatMessage(string ssn)
    {
        // Arrange
        const string ExpectedError = "SSN format must be 999-99-9999";

        // Act
        var result = SSNInput.ValidateSSN(ssn);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(ExpectedError, result.ErrorMessage);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.ValidateSSN"/> returns an invalid result
    /// with a format error message when the SSN contains more than 9 digits.
    /// </summary>
    [Fact]
    public void ValidateSSN_WhenMoreThanNineDigits_ReturnsInvalidWithFormatMessage()
    {
        // Arrange
        const string SSN = "12345678901";
        const string ExpectedError = "SSN format must be 999-99-9999";

        // Act
        var result = SSNInput.ValidateSSN(SSN);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(ExpectedError, result.ErrorMessage);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.ValidateSSN"/> returns an invalid result
    /// when all 9 digits of the SSN are identical (e.g., "111111111").
    /// </summary>
    [Theory]
    [InlineData("111111111")]
    [InlineData("000000000")]
    [InlineData("999999999")]
    public void ValidateSSN_WhenAllSameDigits_ReturnsInvalidWithSameCharacterMessage(string ssn)
    {
        // Arrange
        const string ExpectedError = "SSN cannot be all the same character";

        // Act
        var result = SSNInput.ValidateSSN(ssn);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(ExpectedError, result.ErrorMessage);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.ValidateSSN"/> returns an invalid result
    /// when the SSN digits are the known sequential value "123456789".
    /// </summary>
    [Fact]
    public void ValidateSSN_WhenSequential123456789_ReturnsInvalid()
    {
        // Arrange
        const string SSN = "123456789";
        const string ExpectedError = "SSN cannot be \"123456789\"";

        // Act
        var result = SSNInput.ValidateSSN(SSN);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(ExpectedError, result.ErrorMessage);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.ValidateSSN"/> returns a valid result
    /// with no error message for well-formed SSNs.
    /// </summary>
    [Theory]
    [InlineData("123-45-6780")]
    [InlineData("987-65-4321")]
    [InlineData("246813579")]
    public void ValidateSSN_WhenValidSSN_ReturnsValid(string ssn)
    {
        // Arrange — ssn provided via InlineData

        // Act
        var result = SSNInput.ValidateSSN(ssn);

        // Assert
        Assert.True(result.IsValid);
        Assert.Null(result.ErrorMessage);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.ValidateSSN"/> uses the provided
    /// <c>fieldLabel</c> across all validation error messages, not just the required check.
    /// </summary>
    [Fact]
    public void ValidateSSN_WithCustomLabel_UsesLabelInAllErrorMessages()
    {
        // Arrange
        const string SSN = "111111111";
        const string CustomLabel = "Spouse SSN";

        // Act
        var result = SSNInput.ValidateSSN(SSN, CustomLabel);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal("Spouse SSN cannot be all the same character", result.ErrorMessage);
    }

    // =========================================================
    // FormatSSN
    // =========================================================

    /// <summary>
    /// Verifies that <see cref="SSNInput.FormatSSN"/> returns an empty string
    /// when the input is <see langword="null"/>, empty, or whitespace.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FormatSSN_WhenNullOrEmpty_ReturnsEmpty(string? value)
    {
        // Arrange — value provided via InlineData

        // Act
        var result = SSNInput.FormatSSN(value!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FormatSSN"/> returns only digits,
    /// without dashes, when the input contains 3 or fewer digits.
    /// </summary>
    [Theory]
    [InlineData("1", "1")]
    [InlineData("12", "12")]
    [InlineData("123", "123")]
    public void FormatSSN_WhenUpToThreeDigits_ReturnsDigitsOnly(string input, string expected)
    {
        // Arrange — Input/Expected provided via InlineData

        // Act
        var result = SSNInput.FormatSSN(input);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FormatSSN"/> inserts a single dash
    /// after the third digit when the input contains 4 or 5 digits (e.g., "123-4").
    /// </summary>
    [Theory]
    [InlineData("1234", "123-4")]
    [InlineData("12345", "123-45")]
    public void FormatSSN_WhenFourOrFiveDigits_ReturnsWithSingleDash(string input, string expected)
    {
        // Arrange — Input/Expected provided via InlineData

        // Act
        var result = SSNInput.FormatSSN(input);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FormatSSN"/> produces the full
    /// "XXX-XX-XXXX" pattern when the input contains 6 to 9 digits.
    /// </summary>
    [Theory]
    [InlineData("123456", "123-45-6")]
    [InlineData("1234567", "123-45-67")]
    [InlineData("12345678", "123-45-678")]
    [InlineData("123456789", "123-45-6789")]
    public void FormatSSN_WhenSixToNineDigits_ReturnsWithTwoDashes(string input, string expected)
    {
        // Arrange — Input/Expected provided via InlineData

        // Act
        var result = SSNInput.FormatSSN(input);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FormatSSN"/> truncates input to 9 digits
    /// before formatting when more than 9 digits are provided.
    /// </summary>
    [Fact]
    public void FormatSSN_WhenMoreThanNineDigits_TruncatesToNineAndFormats()
    {
        // Arrange
        const string Input = "12345678900";
        const string Expected = "123-45-6789";

        // Act
        var result = SSNInput.FormatSSN(Input);

        // Assert
        Assert.Equal(Expected, result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FormatSSN"/> is idempotent —
    /// re-formatting an already formatted SSN returns the same value.
    /// </summary>
    [Fact]
    public void FormatSSN_WhenAlreadyFormatted_ReturnsFormattedValue()
    {
        // Arrange
        const string Input = "123-45-6789";
        const string Expected = "123-45-6789";

        // Act
        var result = SSNInput.FormatSSN(Input);

        // Assert
        Assert.Equal(Expected, result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FormatSSN"/> strips all non-numeric
    /// characters from the input before formatting.
    /// </summary>
    [Fact]
    public void FormatSSN_WhenContainsNonNumericChars_StripsNonDigits()
    {
        // Arrange
        const string Input = "abc123def456ghi789";
        const string Expected = "123-45-6789";

        // Act
        var result = SSNInput.FormatSSN(Input);

        // Assert
        Assert.Equal(Expected, result);
    }

    // =========================================================
    // MaskSSN
    // =========================================================

    /// <summary>
    /// Verifies that <see cref="SSNInput.MaskSSN"/> returns an empty string
    /// when the SSN is <see langword="null"/>, empty, or whitespace.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void MaskSSN_WhenNullOrEmpty_ReturnsEmpty(string? ssn)
    {
        // Arrange — ssn provided via InlineData

        // Act
        var result = SSNInput.MaskSSN(ssn);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.MaskSSN"/> returns the original value
    /// unchanged when the input contains fewer than 4 digits.
    /// </summary>
    [Theory]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("123")]
    public void MaskSSN_WhenFewerThanFourDigits_ReturnsOriginalValue(string ssn)
    {
        // Arrange — ssn provided via InlineData

        // Act
        var result = SSNInput.MaskSSN(ssn);

        // Assert
        Assert.Equal(ssn, result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.MaskSSN"/> applies the "***-**-XXXX"
    /// mask pattern when the input contains exactly 4 digits.
    /// </summary>
    [Fact]
    public void MaskSSN_WhenFourDigits_ShowsLastFourWithMaskPattern()
    {
        // Arrange
        const string SSN = "1234";
        const string Expected = "***-**-1234";

        // Act
        var result = SSNInput.MaskSSN(SSN);

        // Assert
        Assert.Equal(Expected, result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.MaskSSN"/> masks the first 5 digits
    /// and exposes only the last 4, regardless of whether the input is formatted
    /// with dashes or provided as raw digits.
    /// </summary>
    [Theory]
    [InlineData("123456789", "***-**-6789")]
    [InlineData("123-45-6789", "***-**-6789")]
    [InlineData("987654321", "***-**-4321")]
    public void MaskSSN_WhenNineDigits_MasksFirstFiveAndShowsLastFour(string ssn, string expected)
    {
        // Arrange — ssn/Expected provided via InlineData

        // Act
        var result = SSNInput.MaskSSN(ssn);

        // Assert
        Assert.Equal(expected, result);
    }

    // =========================================================
    // FindDuplicateSSNs
    // =========================================================

    /// <summary>
    /// Verifies that <see cref="SSNInput.FindDuplicateSSNs"/> returns an empty
    /// error list when the input list is empty.
    /// </summary>
    [Fact]
    public void FindDuplicateSSNs_WhenEmptyList_ReturnsNoErrors()
    {
        // Arrange
        List<string?> ssns = [];

        // Act
        var result = SSNInput.FindDuplicateSSNs(ssns, i =>
        {
            return $"Employee {i + 1}";
        });

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FindDuplicateSSNs"/> returns an empty
    /// error list when all SSNs in the list are unique.
    /// </summary>
    [Fact]
    public void FindDuplicateSSNs_WhenNoDuplicates_ReturnsNoErrors()
    {
        // Arrange
        List<string?> ssns = ["123-45-6789", "987-65-4321", "111-22-3333"];

        // Act
        var result = SSNInput.FindDuplicateSSNs(ssns, i =>
        {
            return $"Employee {i + 1}";
        });

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FindDuplicateSSNs"/> returns a single
    /// error message that includes both labels when one SSN appears exactly twice.
    /// </summary>
    [Fact]
    public void FindDuplicateSSNs_WhenOneDuplicatePair_ReturnsOneErrorWithBothLabels()
    {
        // Arrange
        List<string?> ssns = ["123-45-6789", "987-65-4321", "123-45-6789"];

        // Act
        var result = SSNInput.FindDuplicateSSNs(ssns, i =>
        {
            return $"Employee {i + 1}";
        });

        // Assert
        Assert.Single(result);
        Assert.Contains("Employee 1", result[0]);
        Assert.Contains("Employee 3", result[0]);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FindDuplicateSSNs"/> returns one error
    /// per duplicate group when multiple distinct SSNs are each duplicated.
    /// </summary>
    [Fact]
    public void FindDuplicateSSNs_WhenMultipleDuplicatePairs_ReturnsOneErrorPerDuplicateGroup()
    {
        // Arrange
        List<string?> ssns = ["123-45-6789", "987-65-4321", "123-45-6789", "987-65-4321"];

        // Act
        var result = SSNInput.FindDuplicateSSNs(ssns, i =>
        {
            return $"Employee {i + 1}";
        });

        // Assert
        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FindDuplicateSSNs"/> returns a single
    /// error message listing all labels when the same SSN appears three or more times.
    /// </summary>
    [Fact]
    public void FindDuplicateSSNs_WhenThreeSameSSNs_ReturnsOneErrorListingAllLabels()
    {
        // Arrange
        List<string?> ssns = ["123-45-6789", "123-45-6789", "123-45-6789"];

        // Act
        var result = SSNInput.FindDuplicateSSNs(ssns, i =>
        {
            return $"Employee {i + 1}";
        });

        // Assert
        Assert.Single(result);
        Assert.Contains("Employee 1", result[0]);
        Assert.Contains("Employee 2", result[0]);
        Assert.Contains("Employee 3", result[0]);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FindDuplicateSSNs"/> skips
    /// <see langword="null"/> entries without throwing or reporting false positives.
    /// </summary>
    [Fact]
    public void FindDuplicateSSNs_WhenListContainsNullEntries_SkipsNulls()
    {
        // Arrange
        List<string?> ssns = [null, "123-45-6789", null, "987-65-4321"];

        // Act
        var result = SSNInput.FindDuplicateSSNs(ssns, i =>
        {
            return $"Employee {i + 1}";
        });

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FindDuplicateSSNs"/> ignores entries
    /// that do not contain exactly 9 digits and does not treat them as duplicates.
    /// </summary>
    [Fact]
    public void FindDuplicateSSNs_WhenListContainsInvalidSSNs_SkipsInvalidEntries()
    {
        // Arrange
        List<string?> ssns = ["123", "456", "123"];

        // Act
        var result = SSNInput.FindDuplicateSSNs(ssns, i =>
        {
            return $"Employee {i + 1}";
        });

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that <see cref="SSNInput.FindDuplicateSSNs"/> correctly detects
    /// a duplicate when one SSN is formatted with dashes and the other is raw digits
    /// representing the same value.
    /// </summary>
    [Fact]
    public void FindDuplicateSSNs_WhenFormattedAndUnformattedSameSSN_DetectsDuplicate()
    {
        // Arrange
        List<string?> ssns = ["123456789", "123-45-6789"];

        // Act
        var result = SSNInput.FindDuplicateSSNs(ssns, i =>
        {
            return $"Employee {i + 1}";
        });

        // Assert
        Assert.Single(result);
    }
}
