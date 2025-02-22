﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Ydb.Sdk.Client
{
    public interface IResponse
    {
        Status Status { get; }
    }
    public class ResponseBase : IResponse
    {
        public Status Status { get; } = new Status(StatusCode.Unspecified);

        protected ResponseBase(Status status)
        {
            Status = status;
        }

        public void EnsureSuccess()
        {
            Status.EnsureSuccess();
        }
    }

    public class ClientInternalErrorResponse : ResponseBase
    {
        public ClientInternalErrorResponse(string message)
            : base(new Status(StatusCode.ClientInternalError, message))
        {
        }
    }

    public class ResponseWithResultBase<TResult> : ResponseBase
        where TResult : class
    {
        private readonly TResult? _result;

        protected ResponseWithResultBase(Status status)
            : base(status)
        {

        }

        protected ResponseWithResultBase(Status status, TResult? result)
            : base(status)
        {
            if (result != null)
            {
                EnsureSuccess();
            }

            _result = result;
        }
        public TResult Result
        {
            get
            {
                EnsureSuccess();
                Debug.Assert(_result != null);
                return _result;
            }
        }
    }

    public abstract class StreamResponse<TProtoResponse, TResponse>
        where TProtoResponse : class
        where TResponse : class
    {
        private Driver.StreamIterator<TProtoResponse> _iterator;
        private TResponse? _response;
        private bool _transportError = false;

        internal StreamResponse(Driver.StreamIterator<TProtoResponse> iterator)
        {
            _iterator = iterator;
        }

        public TResponse Response
        {
            get
            {
                if (_response is null)
                {
                    throw new InvalidOperationException("Stream response not available before first Next() call.");
                }

                return _response;
            }
        }

        public async Task<bool> Next()
        {
            if (_transportError)
            {
                return false;
            }

            try
            {
                bool result = await _iterator.Next();
                if (result)
                {
                    _response = MakeResponse(_iterator.Response);
                }

                return result;
            }
            catch (Driver.TransportException e)
            {
                _response = MakeResponse(e.Status);
                _transportError = true;
                return true;
            }
        }

        protected abstract TResponse MakeResponse(TProtoResponse protoResponse);
        protected abstract TResponse MakeResponse(Status status);
    }

    public sealed class EmptyResult
    {
    }

    public sealed class EmptyMetadata
    {
    }

    public abstract class OperationResponse<TResult, TMetadata> : IClientOperation
        where TResult : class
        where TMetadata : class
    {
        private TResult? _result;
        private TMetadata? _metadata;

        public string Id { get; }

        public bool IsReady { get; }

        public Status Status { get; }

        public bool HasResult
        {
            get { return IsReady && _result != null; }
        }

        public TResult Result
        {
            get
            {
                if (_result is null)
                {
                    throw new OperationException(Id, "Operation result unavailable.");
                }

                return _result;
            }
        }

        public bool HasMetadata
        {
            get { return _metadata != null; }
        }

        public TMetadata Metadata
        {
            get
            {
                if (_metadata is null)
                {
                    throw new OperationException(Id, "Operation metadata unavailable.");
                }

                return _metadata;
            }
        }

        protected OperationResponse(ClientOperation operation)
        {
            Id = operation.Id;
            IsReady = operation.IsReady;
            Status = operation.Status;

            if (operation.HasResult)
            {
                _result = UnpackResult(operation);
            }

            if (operation.HasMetadata)
            {
                _metadata = UnpackMetadata(operation);
            }
        }

        protected OperationResponse(Status status)
            : this(new ClientOperation(status))
        {
        }

        protected abstract TResult UnpackResult(ClientOperation operation);
        protected abstract TMetadata UnpackMetadata(ClientOperation operation);

    }
}
