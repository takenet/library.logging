using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Messaging;
using System.Security.Principal;

namespace Takenet.Library.Logging
{
    /// <summary>
    /// Utility class to make the access to log queues.
    /// </summary>
    public sealed class QueueLogger : ILogger
    {
        public const string QUEUE_PATH_TEMPLATE = @".\private$\takenet_{0}_log";

        #region Constructors

        /// <summary>
        /// Instantiate a new QueueLogger class
        /// without specifying a log filter
        /// </summary>
        public QueueLogger()
        {

        }

        /// <summary>
        /// Instantiate a new QueueLogger class
        /// using specified log filter
        /// </summary>
        /// <param name="filter"></param>
        public QueueLogger(ILogFilter filter)
        {
            Filter = filter;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Send a log message to the queue
        /// </summary>
        /// <param name="logMessage"></param>
        public static void EnqueueLog(LogMessage logMessage)
        {
            EnqueueLog(logMessage, GetQueuePath(logMessage.ApplicationName));
        }

        /// <summary>
        /// Send a log message to the queue
        /// </summary>
        /// <param name="logMessage"></param>
        public static void EnqueueLog(LogMessage logMessage, string queuePath)
        {
            if (!MessageQueue.Exists(queuePath))
            {
                CreateQueue(queuePath);
            }


            using (MessageQueue messageQueue = new MessageQueue(queuePath, false, true, QueueAccessMode.Send))
            {
                using (Message message = new Message())
                {
                    message.Formatter = new BinaryMessageFormatter();
                    message.Body = logMessage;

                    messageQueue.Send(message);
                }
            }
        }

        /// <summary>
        /// Pop a message for log queue, if exists one.
        /// The process will be locked until a message is taken from the queue.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public static LogMessage DequeueLog(string applicationName)
        {
            return DequeueLog(applicationName, GetQueuePath(applicationName));
        }

        /// <summary>
        /// Pop a message for log queue, if exists one.
        /// The process will be locked until a message is taken from the queue.
        /// </summary>
        public static LogMessage DequeueLog(string applicationName, string queuePath)
        {
            if (!MessageQueue.Exists(queuePath))
            {
                CreateQueue(queuePath);
            }

            LogMessage logmessage = null;

            using (var messageQueue = new MessageQueue(queuePath, false, true, QueueAccessMode.Receive))
            {
                using (Message message = messageQueue.Receive())
                {
                    message.Formatter = new BinaryMessageFormatter();
                    logmessage = (LogMessage)message.Body;
                }
            }

            return logmessage;
        }

        /// <summary>
        /// Get a queue path for specific application, 
        /// according to the queue name template.
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public static string GetQueuePath(string applicationName)
        {
            string queueApplicationName = applicationName.ToLower().Replace(' ', '_');
            string queuePath = string.Format(QUEUE_PATH_TEMPLATE, queueApplicationName);

            return queuePath;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Creates a queue in the specified path with
        /// the correct user permissions
        /// </summary>
        /// <param name="queuePath"></param>
        private static void CreateQueue(string queuePath)
        {
            var queue = MessageQueue.Create(queuePath);

            // Gives to the Everyone Windows user full control to the created log queue
            SecurityIdentifier everyoneSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            var everyoneAccount = (NTAccount)everyoneSid.Translate(typeof(NTAccount));
            queue.SetPermissions(everyoneAccount.Value, MessageQueueAccessRights.FullControl);

            queue.Dispose();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Current filter of logger
        /// </summary>
        public ILogFilter Filter { get; set; }

        #endregion

        #region ILogger Members

        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="logMessage"></param>
        public void WriteLog(LogMessage logMessage)
        {
            if (this.ShouldWriteLog(logMessage))
            {
                EnqueueLog(logMessage);
            }
        }

        #endregion
    }
}