namespace AuthServer.Contracts.Version1.ResponseContracts
{
    public class Response<T>
    {
        public Response(T response)
        {
            Data = response;
        }

        public T Data { get; set; }
    }
}