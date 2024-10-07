﻿global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using System.Diagnostics.CodeAnalysis;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Buffers;
global using System.Runtime.CompilerServices;
global using System.Numerics;
global using System.Reflection;
global using System.Diagnostics;
global using System.Diagnostics.Metrics;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Configuration;
global using Microsoft.EntityFrameworkCore;

global using LemonKit.Settings;
global using LemonKit.Settings.Attributes;
global using LemonKit.Settings.Builders;
global using LemonKit.Settings.Extensions;
global using LemonKit.Settings.Providers;

global using LemonKit.Processors.Attributes;
global using LemonKit.Processors;
global using LemonKit.Processors.Apis;

global using LemonKit.Validation;
global using LemonKit.Validation.Attributes;

global using LemonKit.Observe.Attributes;
global using LemonKit.Observe;

global using LemonKit.Results;

global using LemonKit.Services.Attributes;

global using LemonKit.Extensions;

global using LemonKit.SimpleDemo.Models;
global using LemonKit.SimpleDemo.Settings;
global using LemonKit.SimpleDemo.Validation;
global using LemonKit.SimpleDemo.Extensions;

global using LemonKit.SimpleDemo.Database;
global using LemonKit.SimpleDemo.Database.Models;
global using LemonKit.SimpleDemo.Database.Concrete.Pets;
global using LemonKit.SimpleDemo.Database.Interfaces.Pets;