using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Model.Utilities
{
    public class Response
    {
        public Response(HttpStatusCode? statusCode, string errorReason, bool isDeserializeSuccess, bool requestSent, bool isAuthRequest = false, bool userSignedIn = false, bool fromCache = false, IDictionary<string, string[]> validation = null)
        {
            StatusCode = statusCode;
            ErrorReason = errorReason;
            IsDeserializeSuccess = isDeserializeSuccess;
            _requestSent = requestSent;
            _isAuthRequest = isAuthRequest;
            _userSignedIn = userSignedIn;
            _fromCache = fromCache;
            ValidationDictionary = validation;
        }

        public Response(Response response, bool isAuthRequest = false, bool userSignedIn = false) :
            this(response.StatusCode, response.ErrorReason, response.IsDeserializeSuccess, response._requestSent, isAuthRequest, userSignedIn, response._fromCache, response.ValidationDictionary)
        { }

        protected readonly bool _requestSent;
        protected readonly bool _isAuthRequest;
        protected readonly bool _userSignedIn;
        protected readonly bool _fromCache;

        public HttpStatusCode? StatusCode { get; }
        public string ErrorReason { get; }
        public bool IsDeserializeSuccess { get; }
        public bool IsConnectionFailed => _requestSent && StatusCode == null;
        public bool IsSuccess => IsSuccessStatusCode && IsDeserializeSuccess;
        public bool IsUnauthorized => StatusCode == HttpStatusCode.Unauthorized || StatusCode == HttpStatusCode.Forbidden || (_isAuthRequest && !_userSignedIn);
        public bool IsSuccessStatusCode => StatusCode >= HttpStatusCode.OK && StatusCode.Value <= (HttpStatusCode)299;
        public IDictionary<string, string[]> ValidationDictionary { get; }
    }

    public sealed class Response<T> : Response
    {
        public Response(T data, HttpStatusCode? statusCode, string errorReason, bool isDeserializeSuccess, bool requestSent, bool isAuthRequest = false, bool userSignedIn = false, bool fromCache = false, IDictionary<string, string[]> validation = null)
            : base(statusCode, errorReason, isDeserializeSuccess, requestSent, isAuthRequest, userSignedIn, fromCache, validation)
        {
            Data = data;
        }

        public Response(Response<T> response, bool isAuthRequest = false, bool userSignedIn = false) :
            this(response.Data, response.StatusCode, response.ErrorReason, response.IsDeserializeSuccess, response._requestSent, isAuthRequest, userSignedIn, response._fromCache, response.ValidationDictionary)
        {
        }

        public T Data { get; }
    }
}