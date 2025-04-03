using M241.Server.Data.Models;

namespace M241.Server.Common.Dtos
{
    public class ClientDto
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ClientDevice MapToClient()
        {
            return new ClientDevice { Id = Id, Name = Name };
        }
    }
}
