// Code generated by Microsoft (R) AutoRest Code Generator 1.0.1.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Lykke.Service.EmailFormatter.AutorestClient
{
    using Lykke.Service;
    using Lykke.Service.EmailFormatter;
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for EmailFormattingService.
    /// </summary>
    internal static partial class EmailFormattingServiceExtensions
    {
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='caseId'>
            /// </param>
            /// <param name='partnerId'>
            /// </param>
            /// <param name='language'>
            /// </param>
            /// <param name='parameters'>
            /// </param>
            public static EmailFormatResponse ApiFormatterByCaseIdByPartnerIdByLanguagePost(this IEmailFormattingService operations, string caseId, string partnerId, string language, IDictionary<string, string> parameters = default(IDictionary<string, string>))
            {
                return operations.ApiFormatterByCaseIdByPartnerIdByLanguagePostAsync(caseId, partnerId, language, parameters).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='caseId'>
            /// </param>
            /// <param name='partnerId'>
            /// </param>
            /// <param name='language'>
            /// </param>
            /// <param name='parameters'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<EmailFormatResponse> ApiFormatterByCaseIdByPartnerIdByLanguagePostAsync(this IEmailFormattingService operations, string caseId, string partnerId, string language, IDictionary<string, string> parameters = default(IDictionary<string, string>), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.ApiFormatterByCaseIdByPartnerIdByLanguagePostWithHttpMessagesAsync(caseId, partnerId, language, parameters, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static void ApiIsAliveGet(this IEmailFormattingService operations)
            {
                operations.ApiIsAliveGetAsync().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task ApiIsAliveGetAsync(this IEmailFormattingService operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.ApiIsAliveGetWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

    }
}
