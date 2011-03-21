namespace Samples.SqlCe.DomainObjects {
    public class User {
        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string EmailAddress { get; set; }
    }
}
