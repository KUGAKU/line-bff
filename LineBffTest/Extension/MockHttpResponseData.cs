﻿using System;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace LineBffTest.Extension
{
    public sealed class MockHttpResponseData : HttpResponseData
    {

        public MockHttpResponseData(FunctionContext context) : base(context)
        { }

        public override HttpStatusCode StatusCode { get; set; }
        public override HttpHeadersCollection Headers { get; set; } = new HttpHeadersCollection();
        public override Stream Body { get; set; } = new MemoryStream();
        public override HttpCookies Cookies { get; }
    }
}

