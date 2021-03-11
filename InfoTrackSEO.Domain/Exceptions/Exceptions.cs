using System;
using System.Collections.Generic;
using System.Text;

namespace InfoTrackSEO.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string msg) : base(msg)
        {

        }
    }
    public class DomainServiceException : Exception
    {
        public DomainServiceException(string msg) : base(msg)
        {

        }
    }
}
