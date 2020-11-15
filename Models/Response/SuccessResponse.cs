namespace TestSlabon.Models.Response
{
    public class SuccessResponse
    {
        public SuccessResponse()
        {
            Success = true;
            Message = "Successful operation";
        }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
