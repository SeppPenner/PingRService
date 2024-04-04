#pragma warning disable IDE0065 // Die using-Anweisung wurde falsch platziert.
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Net;
global using System.Net.Security;
global using System.Reflection;
global using System.Security.Cryptography.X509Certificates;
global using System.Text.Json.Serialization;

global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;

global using PingRService.Configuration;
global using PingRService.Constants;
global using PingRService.Exceptions;
global using PingRService.Extensions;

global using Serilog;
global using Serilog.Context;
global using Serilog.Core;
global using Serilog.Events;
global using Serilog.Exceptions;

global using ILogger = Serilog.ILogger;
#pragma warning restore IDE0065 // Die using-Anweisung wurde falsch platziert.