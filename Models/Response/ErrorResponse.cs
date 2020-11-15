using System.Collections.Generic;

namespace TestSlabon.Models.Response
{
    public class ErrorResponse
    {
        public ErrorResponse(IEnumerable<string> _error, int _codeError)
        {
            Errors = _error;
            CodeError = _codeError;
        }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public int CodeError { get; set; }
    }
}
