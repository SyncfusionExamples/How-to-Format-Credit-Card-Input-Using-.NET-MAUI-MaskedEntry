using Syncfusion.Maui.Inputs;
using System.Text.RegularExpressions;
namespace CreditCardInput;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	/// <summary>
    /// Handles cardholder name input changes
    /// </summary>
    private void OnCardholderChanged(object sender, TextChangedEventArgs e)
    {
        string name = e.NewTextValue ?? string.Empty;

        if (string.IsNullOrEmpty(name))
        {
            NameStatusLabel.Text = "○ Name required";
			NameStatusLabel.TextColor = Colors.Gray;
        }
        else if (name.Length < 3)
        {
            NameStatusLabel.Text = "✗ Name too short";
            NameStatusLabel.TextColor = Colors.Red;
        }
        else
        {
            NameStatusLabel.Text = "✓ Valid";
            NameStatusLabel.TextColor = Colors.Green;
        }

        UpdateSubmitButtonState();
    }

    /// <summary>
    /// Validates card number
    /// </summary>
    private bool IsValidCardNumber(string cardNumber)
    {
        // Remove any non-digit characters
        cardNumber = Regex.Replace(cardNumber, @"\D", "");

        // Check length (13-19 digits)
        if (cardNumber.Length < 13 || cardNumber.Length > 19)
            return false;

        // Luhn algorithm
        int sum = 0;
        bool isSecondDigit = false;

        // Process digits from right to left
        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = cardNumber[i] - '0';

            if (isSecondDigit)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            isSecondDigit = !isSecondDigit;
        }

        return sum % 10 == 0;
    }

    /// <summary>
    /// Validates expiry date is in MM/YY format and hasn't expired
    /// </summary>
    private bool IsValidExpiryDate(string expiryDate)
    {
        // Check MM/YY format
        if (!Regex.IsMatch(expiryDate, @"^\d{2}/\d{2}$"))
            return false;

        string[] parts = expiryDate.Split('/');
        if (!int.TryParse(parts[0], out int month) || !int.TryParse(parts[1], out int year))
            return false;

        // Validate month (1-12)
        if (month < 1 || month > 12)
            return false;

        try
        {
            // Convert 2-digit year to 4-digit year (20XX)
            int fullYear = 2000 + year;

            // Create date for the last day of the expiry month
            int daysInMonth = DateTime.DaysInMonth(fullYear, month);
            DateTime expiryDateTime = new DateTime(fullYear, month, daysInMonth);

            // Card is valid if expiry date is today or in the future
            return expiryDateTime >= DateTime.Now;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Updates the submit button state based on form validation
    /// </summary>
    private void UpdateSubmitButtonState()
    {
        bool cardValid = CardStatusLabel.TextColor == Colors.Green;
        bool expiryValid = ExpiryStatusLabel.TextColor == Colors.Green;
        bool cvvValid = CVVStatusLabel.TextColor == Colors.Green;
        bool nameValid = NameStatusLabel.TextColor == Colors.Green;

        bool formIsValid = cardValid && expiryValid && cvvValid && nameValid;

        SubmitButton.IsEnabled = formIsValid;
        SubmitButton.Opacity = formIsValid ? 1.0 : 0.6;
        SubmitButton.BackgroundColor = formIsValid ? 
            Color.FromArgb("#007AFF") : 
            Color.FromArgb("#CCCCCC");
    }

    /// <summary>
    /// Processes payment when submit button is clicked
    /// </summary>
    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        string cardNumber = CardNumberEntry.Text?.Replace(" ", "") ?? "";
        string expiry = ExpiryEntry.Text ?? "";
        string cvv = CVVEntry.Text?.Replace(" ", "") ?? "";
        string name = CardholderEntry.Text ?? "";

        // Display success message with masked card number
        string maskedCard = $"****{cardNumber.Substring(Math.Max(0, cardNumber.Length - 4))}";

        await DisplayAlert(
            "Payment Submitted",
            $"Card: {maskedCard}\nExpiry: {expiry}\nName: {name}\n\n Payment processed successfully!",
            "OK");

        // In a real application, you would send this data to your payment processor
        // Make sure to use HTTPS and never log sensitive card data
    }

    /// <summary>
    /// Handles card number input changes and validates card
    /// </summary>
    private void CardNumberEntry_ValueChanged(object sender, MaskedEntryValueChangedEventArgs e)
    {
        string cardNumber = GetDigits(e.NewValue);

        // Keep the mask stable while typing so the control does not reset its text.
        const int expectedCardLength = 16;
        bool isCardComplete = cardNumber.Length == expectedCardLength;

        // Update UI
        if (string.IsNullOrEmpty(cardNumber))
        {
			CardStatusLabel.Text = "○ Card number required";
			CardStatusLabel.TextColor = Colors.Gray;
        }
        else if (!isCardComplete)
        {
			CardStatusLabel.Text = $"Enter all {expectedCardLength} digits";
			CardStatusLabel.TextColor = Colors.Gray;
        }
        else if (isCardComplete)
        {
			CardStatusLabel.Text = "✓ Valid Card";
			CardStatusLabel.TextColor = Colors.Green;
        }
        else
        {
			CardStatusLabel.Text = "✗ Invalid Card";
			CardStatusLabel.TextColor = Colors.Red;
        }

        UpdateSubmitButtonState();
    }

    /// <summary>
    /// Handles expiry date input changes
    /// </summary>
    private void ExpiryEntry_ValueChanged(object sender, MaskedEntryValueChangedEventArgs e)
    {
        string expiryDigits = GetDigits(e.NewValue);
        string expiry = expiryDigits.Length == 4
            ? $"{expiryDigits[..2]}/{expiryDigits[2..]}"
            : expiryDigits;

        if (string.IsNullOrEmpty(expiry))
        {
            ExpiryStatusLabel.Text = "○ Expiry date required";
            ExpiryStatusLabel.TextColor = Colors.Gray;
        }
        else if (expiry.Length < 5)
        {
            ExpiryStatusLabel.Text = $"Incomplete ({expiry.Length}/5)";
            ExpiryStatusLabel.TextColor = Colors.Gray;
        }
        else
        {
            bool isValid = IsValidExpiryDate(expiry);
            ExpiryStatusLabel.Text = isValid ? "✓ Valid" : "✗ Expired or invalid date";
            ExpiryStatusLabel.TextColor = isValid ? Colors.Green : Colors.Red;
        }

        UpdateSubmitButtonState();
    }

    /// <summary>
    /// Handles CVV input changes
    /// </summary>
    private void CVVEntry_ValueChanged(object sender, MaskedEntryValueChangedEventArgs e)
    {
        string cvv = GetDigits(e.NewValue);
        int expectedLength = 3;

        if (string.IsNullOrEmpty(cvv))
        {
            CVVStatusLabel.Text = "○ CVV required";
            CVVStatusLabel.TextColor = Colors.Gray;
        }
        else if (cvv.Length < expectedLength)
        {
            CVVStatusLabel.Text = $"Incomplete ({cvv.Length}/{expectedLength})";
            CVVStatusLabel.TextColor = Colors.Gray;
        }
        else
        {
            CVVStatusLabel.Text = "✓ Valid";
            CVVStatusLabel.TextColor = Colors.Green;
        }

        UpdateSubmitButtonState();
    }

    private static string GetDigits(object? value)
    {
        return Regex.Replace(value?.ToString() ?? string.Empty, @"\D", "");
    }
}
