﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Ydb.Sdk.Client;

namespace Ydb.Sdk.Operations
{
    public partial class OperationsClient
    {
        public async Task<ClientOperation> PollReady(
            string id, 
            TimeSpan? delay = default,
            CancellationToken cancellationToken = default)
        {
            delay ??= TimeSpan.FromSeconds(10);

            while (true)
            {
                var operation = await GetOperation(id);
                if (operation.IsReady)
                {
                    return operation;
                }

                await Task.Delay(delay.Value, cancellationToken);
            }
        }
    }
}