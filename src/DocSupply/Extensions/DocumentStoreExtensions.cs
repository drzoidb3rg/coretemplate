using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocSupply.Features.Session;
using Raven.Client;
using DocSupply.Models;

namespace DocSupply
{
    public static class DocumentStoreExtensions
    {
        public static T Load<T>(this IDocumentStore store, string id) where T : RavenRecord
        {
            using (var session = store.OpenSession())
                return session.Load<T>(id);
        }

        public static void Store (this IDocumentStore store, object doc)
        {
            using (var session = store.OpenSession())
            {
                session.Store(doc);
                session.SaveChanges();
            }
        }

        public static void Delete<T>(this IDocumentStore store, string id)
        {
            using (var session = store.OpenSession())
            {
                var doc = session.Load<T>(id);
                session.Delete(doc);
                session.SaveChanges();
            }
        }
    }
}
