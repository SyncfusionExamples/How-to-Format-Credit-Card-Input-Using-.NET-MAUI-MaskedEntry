# How-to-Format-Credit-Card-Input-Using-.NET-MAUI-MaskedEntry
This section explains how to format and validate credit card input in .NET MAUI using the SfMaskedEntry control. By applying masks for the card number, expiry date, and CVV fields, you can ensure that users enter data in the correct format while providing a secure and user-friendly payment form experience.

## XAML

Define the SfMaskedEntry controls with appropriate masks to automatically format the credit card number, expiry date, and CVV fields as users enter data.
```xml
<VerticalStackLayout Padding="20"
                     Spacing="20">

            <!-- Header -->
            <VerticalStackLayout Spacing="5">
                <Label Text="Payment Details"
                       FontSize="24"
                       FontAttributes="Bold" />
                <Label Text="Enter your card information securely"
                       FontSize="12" />
            </VerticalStackLayout>

            <!-- Card Number -->
            <VerticalStackLayout Spacing="8">
                <Label Text="Card Number"
                       FontSize="14"
                       FontAttributes="Bold" />

                <syncfusion:SfMaskedEntry x:Name="CardNumberEntry"
                                          Mask="0000 0000 0000 0000"
                                          Placeholder="Enter card number"
                                          PromptChar="_"
                                          FontSize="16"
                                          ValueChanged="CardNumberEntry_ValueChanged" />

                <Label x:Name="CardStatusLabel"
                       FontSize="11" />
            </VerticalStackLayout>

            <!-- Expiry and CVV -->
            <Grid ColumnDefinitions="*,*"
                  ColumnSpacing="15"
                  RowDefinitions="Auto,52,20">

                <Label Grid.Column="0"
                       Text="Expiry Date"
                       FontSize="14"
                       FontAttributes="Bold" />

                <Label Grid.Column="1"
                       Text="CVV"
                       FontSize="14"
                       FontAttributes="Bold" />

                <syncfusion:SfMaskedEntry Grid.Row="1"
                                          Grid.Column="0"
                                          x:Name="ExpiryEntry"
                                          Mask="00/00"
                                          Placeholder="MM/YY"
                                          PromptChar="_"
                                          WidthRequest="58"
                                          FontSize="16"
                                          ValueChanged="ExpiryEntry_ValueChanged" />

                <syncfusion:SfMaskedEntry Grid.Row="1"
                                          Grid.Column="1"
                                          x:Name="CVVEntry"
                                          Mask="000"
                                          Placeholder="123"
                                          PromptChar="_"
                                          WidthRequest="45"
                                          FontSize="16"
                                          ValueChanged="CVVEntry_ValueChanged" />

                <Label Grid.Row="2"
                       Grid.Column="0"
                       x:Name="ExpiryStatusLabel"
                       FontSize="11" />

                <Label Grid.Row="2"
                       Grid.Column="1"
                       x:Name="CVVStatusLabel"
                       FontSize="11" />
            </Grid>

            <!-- Cardholder Name -->
            <VerticalStackLayout Spacing="8">
                <Label Text="Cardholder Name"
                       FontSize="14"
                       FontAttributes="Bold" />

                <Entry x:Name="CardholderEntry"
                       Placeholder="Full name on card"
                       FontSize="16"
                       TextChanged="OnCardholderChanged" />

                <Label x:Name="NameStatusLabel"
                       FontSize="11" />
            </VerticalStackLayout>

            <Button x:Name="SubmitButton"
                    Text="Process Payment"
                    IsEnabled="False"
                    Opacity="0.6"
                    TextColor="White"
                    BackgroundColor="#007AFF"
                    Clicked="OnSubmitClicked" />

</VerticalStackLayout> 

```

## C#

Handle the ValueChanged event to validate the entered values and provide immediate feedback to users. The card number is formatted using a mask, the expiry date is validated against the current date, and the CVV length is verified before enabling the payment button.

```csharp
using System.Text.RegularExpressions;

    private void CardNumberEntry_ValueChanged(object sender, MaskedEntryValueChangedEventArgs e)
    {
        string cardNumber = GetDigits(e.NewValue);

        const int expectedCardLength = 16;
        bool isCardComplete = cardNumber.Length == expectedCardLength;

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
        else
        {
            CardStatusLabel.Text = "✓ Valid Card";
            CardStatusLabel.TextColor = Colors.Green;
        }

        UpdateSubmitButtonState();
    }

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

            ExpiryStatusLabel.Text = isValid
                ? "✓ Valid"
                : "✗ Expired or invalid date";

            ExpiryStatusLabel.TextColor = isValid
                ? Colors.Green
                : Colors.Red;
        }

        UpdateSubmitButtonState();
    }

    private void CVVEntry_ValueChanged(object sender, MaskedEntryValueChangedEventArgs e)
    {
        string cvv = GetDigits(e.NewValue);

        if (string.IsNullOrEmpty(cvv))
        {
            CVVStatusLabel.Text = "○ CVV required";
            CVVStatusLabel.TextColor = Colors.Gray;
        }
        else if (cvv.Length < 3)
        {
            CVVStatusLabel.Text = $"Incomplete ({cvv.Length}/3)";
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
        return Regex.Replace(
            value?.ToString() ?? string.Empty,
            @"\D",
            "");
    } 

```
The SfMaskedEntry control automatically formats the credit card number as XXXX XXXX XXXX XXXX, enforces the MM/YY pattern for expiry dates, and restricts CVV input to three digits, helping users enter payment information accurately and consistently.