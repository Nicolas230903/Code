using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACHE.Model.Negocio.TiendaNube
{
    public class ResponseGetOrders
    {
        public int? id { get; set; }
        public string token { get; set; }
        public string store_id { get; set; }
        public string shipping_min_days { get; set; }
        public string shipping_max_days { get; set; }
        public string billing_name { get; set; }
        public string billing_phone { get; set; }
        public string billing_address { get; set; }
        public string billing_number { get; set; }
        public string billing_floor { get; set; }
        public string billing_locality { get; set; }
        public string billing_zipcode { get; set; }
        public string billing_city { get; set; }
        public string billing_province { get; set; }
        public string billing_country { get; set; }
        public string shipping_cost_owner { get; set; }
        public string shipping_cost_customer { get; set; }
        public List<string> coupon { get; set; }
        public PromotionalDiscount promotional_discount { get; set; }
        public string subtotal { get; set; }
        public string discount { get; set; }
        public string discount_coupon { get; set; }
        public string discount_gateway { get; set; }
        public string total { get; set; }
        public string total_usd { get; set; }
        public bool checkout_enabled { get; set; }
        public string weight { get; set; }
        public string currency { get; set; }
        public string language { get; set; }
        public string gateway { get; set; }
        public string gateway_id { get; set; }
        public string shipping { get; set; }
        public string shipping_option { get; set; }
        public string shipping_option_code { get; set; }
        public string shipping_option_reference { get; set; }
        public ShippingPickupDetails shipping_pickup_details { get; set; }
        public string shipping_tracking_number { get; set; }
        public string shipping_tracking_url { get; set; }
        public string shipping_store_branch_name { get; set; }
        public string shipping_pickup_type { get; set; }
        public List<string> shipping_suboption { get; set; }
        public Extra extra { get; set; }
        public string storefront { get; set; }
        public string note { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public CompletedAt completed_at { get; set; }
        public string next_action { get; set; }
        public PaymentDetails payment_details { get; set; }
        public List<string> attributes { get; set; }
        public Customer customer { get; set; }
        public List<Product> products { get; set; }
        public int? number { get; set; }
        public string cancel_reason { get; set; }
        public string owner_note { get; set; }
        public string cancelled_at { get; set; }
        public string closed_at { get; set; }
        public string read_at { get; set; }
        public string status { get; set; }
        public string payment_status { get; set; }
        public ShippingAddress shipping_address { get; set; }
        public string shipping_status { get; set; }
        public string shipped_at { get; set; }
        public string paid_at { get; set; }
        public string landing_url { get; set; }
        public ClientDetails client_details { get; set; }
        public string app_id { get; set; }


        public class PromotionalDiscount
        {
            public string id { get; set; }
            public int? store_id { get; set; }
            public string order_id { get; set; }
            public DateTime created_at { get; set; }
            public string total_discount_amount { get; set; }
            public List<string> contents { get; set; }
            public List<string> promotions_applied { get; set; }

        }

        public class Extra
        {
            public string giftwrap { get; set; }
        }

        public class CompletedAt
        {
            public string date { get; set; }
            public int? timezone_type { get; set; }
            public string timezone { get; set; }

        }

        public class PaymentDetails
        {
            public string method { get; set; }
            public string credit_card_company { get; set; }
            public string installments { get; set; }

        }

        public class DefaultAddress
        {
            public string address { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public DateTime created_at { get; set; }
            public bool Default { get; set; }
            public string floor { get; set; }
            public int? id { get; set; }
            public string locality { get; set; }
            public string name { get; set; }
            public string number { get; set; }
            public string phone { get; set; }
            public string province { get; set; }
            public DateTime updated_at { get; set; }
            public string zipcode { get; set; }

        }

        public class Address
        {
            public string address { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public DateTime created_at { get; set; }
            public bool Default { get; set; }
            public string floor { get; set; }
            public int? id { get; set; }
            public string locality { get; set; }
            public string name { get; set; }
            public string number { get; set; }
            public string phone { get; set; }
            public string province { get; set; }
            public DateTime updated_at { get; set; }
            public string zipcode { get; set; }

        }

        public class Extra2
        {

        }

        public class Customer
        {
            public int? id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string identification { get; set; }
            public string phone { get; set; }
            public string note { get; set; }
            public DefaultAddress default_address { get; set; }
            public List<Address> addresses { get; set; }
            public string billing_name { get; set; }
            public string billing_phone { get; set; }
            public string billing_address { get; set; }
            public string billing_number { get; set; }
            public string billing_floor { get; set; }
            public string billing_locality { get; set; }
            public string billing_zipcode { get; set; }
            public string billing_city { get; set; }
            public string billing_province { get; set; }
            public string billing_country { get; set; }
            public Extra2 extra { get; set; }
            public string total_spent { get; set; }
            public string total_spent_currency { get; set; }
            public int? last_order_id { get; set; }
            public bool active { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }

        }

        public class Image
        {
            public int? id { get; set; }
            public int? product_id { get; set; }
            public string src { get; set; }
            public int? position { get; set; }
            public List<string> alt { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }

        }

        public class Product
        {
            public int? id { get; set; }
            public string depth { get; set; }
            public string height { get; set; }
            public string name { get; set; }
            public string price { get; set; }
            public int? product_id { get; set; }
            public Image image { get; set; }
            public string quantity { get; set; }
            public bool free_shipping { get; set; }
            public string weight { get; set; }
            public string width { get; set; }
            public string variant_id { get; set; }
            public List<string> variant_values { get; set; }
            public List<string> properties { get; set; }
            public string sku { get; set; }
            public string barcode { get; set; }

        }

        public class ShippingAddress
        {
            public string address { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public DateTime created_at { get; set; }
            public bool Default { get; set; }
            public string floor { get; set; }
            public int? id { get; set; }
            public string locality { get; set; }
            public string name { get; set; }
            public string number { get; set; }
            public string phone { get; set; }
            public string province { get; set; }
            public DateTime updated_at { get; set; }
            public string zipcode { get; set; }

        }

        public class ClientDetails
        {
            public string browser_ip { get; set; }
            public string user_agent { get; set; }

        }

        public class ShippingPickupDetails
        {
            public string name { get; set; }
            public string address { get; set; }
            public string hours { get; set; }

        }


    }
}
