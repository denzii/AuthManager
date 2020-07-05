namespace AuthServer.Contracts.Version1.ResponseContracts
{
    public class Response<T> //Generic Response wrapper to allow adding metadata to responses in the future for HATEOS
    {
        public Response(T response)
        {
            Data = response;
        }

        public T Data { get; set; }
    }
}