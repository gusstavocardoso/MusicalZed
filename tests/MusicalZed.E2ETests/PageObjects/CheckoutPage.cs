namespace MusicalZed.E2ETests.PageObjects;

using Microsoft.Playwright;

public class CheckoutPage : BasePage
{
    private const string NameInput = "input[placeholder='Seu nome completo']";
    private const string EmailInput = "input[type='email']";
    private const string PhoneInput = "input[placeholder='(11) 99999-9999']";
    private const string AddressInput = "input[placeholder*='Rua']";
    private const string CityInput = "input[placeholder='Cidade']";
    private const string StateSelect = "select";
    private const string ZipInput = "input[placeholder='00000-000']";
    private const string PixPaymentBtn = "label:has-text('PIX')";
    private const string ConfirmBtn = ".btn-mz:has-text('Confirmar')";
    private const string ErrorAlert = ".alert-danger";

    public CheckoutPage(IPage page, string baseUrl) : base(page, baseUrl) { }

    public async Task GoToAsync() => await NavigateAsync("/checkout");

    public async Task FillCustomerDataAsync(string name, string email, string phone)
    {
        await Page.Locator(NameInput).FillAsync(name);
        await Page.Locator(EmailInput).FillAsync(email);
        await Page.Locator(PhoneInput).FillAsync(phone);
    }

    public async Task FillAddressAsync(string address, string city, string state, string zip)
    {
        await Page.Locator(AddressInput).FillAsync(address);
        await Page.Locator(CityInput).FillAsync(city);
        await Page.Locator(StateSelect).SelectOptionAsync(new SelectOptionValue { Label = state });
        await Page.Locator(ZipInput).FillAsync(zip);
    }

    public async Task SelectPixPaymentAsync() =>
        await Page.Locator(PixPaymentBtn).ClickAsync();

    public async Task ClickConfirmAsync() =>
        await Page.Locator(ConfirmBtn).ClickAsync();

    public async Task<bool> IsErrorVisibleAsync() =>
        await IsElementVisibleAsync(ErrorAlert);

    public async Task<string?> GetErrorMessageAsync()
    {
        if (!await IsErrorVisibleAsync()) return null;
        return await Page.Locator(ErrorAlert).InnerTextAsync();
    }
}
