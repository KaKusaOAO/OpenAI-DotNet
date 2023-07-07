﻿using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Extensions
{
    internal static class ResponseExtensions
    {
        private const string Organization = "Openai-Organization";
        private const string RequestId = "X-Request-ID";
        private const string ProcessingTime = "Openai-Processing-Ms";

        internal static void SetResponseData(this BaseResponse response, HttpResponseHeaders headers)
        {
            if (headers.Contains(Organization))
            {
                response.Organization = headers.GetValues(Organization).FirstOrDefault();
            }

            if (headers.Contains(ProcessingTime))
            {
                response.ProcessingTime = TimeSpan.FromMilliseconds(double.Parse(headers.GetValues(ProcessingTime).First()));
            }

            if (headers.Contains(RequestId))
            {
                response.RequestId = headers.GetValues(RequestId).FirstOrDefault();
            }
        }

        internal static async Task<string> ReadAsStringAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default, [CallerMemberName] string methodName = null)
        {
            var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new OpenAIException(methodName, responseAsString, response);
            }

            return responseAsString;
        }

        internal static async Task CheckResponseAsync(this HttpResponseMessage response, CancellationToken cancellationToken = default, [CallerMemberName] string methodName = null)
        {
            if (!response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                throw new OpenAIException(methodName, responseAsString, response);
            }
        }

        internal static T DeserializeResponse<T>(this HttpResponseMessage response, string json, JsonSerializerOptions settings) where T : BaseResponse
        {
            var result = JsonSerializer.Deserialize<T>(json, settings);
            result.SetResponseData(response.Headers);
            return result;
        }
    }
}
