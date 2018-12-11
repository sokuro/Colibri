using System;
using System.Linq;
using Colibri.Models;
using Colibri.ViewModels;
using EasyNetQ;
using Microsoft.CodeAnalysis;

namespace Colibri.Subscriber.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            //AdvertisementViewModel advertisementViewModel = new AdvertisementViewModel();
            //Products products = new Products();

            using (var bus = RabbitHutch.CreateBus("host=localhost"))
            {
                //bus.Subscribe<AdvertisementViewModel>("my_subscription_id", HandleAddedAdvertisementMessage);
                bus.Subscribe<Products>("my_subscription_id", HandleAddedProductsMessage);
                bus.Subscribe<Products>("products_by_admin", HandleAddedProductsByAdminMessage);
                bus.Subscribe<Products>("removed_products_by_admin", HandleRemovedProductsByAdminMessage);
                bus.Subscribe<CategoryGroups>("create_category_groups", HandleAddedCategoryGroupMessage);
                bus.Subscribe<CategoryGroups>("remove_category_groups", HandleRemovedCategoryGroupMessage);

                Console.WriteLine("Listening for messages. Hit <return> to quit.");
                Console.ReadLine();

            }
        }

        private static void HandleAddedProductsMessage(Products obj)
        {
            Console.WriteLine("Processing Product by Consumer Name: {0}", obj.Name);
        }

        private static void HandleAddedProductsByAdminMessage(Products obj)
        {
            Console.WriteLine("Processing Product by Admin Name: {0}", obj.Name);
        }

        private static void HandleRemovedProductsByAdminMessage(Products obj)
        {
            Console.WriteLine("Removed Product by Admin Name: {0}", obj.Name);
        }

        private static void HandleAddedAdvertisementMessage(AdvertisementViewModel obj)
        {
            Console.WriteLine("Processing Name: {0}", obj.Products);
        }

        private static void HandleAddedCategoryGroupMessage(CategoryGroups obj1)
        {
            Console.WriteLine("Added Category Name: {0}", obj1.Name);
        }

        private static void HandleRemovedCategoryGroupMessage(CategoryGroups obj2)
        {
            Console.WriteLine("Removed Category Name: {0}", obj2.Name);
        }
    }
}
