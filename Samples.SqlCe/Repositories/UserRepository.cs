using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using Samples.SqlCe.DomainObjects;

namespace Samples.SqlCe.Repositories {
    public class UserRepository {
        private ISession _session;
        public ISession session {
            get { return _session; }
            set { _session = value; }
        }

        public bool CheckUsernameIsAvailable(string username) {
            IList<User> results = session.CreateCriteria(typeof(User))
                .Add(Expression.Eq("Username", username))
                .List<User>();

            return results.Count >= 1;
        }
    }
}
