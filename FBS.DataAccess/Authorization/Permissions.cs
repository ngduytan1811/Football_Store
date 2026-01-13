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
            public const string ProductCreate = "Product.Create";
            public const string ProductEdit = "Product.Edit";
            public const string ProductDelete = "Product.Delete";
            public const string CategoryCreate = "Category.Create";
            public const string CategoryEdit = "Category.Edit";
            public const string CategoryDelete = "Category.Delete";
        }

        public static class Order
        {
            public const string View = "Order.View";
            public const string Update = "Order.Update";
            public const string ConfirmPayment = "Order.ConfirmPayment";
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
            public const string Create = "Customer.Create";
            public const string Edit = "Customer.Edit";
            public const string Update = "Customer.Update";
            public const string Delete = "Customer.Delete";
        }

        public static class Contact
        {
            public const string Manage = "Contact.Manage"; 
            public const string Delete = "Contact.Delete";
        }

        public static class Revenue
        {
            public const string View = "Revenue.View";
        }
    }
}

