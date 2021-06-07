﻿/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using SK.Libretro.Utilities;
using System;
using System.Text.RegularExpressions;
using static SK.Libretro.LibretroHeader;

namespace SK.Libretro
{
    internal static class LibretroLog
    {
        private const string LOG_PRINTF_CALLER = "Libretro.Wrapper.RetroLogPrintf";

        private static readonly Regex _argumentsRegex = new Regex(@"%(?:\d+\$)?[+-]?(?:[ 0]|'.{1})?-?\d*(?:\.\d+)?([bcdeEufFgGosxX])", RegexOptions.Compiled);

        public static void RetroLogPrintf(retro_log_level level, string format, IntPtr arg1, IntPtr arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5, IntPtr arg6, IntPtr arg7, IntPtr arg8, IntPtr arg9, IntPtr arg10, IntPtr arg11, IntPtr arg12)
        {
            if (level < LibretroWrapper.LogLevel)
                return;

            int argumentsToPush;
            try
            {
                argumentsToPush = GetFormatArgumentCount(format);
            }
            catch (NotImplementedException /*e*/)
            {
                //Log.Warning(e.Message);
                return;
            }

            if (argumentsToPush > 12)
            {
                Logger.Instance.LogWarning($"Too many arguments ({argumentsToPush}) supplied to retroLogCallback", LOG_PRINTF_CALLER);
                return;
            }

            Sprintf(out string formattedString, format, argumentsToPush >= 1 ? arg1 : IntPtr.Zero, argumentsToPush >= 2 ? arg2 : IntPtr.Zero, argumentsToPush >= 3 ? arg3 : IntPtr.Zero, argumentsToPush >= 4 ? arg4 : IntPtr.Zero, argumentsToPush >= 5 ? arg5 : IntPtr.Zero, argumentsToPush >= 6 ? arg6 : IntPtr.Zero, argumentsToPush >= 7 ? arg7 : IntPtr.Zero, argumentsToPush >= 8 ? arg8 : IntPtr.Zero, argumentsToPush >= 9 ? arg9 : IntPtr.Zero, argumentsToPush >= 10 ? arg10 : IntPtr.Zero, argumentsToPush >= 11 ? arg11 : IntPtr.Zero, argumentsToPush >= 12 ? arg12 : IntPtr.Zero);

            switch (level)
            {
                case retro_log_level.RETRO_LOG_DEBUG:
                    Logger.Instance.LogInfo(formattedString, LOG_PRINTF_CALLER);
                    break;
                case retro_log_level.RETRO_LOG_INFO:
                    Logger.Instance.LogInfo(formattedString, LOG_PRINTF_CALLER);
                    break;
                case retro_log_level.RETRO_LOG_WARN:
                    Logger.Instance.LogWarning(formattedString, LOG_PRINTF_CALLER);
                    break;
                case retro_log_level.RETRO_LOG_ERROR:
                    Logger.Instance.LogError(formattedString, LOG_PRINTF_CALLER);
                    break;
            }
        }

        private static int GetFormatArgumentCount(string format)
        {
            int argumentsToPush = 0;
            MatchCollection matches = _argumentsRegex.Matches(format);

            foreach (Match match in matches)
            {
                switch (match.Groups[1].Value)
                {
                    case "b":
                    case "d":
                    case "x":
                    case "s":
                    case "u":
                        argumentsToPush += 1;
                        break;
                    case "f":
                    case "m":
                        argumentsToPush += 2;
                        break;
                    default:
                        throw new NotImplementedException($"Placeholder '{match.Value}' not implemented");
                }
            }

            return argumentsToPush;
        }

        private static void Sprintf(out string buffer, string format, params IntPtr[] args)
        {
            buffer = format;
        }
    }
}
