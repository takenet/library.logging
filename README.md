Takenet Library.Logging
=======================

Provides a simple logging interface for applications and some basic implementations of this interface.

Loggers
-------

A logger is a class that implements the **ILogger** interface.

Built-in loggers:

- **DebugLogger**: Writes to the output window of Visual Studio
- **EventViewerLogger**: Writes to the Windows Event Viewer
- **QueueLogger**: Writes to a MSMQ queue
- **TextFileLogger**: Writes to a file in the file system
- **AggregateLogger**: Allow logging in multiple loggers

Custom loggers (each one in a specific assembly):

- **EntityFrameworkLogger**: Writes to a SQL database using the EntityFramework library
- **MongoLogger**: Writes to a MongoDB collection using the official MongoDB driver
- **HttpLogger**: Writes to a HTTP service, via HTTP POST/PUT and JSON


Filters
-------

Provides message filtering capabilities to the loggers.

- **ApiLogFilter**: Queries a HTTP API if the message should be logged
- **SeverityLogFilter**: Filters the messages according to the severity
- **CachedLogFilter**: Allow caching of external log filters, like the ApiLogFilter

Extensions
----------

There are some helper extensions methods to make easier the use of the library. Instead of instantiating a LogMessage object and calling the *LogMessage* method of *ILogger* interface, just uses the specified extension methods for each severity:

- **WriteVerbose**
- **WriteInformation**
- **WriteWarning**
- **WriteError**
- **WriteCritical**

And for each extension methods, there's overloads with default values to avoid the developer needs to pass **null** where some information is not available.
One of these overloads receives a **Func<string>** instead of a string for the message parameter, which is invoked only if the log message is not filtered. This is useful to avoid the ovehead of string formatting / concatenating when the message is filtered.

Async support
-------------

The library also defines a ILoggerAsync interface, with a *LogAsync* method, for asynchronous implementations.
