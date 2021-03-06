﻿using System;
using System.Collections.Generic;

namespace Darc.Core.Exceptions
{
    using System.ComponentModel.DataAnnotations;

    public class BizException : Exception
    {
        private readonly List<string> _errorMessage = new List<string>();

        public BizException(Exception ex)
        {
            _errorMessage.Add(ex.Message);
        }

        public BizException(string message)
            : base(message)
        {
            _errorMessage.Add(message);
        }

        public BizException(IEnumerable<string> messages)
        {
            _errorMessage.AddRange(messages);
        }

        public BizException(IEnumerable<ValidationResult> results)
        {
            foreach (var item in results)
            {
                _errorMessage.Add(item.ErrorMessage);
            }
        }

        public List<string> ErrorMessages
        {
            get { return _errorMessage; }
        }
    }
}
