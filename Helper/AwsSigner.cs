using System.Security.Cryptography;
using System.Text;

namespace SimpleDrive.Helpers;

public class AwsSigner
{
    private readonly string _accessKey;
    private readonly string _secretKey;
    private readonly string _region;
    private readonly string _service;

    public AwsSigner(string accessKey, string secretKey, string region, string service = "s3")
    {
        _accessKey = accessKey;
        _secretKey = secretKey;
        _region = region;
        _service = service;
    }

    public void SignRequest(HttpRequestMessage request, string method, string canonicalUri, byte[] payload)
    {
        var now = DateTime.UtcNow;
        var amzDate = now.ToString("yyyyMMddTHHmmssZ");
        var dateStamp = now.ToString("yyyyMMdd");

        string payloadHash = Hash(payload);
        request.Headers.Add("x-amz-content-sha256", payloadHash);
        request.Headers.Add("x-amz-date", amzDate);
        request.Headers.Host = request.RequestUri!.Host;

        string canonicalHeaders = $"host:{request.RequestUri.Host}\n" +
                                  $"x-amz-content-sha256:{payloadHash}\n" +
                                  $"x-amz-date:{amzDate}\n";
        string signedHeaders = "host;x-amz-content-sha256;x-amz-date";

        string canonicalRequest = $"{method}\n{canonicalUri}\n\n{canonicalHeaders}\n{signedHeaders}\n{payloadHash}";
        string hashedCanonicalRequest = Hash(Encoding.UTF8.GetBytes(canonicalRequest));

        string credentialScope = $"{dateStamp}/{_region}/{_service}/aws4_request";
        string stringToSign = $"AWS4-HMAC-SHA256\n{amzDate}\n{credentialScope}\n{hashedCanonicalRequest}";

        byte[] signingKey = GetSignatureKey(_secretKey, dateStamp, _region, _service);
        string signature = ToHexString(HmacSHA256(signingKey, stringToSign));

        string authorizationHeader = $"AWS4-HMAC-SHA256 Credential={_accessKey}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";
        request.Headers.TryAddWithoutValidation("Authorization", authorizationHeader);
    }

    private static string Hash(byte[] data)
    {
        using var sha256 = SHA256.Create();
        return ToHexString(sha256.ComputeHash(data));
    }

    private static byte[] HmacSHA256(byte[] key, string data)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
    }

    private static byte[] GetSignatureKey(string key, string date, string region, string service)
    {
        byte[] kDate = HmacSHA256(Encoding.UTF8.GetBytes("AWS4" + key), date);
        byte[] kRegion = HmacSHA256(kDate, region);
        byte[] kService = HmacSHA256(kRegion, service);
        return HmacSHA256(kService, "aws4_request");
    }

    private static string ToHexString(byte[] bytes) =>
        BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
}
