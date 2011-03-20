namespace Samples.SqlCe.DomainObjects {
    public class User {
        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string Firstname { get; set; }
        public virtual string Lastname { get; set; }
        public virtual string Email { get; set; }
    }
}
