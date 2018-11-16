// THIS FILE IS A PART OF EMZI0767'S BOT EXAMPLES
//
// --------
// 
// Copyright 2017 Emzi0767
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//  http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// --------
//
// This is an interactivity example. It shows how to properly utilize 
// Interactivity module.

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace DSPlus.Examples
{
#pragma warning disable IDE1006 // Naming Styles
    public static class NasNative
    {
        [DllImport("hsnxt-nas.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void hello_world();
    }
#pragma warning restore IDE1006 // Naming Styles

    // note that in here we explicitly ask for duration. This is optional,
    // since we set the defaults.
    public class ExampleInteractiveCommands : BaseCommandModule
    {
        [Command("hi"), Description("sup")]
        public async Task Hi(CommandContext ctx)
        {
            NasNative.hello_world();
            await ctx.RespondAsync("meme");
        }
    }
}
