namespace Samples.SqlCe.DomainObjects {
    public class User {
        private string _firstname;
        private string _lastname;
        private string _email;
        private int _id;
        private string _username;

        public int Id {
            get { return _id; }
            set { _id = value; }
        }

        public string Username {
            get { return _username; }
            set { _username = value; }
        }

        public string Firstname {
            get { return _firstname; }
            set { _firstname = value; }
        }

        public string Lastname {
            get { return _lastname; }
            set { _lastname = value; }
        }

        public string Email {
            get { return _email; }
            set { _email = value; }
        }

    }
}
