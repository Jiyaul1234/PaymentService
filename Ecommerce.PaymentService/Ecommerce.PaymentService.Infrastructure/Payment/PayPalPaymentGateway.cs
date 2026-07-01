using Ecommerce.PaymentService.Application.Interface;
using Ecommerce.PaymentService.Application.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using Microsoft.Extensions.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ecommerce.PaymentService.Infrastructure.Payment
{
    /// <summary>
    /// Minimal PayPal integration using REST API (OAuth2 + Orders/Capture).
    /// Configuration expected in configuration:
    /// PayPal:ClientId, PayPal:Secret, PayPal:Environment (Sandbox|Live)
    /// </summary>
    public class PayPalPaymentGateway : IPaymentGateway
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _clientId;
        private readonly string _secret;
        private readonly string _baseUrl;

        public PayPalPaymentGateway(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _clientId = configuration["PayPal:ClientId"] ?? string.Empty;
            _secret = configuration["PayPal:Secret"] ?? string.Empty;
            var env = configuration["PayPal:Environment"] ?? "Sandbox";
            _baseUrl = env.Equals("Live", StringComparison.OrdinalIgnoreCase)
                ? "https://api-m.paypal.com"
                : "https://api-m.sandbox.paypal.com";
        }

        public async Task<PaymentResult> PayAsync(PaymentRequest request)
        {
            try
            {
                var token = await GetAccessTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return new PaymentResult { IsSuccess = false, ErrorMessage = "Failed to obtain PayPal access token" };
                }

                var createOrderResponse = await CreateAndCaptureOrderAsync(request, token);
                return createOrderResponse;
            }
            catch (Exception ex)
            {
                return new PaymentResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }

        private async Task<string?> GetAccessTokenAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_secret}"));
            var req = new HttpRequestMessage(HttpMethod.Post, _baseUrl + "/v1/oauth2/token");
            req.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
            req.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

            var res = await client.SendAsync(req);
            if (!res.IsSuccessStatusCode) return null;
            using var stream = await res.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            if (doc.RootElement.TryGetProperty("access_token", out var tokenEl))
            {
                return tokenEl.GetString();
            }

            return null;
        }

        private async Task<PaymentResult> CreateAndCaptureOrderAsync(PaymentRequest request, string accessToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var orderPayload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = request.Currency ?? "USD",
                            value = request.Amount.ToString("F2")
                        }
                    }
                }
            };

            var createContent = new StringContent(JsonSerializer.Serialize(orderPayload), Encoding.UTF8, "application/json");
            var createRes = await client.PostAsync(_baseUrl + "/v2/checkout/orders", createContent);
            if (!createRes.IsSuccessStatusCode)
            {
                var err = await createRes.Content.ReadAsStringAsync();
                return new PaymentResult { IsSuccess = false, ErrorMessage = $"Create order failed: {err}" };
            }

            using var createStream = await createRes.Content.ReadAsStreamAsync();
            using var createDoc = await JsonDocument.ParseAsync(createStream);
            if (!createDoc.RootElement.TryGetProperty("id", out var orderIdEl))
            {
                return new PaymentResult { IsSuccess = false, ErrorMessage = "Create order response missing id" };
            }

            var orderId = orderIdEl.GetString();
            if (string.IsNullOrEmpty(orderId))
            {
                return new PaymentResult { IsSuccess = false, ErrorMessage = "Empty order id" };
            }

            // Capture the order
            var captureRes = await client.PostAsync($"{_baseUrl}/v2/checkout/orders/{orderId}/capture", null);
            if (!captureRes.IsSuccessStatusCode)
            {
                var err = await captureRes.Content.ReadAsStringAsync();
                return new PaymentResult { IsSuccess = false, ErrorMessage = $"Capture failed: {err}" };
            }

            using var capStream = await captureRes.Content.ReadAsStreamAsync();
            using var capDoc = await JsonDocument.ParseAsync(capStream);

            // try to extract capture id
            try
            {
                var pu = capDoc.RootElement.GetProperty("purchase_units")[0];
                var payments = pu.GetProperty("payments");
                var captures = payments.GetProperty("captures");
                var captureId = captures[0].GetProperty("id").GetString();

                return new PaymentResult
                {
                    IsSuccess = true,
                    TransactionId = captureId ?? orderId
                };
            }
            catch
            {
                return new PaymentResult { IsSuccess = true, TransactionId = orderId };
            }
        }
    }
}
