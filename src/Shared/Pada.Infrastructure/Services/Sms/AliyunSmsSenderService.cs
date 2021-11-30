using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EasyCaching.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pada.Abstractions.Services;
using Pada.Abstractions.Services.Sms;
using Pada.Infrastructure.Caching;
using Pada.Infrastructure.Services.Sms.Models;

namespace Pada.Infrastructure.Services.Sms
{
    public class AliyunSmsSenderService : ISmsSender
    {
        const string SEPARATOR = "&";

        private string RegisterPhonePrefix = "pada:register:phone:";

        private readonly int timeoutInMilliSeconds = 100000;
        private readonly string version = "2017-05-25";
        private readonly string action = "SendSms";
        private readonly string format = "JSON";
        private readonly string domain = "dysmsapi.aliyuncs.com";
        private readonly string regionId;
        private readonly string accessKeyId;
        private readonly string accessKeySecret;
        private readonly bool isTest;
        private readonly string templateCode;
        private readonly string signName;

        private readonly ILogger<AliyunSmsSenderService> _logger;
        private readonly IEasyCachingProvider _cachingProvider;

        public AliyunSmsSenderService(ILogger<AliyunSmsSenderService> logger,
            IOptionsMonitor<AliyunSmsOptions> options,
            IEasyCachingProvider cachingProvider)
        {
            _logger = logger;
            _cachingProvider = cachingProvider;

            regionId = options.CurrentValue.RegionId;
            accessKeyId = options.CurrentValue.AccessKeyId;
            accessKeySecret = options.CurrentValue.AccessKeySecret;
            isTest = options.CurrentValue.IsTest;
            templateCode = options.CurrentValue.TemplateCode;
            signName = options.CurrentValue.SignName;
        }

        public async Task<bool> SendSmsAsync(SmsSend model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model));

                if (isTest || model.IsTest)
                {
                    model.IsTest = true;
                    model.IsSucceed = true;
                    return true;
                }

                var paramers = new Dictionary<string, string>();
                paramers.Add("PhoneNumbers", model.PhoneNumber);
                paramers.Add("SignName", model.SignName);
                paramers.Add("TemplateCode", model.TemplateCode);
                paramers.Add("TemplateParam", model.TemplateParam);
                paramers.Add("AccessKeyId", accessKeyId);

