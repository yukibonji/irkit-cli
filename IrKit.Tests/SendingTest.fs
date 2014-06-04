﻿namespace IrKit.Tests

open System
open NUnit.Framework
open FsUnit
open Foq
open System.Net.Http
open IrKit

[<TestFixture>]
type SendingTest () =

  [<TestCase("192.168.1.200")>]
  [<TestCase("192.168.1.201")>]
  member test.``should request a msg when sending the msg to the device by wifi.`` ip =
    let expectedReq = fun (req:HttpRequestMessage) -> 
      req.Method = HttpMethod.Post
      && req.RequestUri = Uri(sprintf "http://%s/messages" ip)

    let http : HttpMessageInvoker = Mock.With(fun h -> 
      <@
        h.SendAsync(is(expectedReq), any()) --> null
      @>)

    { Frequency = 40; Data = [] }
    |> send http (Wifi ip)
    |> Async.RunSynchronously

    verify <@ http.SendAsync(is(expectedReq), any()) @> once