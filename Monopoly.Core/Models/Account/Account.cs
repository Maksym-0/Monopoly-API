namespace Monopoly.Core.Models.Account
{
    public class Account
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }
        public string Password { get; private set; }

        public Account(Guid id, string name, string password)
        {
            Id = id;
            Name = name;
            Password = password;
        }
        public Account() { }
    }
}
