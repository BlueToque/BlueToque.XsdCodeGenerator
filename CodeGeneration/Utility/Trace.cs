//=============================================================================
//
// Copyright (C) 2007 Michael Coyle, Blue Toque
// Copyright (C) 2023 Michael Coyle, Blue Toque Software
// http://www.BlueToque.ca/Products/CodeGeneration.html
// michael.coyle@BlueToque.ca
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// http://www.gnu.org/licenses/gpl.txt
//
//=============================================================================
using System;
using System.Diagnostics;

namespace CodeGeneration
{
    /// <summary>
    /// A Facade class to control trace
    /// This allows us to turn on or off trace for this module only,
    /// and to control what level of trace comes from this module (Info, Warning, Error, etc)
    /// </summary>
    internal static class Trace
    {
        static readonly TraceSource m_traceSource = new TraceSource("CodeGeneration");

        public static CorrelationManager CorrelationManager => System.Diagnostics.Trace.CorrelationManager;

        public static TraceListenerCollection Listeners => System.Diagnostics.Trace.Listeners;

        public static SourceSwitch Switch => m_traceSource.Switch;

        #region assert
        [Conditional("TRACE")]
        public static void Assert(bool condition) => System.Diagnostics.Trace.Assert(condition);
        [Conditional("TRACE")]
        public static void Assert(bool condition, string message) => System.Diagnostics.Trace.Assert(condition, message);
        #endregion

        #region information
        [Conditional("TRACE")]
        public static void TraceInformation(string str) => m_traceSource.TraceInformation(str);
        [Conditional("TRACE")] 
        public static void TraceInformation(string format, params object[] args) => m_traceSource.TraceInformation(format, args);
        #endregion

        #region error
        [Conditional("TRACE")]
        public static void TraceError(string format) => m_traceSource.TraceEvent(TraceEventType.Error, 0, format);
        [Conditional("TRACE")]
        public static void TraceError(int number, string format) => m_traceSource.TraceEvent(TraceEventType.Error, number, format);
        [Conditional("TRACE")]
        public static void TraceError(string format, params object[] args) => m_traceSource.TraceEvent(TraceEventType.Error, 0, format, args);
        [Conditional("TRACE")]
        public static void TraceError(int number, string format, params object[] args) => m_traceSource.TraceEvent(TraceEventType.Error, number, format, args);
        #endregion

        #region warning
        [Conditional("TRACE")]
        public static void TraceWarning(string format) => m_traceSource.TraceEvent(TraceEventType.Warning, 0, format);
        [Conditional("TRACE")]
        public static void TraceWarning(string format, params object[] args) => m_traceSource.TraceEvent(TraceEventType.Warning, 0, format, args);
        #endregion

        #region event
        [Conditional("TRACE")]
        public static void TraceEvent(TraceEventType eventType, int id, string format) => m_traceSource.TraceEvent(eventType, id, format);
        [Conditional("TRACE")]
        public static void TraceEvent(TraceEventType eventType, int id, string format, params object[] args) => m_traceSource.TraceEvent(eventType, id, format, args);
        #endregion

        #region writeline
        [Conditional("TRACE")]
        public static void WriteLine(string message) => m_traceSource.TraceEvent(TraceEventType.Verbose, 0, message);
        [Conditional("TRACE")]
        public static void WriteLine(string message, string format) => m_traceSource.TraceEvent(TraceEventType.Verbose, 0, message, format);
        #endregion

    }
}
