namespace Narc.Framework.Utilities
{
    using System;
    using ComLib;
    using Exceptions;

    /// <summary>
    ///     Wrapper class to simplify lines of code around Try/Catch blocks with various customized behaviour.
    /// </summary>
    public class Try
    {
        private static LamdaLogger _logger = new LamdaLogger();


        /// <summary>
        ///     Initialize logging lamda.
        /// </summary>
        public static LamdaLogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }


        /// <summary>
        ///     Calls the action and logs any exception that occurrs
        /// </summary>
        /// <param name="action">Action to execute inside a try/catch.</param>
        public static void CatchLog(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex, null);
            }
        }


        /// <summary>
        ///     Executes an action inside a try catch and logs any exceptions.
        /// </summary>
        /// <param name="errorMessage">Error message to log.</param>
        /// <param name="action">The function to call.</param>
        public static void CatchLog(string errorMessage, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger.Error(errorMessage, ex, null);
            }
        }


        /// <summary>
        ///     Calls the action and logs any exception that occurrs and rethrows the exception.
        ///     <param name="action">Action to execute in a try/catch.</param>
        /// </summary>
        public static void CatchLogRethrow(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex, null);
                throw ex;
            }
        }


        /// <summary>
        ///     Executes an action inside a try catch and logs any exceptions.
        /// </summary>
        /// <param name="errorMessage">Error message to log.</param>
        /// <param name="action">The function to call.</param>
        /// <param name="logger">The logger to use</param>
        public static void CatchLog(string errorMessage, Action action, Action<object, Exception, object[]> logger)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger(errorMessage, ex, null);
            }
        }


        /// <summary>
        ///     Executes an action inside a try catch and logs any exceptions.
        /// </summary>
        /// <param name="action">The function to call.</param>
        /// <param name="exceptionHandler">The action to use for handling the exception</param>
        public static void Catch(Action action, Action<Exception> exceptionHandler = null)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (exceptionHandler != null)
                    exceptionHandler(ex);
            }
        }


        /// <summary>
        ///     Executes an action inside a try catch and logs any exceptions.
        /// </summary>
        /// <param name="errorMessage">Error message to log.</param>
        /// <param name="action">The function to call.</param>
        /// <param name="exceptionHandler">The action to use for handling the exception</param>
        /// <param name="finallyHandler">The action to use in the finally block</param>
        public static void CatchHandle(string errorMessage, Action action, Action<Exception> exceptionHandler,
                                       Action finallyHandler)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (exceptionHandler != null)
                    exceptionHandler(ex);
            }
            finally
            {
                if (finallyHandler != null)
                {
                    finallyHandler();
                }
            }
        }


        public static T CatchLogGet<T>(string errorMessage, Func<T> action)
        {
            return CatchLogGet(errorMessage, false, action, null);
        }


        public static T CatchLogGet<T>(string errorMessage, bool rethrow, Func<T> action,
                                       Action<object, Exception, object[]> logger)
        {
            T result = default(T);
            try
            {
                result = action();
            }
            catch (Exception ex)
            {
                if (logger != null)
                    logger(errorMessage, ex, null);
                else if (_logger != null)
                    _logger.Error(errorMessage, ex, null);

                if (rethrow) throw ex;
            }
            return result;
        }

        public static void CatchBiz(Action action, Action<BizException> exceptionHandler)
        {
            try
            {
                action();
            }
            catch (BizException ex)
            {
                exceptionHandler(ex);
            }
        }

        public static void CatchBiz(Action action, Action<BizException> bizExceptionHandler,
                                    Action<Exception> exceptionHandler)
        {
            try
            {
                action();
            }
            catch (BizException ex)
            {
                bizExceptionHandler(ex);
            }
            catch (Exception ex)
            {
                _logger.Error("Exception occurs in json action.", ex);
                exceptionHandler(ex);
            }
        }
    }
}