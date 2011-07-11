using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Globalization;

namespace FimExtensions.FimActivityLibrary
{
    /// <summary>
    /// Provides a set of methods and properties that enable applications to trace the execution of code and associate trace messages with their source. 
    /// </summary>
    [ComVisible(false)]
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class TraceSourceEx : TraceSource
    {
        /// <summary>
        /// The instance for trace within the common dll
        /// </summary>
        internal static TraceSourceEx Instance
        {
            get { return new TraceSourceEx("Microsoft.ResourceManagement"); }
        }

        /// <summary>
        /// Const format string.
        /// </summary>
        private const string FormatString = "{0:d} {0:T} -- {1}";

        /// <summary>
        /// Initializes a new instance of the TraceSource class, using the specified name for the source and the default source level at which tracing is to occur.
        /// </summary>
        /// <param name="name">The name of the source, typically the name of the application.</param>
        public TraceSourceEx(string name) : base(name) { }

        /// <summary>
        /// Writes a trace event at SourceLevels.Critical (0x0001) using the specified message.
        /// </summary>
        /// <param name="message">The informative message to write.</param>
        [Conditional("TRACE")]
        public void TraceFatal(string message)
        {
            TraceEvent(TraceEventType.Critical, 0, FormatString, DateTime.UtcNow, message);
        }

        /// <summary>
        /// Writes a trace event at SourceLevels.Critical (0x0001) using the specified event identifier, and argument array and format.
        /// </summary>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        [Conditional("TRACE")]
        public void TraceFatal(string format, params object[] args)
        {
            TraceEvent(TraceEventType.Critical, 0, FormatString, DateTime.UtcNow, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Writes a trace event at SourceLevels.Error (0x0003)  using the specified message.
        /// </summary>
        /// <param name="message">The informative message to write.</param>
        [Conditional("TRACE")]
        public void TraceError(string message)
        {
            TraceEvent(TraceEventType.Error, 0, FormatString, DateTime.UtcNow, message);
        }

        /// <summary>
        /// Writes a trace event at SourceLevels.Error (0x0003) using the specified argument array and format.
        /// </summary>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        [Conditional("TRACE")]
        public void TraceError(string format, params object[] args)
        {
            TraceEvent(TraceEventType.Error, 0, FormatString, DateTime.UtcNow, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Writes a trace event at SourceLevels.Warning (0x0007)  using the specified message.
        /// </summary>
        /// <param name="message">The informative message to write.</param>
        [Conditional("TRACE")]
        public void TraceWarning(string message)
        {
            TraceEvent(TraceEventType.Warning, 0, FormatString, DateTime.UtcNow, message);
        }

        /// <summary>
        /// Writes a trace event at SourceLevels.Warning (0x0007) using the specified argument array and format.
        /// </summary>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        [Conditional("TRACE")]
        public void TraceWarning(string format, params object[] args)
        {
            TraceEvent(TraceEventType.Warning, 0, FormatString, DateTime.UtcNow, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Writes a trace event at SourceLevels.Information (0x000F) using the specified message.
        /// </summary>
        /// <param name="message">The informative message to write.</param>
        [Conditional("TRACE")]
        public void TraceInfo(string message)
        {
            TraceEvent(TraceEventType.Information, 0, FormatString, DateTime.UtcNow, message);
        }

        /// <summary>
        /// Writes a trace event at SourceLevels.Information (0x000F) using the specified argument array and format.
        /// </summary>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        [Conditional("TRACE")]
        public void TraceInfo(string format, params object[] args)
        {
            TraceEvent(TraceEventType.Information, 0, FormatString, DateTime.UtcNow, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Writes a trace event at SourceLevels.Verbose (0x001F) using the specified message.
        /// </summary>
        /// <param name="message">The informative message to write.</param>
        [Conditional("TRACE")]
        public void TraceVerbose(string message)
        {
            TraceEvent(TraceEventType.Verbose, 0, FormatString, DateTime.UtcNow, message);
        }

        /// <summary>
        /// Writes a trace event at SourceLevels.Verbose (0x001F) using the specified argument array and format.
        /// </summary>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        [Conditional("TRACE")]
        public void TraceVerbose(string format, params object[] args)
        {
            TraceEvent(TraceEventType.Verbose, 0, FormatString, DateTime.UtcNow, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Writes a start event using the specified argument array and format
        /// </summary>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        /// 
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        [Conditional("TRACE")]
        public void TraceStart(string format, params object[] args)
        {
            TraceEvent(TraceEventType.Start, 0, FormatString, DateTime.UtcNow, String.Format(CultureInfo.InvariantCulture, format, args));
            Trace.CorrelationManager.StartLogicalOperation();
        }

        /// <summary>
        /// Writes a start event using the specified message
        /// </summary>
        /// <param name="message">The informative message to write.</param>
        /// 
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        [Conditional("TRACE")]
        public void TraceStart(String message)
        {
            TraceEvent(TraceEventType.Start, 0, FormatString, DateTime.UtcNow, message);
            Trace.CorrelationManager.StartLogicalOperation();
        }

        /// <summary>
        /// Writes a start event using the specified message
        /// </summary>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">The informative message to write.</param>
        /// 
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        [Conditional("TRACE")]
        public void TraceStart(int id, String message)
        {
            TraceEvent(TraceEventType.Start, id, FormatString, DateTime.UtcNow, message);
            Trace.CorrelationManager.StartLogicalOperation();
        }

        /// <summary>
        /// Writes a stop event using the specified argument array and format
        /// </summary>
        /// <param name="format">A format string that contains zero or more format items, which correspond to objects in the args array.</param>
        /// <param name="args">An object array containing zero or more objects to format.</param>
        /// 
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        [Conditional("TRACE")]
        public void TraceStop(string format, params object[] args)
        {
            TraceEvent(TraceEventType.Stop, 0, FormatString, DateTime.UtcNow, String.Format(CultureInfo.InvariantCulture, format, args));
            Trace.CorrelationManager.StopLogicalOperation();
        }

        /// <summary>
        /// Writes a stop event using the specified message
        /// </summary>
        /// <param name="message">The informative message to write.</param>
        /// 
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        [Conditional("TRACE")]
        public void TraceStop(String message)
        {
            TraceEvent(TraceEventType.Stop, 0, FormatString, DateTime.UtcNow, message);
            Trace.CorrelationManager.StopLogicalOperation();
        }

        /// <summary>
        /// Writes a stop event using the specified message
        /// </summary>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">The informative message to write.</param>
        /// 
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        [Conditional("TRACE")]
        public void TraceStop(int id, String message)
        {
            TraceEvent(TraceEventType.Stop, id, FormatString, DateTime.UtcNow, message);
            Trace.CorrelationManager.StopLogicalOperation();
        }

    }
}