                var url = GetSignUrl(paramers, accessKeySecret);
                var result = await HttpGetAsync(url);
                if (result.StatusCode == 200 && !string.IsNullOrEmpty(result.Response))
                {
                    var message = JsonConvert.DeserializeObject<AliyunSendSmsResult>(result.Response);
                    if (message?.Code == "OK")
                    {
                        model.IsSucceed = true;
                        model.Message = message.Code;
                        model.ReceiptId = message.BizId;
                        return true;
                    }
                    else if (message != null)
                    {
                        //smsRecord.
                        model.Message = message.Message;
                    }
                    else
                    {
                        model.Message = result.Response;
                    }
                }
                else
                {
                    model.Message = "Send Sms fail. ";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"send sms error: {ex.Message}", model);
                if (model != null)
                {
                    model.Message = ex.Message;
                }
            }
            finally
            {
                if (model != null)
                {
                    _logger.LogDebug($"sms: {JsonConvert.SerializeObject(model)}");
                    // _smsSendRepository.Add(model);
                    // await _smsSendRepository.SaveChangesAsync();
                }
            }

            return false;
        }

        public async Task<(bool Success, string Message)> SendCaptchaAsync(string phone, string captcha)
        {
            if (string.IsNullOrWhiteSpace(captcha))
                throw new ArgumentNullException(nameof(captcha));
            captcha = captcha.Trim();

            var cacheKey = CacheKey.With(RegisterPhonePrefix, phone);

            var result = await _cachingProvider.GetAsync(cacheKey, async () =>
            {
                var code = captcha;
                var success = await SendSmsAsync(new SmsSend(new Guid())
                {
                    PhoneNumber = phone,
                    Value = code,
                    TemplateType = SmsTemplateType.Captcha,
                    TemplateCode = templateCode,
                    SignName = signName,
                    TemplateParam = JsonConvert.SerializeObject(new {code}),
                });

                if (success)
                    return (true, "send successfully.");
                return (false, "request is on going，try it later.");
            }, TimeSpan.FromMinutes(1));

            return result.Value;
        }

        private async Task<(int StatusCode, string Response)> HttpGetAsync(string url)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.Proxy = null;
            handler.AutomaticDecompression = DecompressionMethods.GZip;
            using (var http = new HttpClient(handler))
            {
                http.Timeout = new TimeSpan(TimeSpan.TicksPerMillisecond * timeoutInMilliSeconds);
                HttpResponseMessage response = await http.GetAsync(url);
                return ((int) response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }

        private static string FormatIso8601Date(DateTime date)
        {
            return date.ToUniversalTime()
                .ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.CreateSpecificCulture("en-US"));
        }

        private static string PercentEncode(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int) c));
                }
            }

            return stringBuilder.ToString();
        }

        private static string ConcatQueryString(Dictionary<string, string> parameters)
        {
            if (null == parameters)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            foreach (var entry in parameters)
            {
                string key = entry.Key;
                string val = entry.Value;
                sb.Append(HttpUtility.UrlEncode(key, Encoding.UTF8));
                if (val != null)
                {
                    sb.Append("=").Append(HttpUtility.UrlEncode(val, Encoding.UTF8));
                }

                sb.Append("&");
            }

            int strIndex = sb.Length;
            if (parameters.Count > 0)
                sb.Remove(strIndex - 1, 1);
            return sb.ToString();
        }

        private static string ComposeUrl(string endpoint, Dictionary<string, string> parameters)
        {
            StringBuilder urlBuilder = new StringBuilder("");
            urlBuilder.Append("http://").Append(endpoint);
            if (urlBuilder.ToString().IndexOf("?") == -1)
            {
                urlBuilder.Append("/?");
            }

            string query = ConcatQueryString(parameters);
            return urlBuilder.Append(query).ToString();
        }

        private static string SignString(string source, string accessSecret)
        {
            using (var algorithm = new HMACSHA1(Encoding.UTF8.GetBytes(accessSecret.ToCharArray())))
            {
                return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(source.ToCharArray())));
            }
        }

        private string GetSignUrl(Dictionary<string, string> parameters, string accessSecret)
        {
            var imutableMap = new Dictionary<string, string>(parameters);
            imutableMap.Add("Timestamp", FormatIso8601Date(DateTime.Now));
            imutableMap.Add("SignatureMethod", "HMAC-SHA1");
            imutableMap.Add("SignatureVersion", "1.0");
            imutableMap.Add("SignatureNonce", Guid.NewGuid().ToString());
            imutableMap.Add("Action", action);
            imutableMap.Add("Version", version);
            imutableMap.Add("Format", format);
            imutableMap.Add("RegionId", regionId);

            IDictionary<string, string> sortedDictionary =
                new SortedDictionary<string, string>(imutableMap, StringComparer.Ordinal);
            StringBuilder canonicalizedQueryString = new StringBuilder();
            foreach (var p in sortedDictionary)
            {
                canonicalizedQueryString
                    .Append("&")
                    .Append(PercentEncode(p.Key)).Append("=")
                    .Append(PercentEncode(p.Value));
            }

            StringBuilder stringToSign = new StringBuilder();
            stringToSign.Append("GET");
            stringToSign.Append(SEPARATOR);
            stringToSign.Append(PercentEncode("/"));
            stringToSign.Append(SEPARATOR);
            stringToSign.Append(PercentEncode(canonicalizedQueryString.ToString().Substring(1)));

            string signature = SignString(stringToSign.ToString(), accessSecret + "&");

            imutableMap.Add("Signature", signature);

            return ComposeUrl(domain, imutableMap);
        }
    }
}