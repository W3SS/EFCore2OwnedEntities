﻿using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace OwnedEntityDemo {
    class Program {
        static void Main (string[] args) {
            using (var context = new OrderContext ()) {
                context.Database.EnsureDeleted ();
                context.Database.EnsureCreated ();
            }
            InsertNewOrder ();
            RetrieveUpdateAndSaveOrder ();
        }

        private static void RetrieveUpdateAndSaveOrder () {
            var order = RetrieveOrder ();
            Console.WriteLine ("Before_____");
            Console.WriteLine ("Ship to: " + order.ShippingAddress?.Street);
            Console.WriteLine ("Bill to: " + order.BillingAddress?.Street);
            order.SetBillingAddress (PostalAddress.Create ("Broadway", "NYC", "NY", "10001"));
            using (var context = new OrderContext ()) {
                context.SalesOrders.Update (order);
                context.SaveChanges ();
            }
            var storedOrder = RetrieveOrder ();
            Console.WriteLine ("After_______");
            Console.WriteLine ("Ship to: " + storedOrder.ShippingAddress?.Street);
            Console.WriteLine ("Bill to: " + storedOrder.BillingAddress?.Street);
        }

        private static void InsertNewOrder () {
            var order = new SalesOrder (DateTime.Today, 100.00M);
            //following three statements work with different combos
            //of commenting
            order.SetShippingAddress (PostalAddress.Create ("One Main", "Burlington", "VT", "05000"));
            //order.SetBillingAddress (PostalAddress.Create ("Two Main", "Burlington", "VT", "05000"));
            //order.CopyShippingAddressToBillingAddress();
            using (var context = new OrderContext ()) {
                context.SalesOrders.Add (order);
                context.SaveChanges ();
            }
        }

        private static SalesOrder RetrieveOrder () {
            using (var context = new OrderContext ()) {
                var order = context.SalesOrders.FirstOrDefault ();
                FixOptionalValueObjects (order);
                return order;
            }
        }

        //my ddd brain will not let me put this into the SalesOrder aggregate root
        //this is how I can reset "empty" to "null" after a query
        private static void FixOptionalValueObjects (SalesOrder order) {
            if (order.ShippingAddress.IsEmpty ()) { order.SetShippingAddress (null); }
            if (order.BillingAddress.IsEmpty ()) { order.SetBillingAddress (null); }
        }

    }
}