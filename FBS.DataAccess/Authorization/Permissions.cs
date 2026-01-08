using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBS.Infrastructure.Authorization
{
    public static class Permissions
    {
        public static class Product
        {
            public const string Create = "Product.Create";
            public const string Edit = "Product.Edit";
            public const string Delete = "Product.Delete";
        }

        public static class Order
        {
            public const string View = "Order.View";
            public const string Update = "Order.Update";
        }

        public static class Blog
        {
            public const string Create = "Blog.Create";
            public const string Edit = "Blog.Edit";
            public const string Delete = "Blog.Delete";
        }

        public static class Review
        {
            public const string Manage = "Review.Manage";
        }

        public static class Customer
        {
            public const string Manage = "Customer.Manage";
        }

        public static class Contact
        {
            public const string Manage = "Contact.Manage";
        }

        public static class Revenue
        {
            public const string View = "Revenue.View";
        }
    }
}

