using System.Net.Http;

namespace cargoSprint.API.CustomExceptions
{
    public static class FoundExceptions
    {


        public static void NotFoundException(int itemId){
        var resp = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        {

            ReasonPhrase = string.Format("Item {0} ID Not Found",itemId)
        };
        throw new CustomExceptions.HttpResponseException(resp.ReasonPhrase);

        }

    }
}