namespace Monopoly.Core.DTO.Accounts
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public AccountDto(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        public AccountDto() { }
    }
}
