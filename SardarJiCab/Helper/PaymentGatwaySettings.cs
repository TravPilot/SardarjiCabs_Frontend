using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using SardarJi_Cab_Booking.Business_Layer;
using SardarJi_Cab_Booking.Models;

namespace SardarJi_Cab_Booking.Helper
{
    
    public class PaymentGatwaySettings
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly IRazorGatewayRepository _razor; 
      
        private readonly long ClientId;
        private string empty_value = "";
        public string gen_hash;

        
        private HttpContext CurrentHttpContext => _httpContextAccessor.HttpContext;
        private ISession Session => CurrentHttpContext.Session;

        public PaymentGatwaySettings(IHttpContextAccessor httpContextAccessor, IConfiguration config, IRazorGatewayRepository razor)
        {
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _razor = razor;
           

          ClientId = Convert.ToInt64(_config["ClientId"]);
        }

        public async Task<PaymentGatewaySettings> GetGatewaySettings()
        {
            PaymentGatewaySettings paymentGatewaySettings = await _razor.GetPaymentGatewaySettings(ClientId);
            return paymentGatewaySettings;
        }

        public async Task<OrderModel> CheckPaymentGateway(double amount, string orderid, bool IsCard, bool IsWallet, string customerdetails, ProfileSettings profileSettings, string service)
        {
            OrderModel order = new OrderModel();
            OrderModel orderModel = new OrderModel();
            string URI = "";

            var checkPG = await GetGatewaySettings();

            if (checkPG.Paymentgateway.ToLower() == "easebuzz".ToLower())
            {
                URI = MethodEaseBuzz(amount, orderid, checkPG.Merchantkey, checkPG.MerchantCode, checkPG.Environment, service);
            }
            else if (checkPG.Paymentgateway.ToLower() == "cc avenue".ToLower())
            {
                //URI = MethodCCAvenue(amount, orderid, checkPG.Merchantkey, checkPG.MerchantCode, checkPG.MerchantId, checkPG.Environment, service);
            }
            else if (checkPG.Paymentgateway.ToLower() == "razorpay".ToLower())
            {
                //URI = MethodRazorpay(amount, orderid, checkPG.Merchantkey, checkPG.MerchantCode, checkPG.MerchantId, checkPG.Environment, out orderModel, profileSettings);

                //order.orderModel = orderModel;
                //orderid = orderModel.orderId;
            }
            else if (checkPG.Paymentgateway.ToLower() == "Cash Free".ToLower())
            {
                URI = MethodCashfree(amount, orderid, checkPG.Merchantkey, checkPG.MerchantCode, checkPG.MerchantId, checkPG.Environment, service, out orderModel);

                order.strformPayU = orderModel.strformPayU;
                orderid = orderModel.orderId;
            }
            else if (checkPG.Paymentgateway.ToLower() == "PayU".ToLower())
            {
                URI = MethodPayU(amount, orderid, checkPG.Merchantkey, checkPG.MerchantCode, checkPG.MerchantId, checkPG.Environment, out orderModel, service);

                order.strformPayU = orderModel.strformPayU;
                orderid = orderModel.orderId;
            }

            if (URI != null || (order.strformPayU != null && order.strformPayU != ""))
            {
                SavetransactionDetails(orderid, amount.ToString(), customerdetails);
            }
            order.URI = URI;

            return order;
        }

        public async Task<RazorPayVM> CheckCapturePaymentGateway()
        {
            RazorPayVM razorPayVM = new RazorPayVM();
            var checkPG = await GetGatewaySettings();

            if (checkPG.Paymentgateway.ToLower() == "easebuzz".ToLower())
            {
                razorPayVM = await CaptureEaseBuzz(checkPG.MerchantCode);
            }
            else if (checkPG.Paymentgateway.ToLower() == "cc avenue".ToLower())
            {
               // razorPayVM = CaptureCCAvenue(checkPG.Merchantkey);
            }
            else if (checkPG.Paymentgateway.ToLower() == "razorpay".ToLower())
            {
                //razorPayVM = CaptureRazorPay(checkPG.Merchantkey, checkPG.MerchantCode);
            }
            else if (checkPG.Paymentgateway.ToLower() == "Cash Free".ToLower())
            {
                razorPayVM =await CaptureCashFree(checkPG.Merchantkey);
            }
            else if (checkPG.Paymentgateway.ToLower() == "PayU".ToLower())
            {
                razorPayVM =await CapturePayU(checkPG.Merchantkey, checkPG.MerchantCode, checkPG.MerchantId, checkPG.Environment);
            }

            return razorPayVM;
        }

        // Builds "https://host" the way Request.Url.GetLeftPart(UriPartial.Authority) used to.
        private string GetSiteRoot()
        {
            var request = CurrentHttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }

        #region CCAvenue
        //public string MethodCCAvenue(double totaamount, string orderId, string Key, string code, string merchantid, string env, string service)
        //{
        //    string mainva = "";

        //    var baseurl = GetBaseURL(service);
        //    CCACrypto ccaCrypto = new CCACrypto();

        //    string workingKey = Key;
        //    string ccaRequest;
        //    string strEncRequest;
        //    string strAccessCode = code;
        //    string currency = "INR";
        //    string merchant_id = merchantid;
        //    string amount = totaamount.ToString();
        //    string redirect_url = GetSiteRoot() + baseurl.SuccessURl;
        //    string cancel_url = GetSiteRoot() + baseurl.FailURl;

        //    ccaRequest = "merchant_id=" + merchant_id + "&order_id=" + orderId + "&amount=" + amount + "&currency=" + currency + "&redirect_url=" + redirect_url + "&cancel_url=" + cancel_url;

        //    strEncRequest = ccaCrypto.Encrypt(ccaRequest, workingKey);
        //    mainva = getURL_CCAvenue(env) + "/transaction/transaction.do?command=initiateTransaction&encRequest=" + strEncRequest + "&access_code=" + strAccessCode;

        //    return mainva;
        //}

        //public RazorPayVM CaptureCCAvenue(string Salt)
        //{
        //    RazorPayVM razorPay = new RazorPayVM();
        //    bool PaymentStatus = false;

        //    try
        //    {
        //        CCACrypto ccaCrypto = new CCACrypto();
        //        string workingKey = Salt;
        //        bool IsSuccess = false; string orderId = ""; string Amt = "";
        //        string encResponse = ccaCrypto.Decrypt(CurrentHttpContext.Request.Form["encResp"], workingKey);
        //        NameValueCollection Params = new NameValueCollection();
        //        string[] segments = encResponse.Split('&');
        //        foreach (string seg in segments)
        //        {
        //            string[] parts = seg.Split('=');
        //            if (parts.Length > 0)
        //            {
        //                string Key = parts[0].Trim();
        //                string Value = parts[1].Trim();
        //                Params.Add(Key, Value);
        //            }
        //        }

        //        for (int i = 0; i < Params.Count; i++)
        //        {
        //            if (Params.Keys[i] == "order_id")
        //            {
        //                orderId = (Params[i]).ToString();
        //            }
        //            if (Params.Keys[i] == "order_status")
        //            {
        //                if (Params[i] == "Success")
        //                {
        //                    IsSuccess = true;
        //                }
        //            }
        //            if (Params.Keys[i] == "amount")
        //            {
        //                Amt = (Params[i]).ToString();
        //            }
        //        }
        //        if (IsSuccess)
        //        {
        //            razorPay = SetdataByOrderId(orderId);
        //            PaymentStatus = true;
        //            razorPay.PaymentStatus = PaymentStatus;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        razorPay.PaymentStatus = PaymentStatus;
        //        return razorPay;
        //    }

        //    return razorPay;
        //}

        //public string getURL_CCAvenue(string env)
        //{
        //    if (env == "test") return "https://test.ccavenue.com";
        //    if (env == "prod") return "https://secure.ccavenue.com";
        //    return "https://test.ccavenue.com";
        //}
        #endregion

        #region PayU
        public string MethodPayU(double totaamount, string orderId, string Key, string code, string merchantid, string env, out OrderModel order, string service)
        {
            var baseurl = GetBaseURL(service);
            string action1;
            string hash1 = string.Empty;
            string txnid1;
            string strForm = string.Empty;
            string mainva = "";
            string[] hashVarsSeq;
            string hash_string = string.Empty;

            txnid1 = orderId;
            CustomerVM BD = Session.GetObject<CustomerVM>("customer");

            if (string.IsNullOrEmpty(CurrentHttpContext.Request.Form["hash"])) 
            {
                if (string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(txnid1) || string.IsNullOrEmpty(totaamount.ToString()))
                {
                    // error
                }
                else
                {
                    hashVarsSeq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10".Split('|');
                    hash_string = "";
                    foreach (string hash_var in hashVarsSeq)
                    {
                        if (hash_var == "key")
                        {
                            hash_string = hash_string + Key + '|';
                        }
                        else if (hash_var == "txnid")
                        {
                            hash_string = hash_string + txnid1 + '|';
                        }
                        else if (hash_var == "amount")
                        {
                            hash_string = hash_string + Convert.ToDecimal(totaamount).ToString("g29") + '|';
                        }
                        else if (hash_var == "productinfo")
                        {
                            hash_string = hash_string + "Booking" + '|';
                        }
                        else if (hash_var == "firstname")
                        {
                            hash_string = hash_string + BD.FirstName.Replace(" ", "") + '|';
                        }
                        else if (hash_var == "email")
                        {
                            hash_string = hash_string + BD.Email + '|';
                        }
                        else
                        {
                            hash_string = hash_string + "" + '|';
                        }
                    }
                }
            }

            hash_string += code; // appending SALT

            hash1 = Generatehash512(hash_string).ToLower();
            action1 = getURL_PayU(env) + "/_payment";

            if (!string.IsNullOrEmpty(hash1))
            {
                var data = new Dictionary<string, string>();
                data.Add("hash", hash1);
                data.Add("txnid", txnid1);
                data.Add("key", Key);
                string AmountForm = Convert.ToDecimal(totaamount).ToString("g29");

                string surl = GetSiteRoot() + baseurl.SuccessURl;
                string failurl = GetSiteRoot() + baseurl.FailURl;
                data.Add("amount", AmountForm);
                data.Add("firstname", BD.FirstName.ToString().Replace(" ", "").Trim());
                data.Add("email", BD.Email.ToString().Trim());
                data.Add("phone", BD.Mobile.ToString().Trim());
                data.Add("productinfo", "Booking");
                data.Add("surl", surl);
                data.Add("furl", failurl);
                data.Add("lastname", "");
                data.Add("curl", failurl);
                data.Add("address1", "");
                data.Add("address2", "");
                data.Add("city", "");
                data.Add("state", "");
                data.Add("country", "");
                data.Add("zipcode", "");
                data.Add("udf1", "");
                data.Add("udf2", "");
                data.Add("udf3", "");
                data.Add("udf4", "");
                data.Add("udf5", "");
                data.Add("pg", "");

                strForm = PreparePOSTForm(action1, data);
            }

            OrderModel orderModel = new OrderModel();
            orderModel.strformPayU = strForm;
            orderModel.orderId = orderId;
            order = orderModel;

            return mainva;
        }

        public async Task<RazorPayVM> CapturePayU(string key, string code, string merchantid, string env)
        {
            RazorPayVM razorPay = new RazorPayVM();
            bool PaymentStatus = false;

            try
            {
                string[] merc_hash_vars_seq;
                string merc_hash_string;
                string merc_hash;
                string order_id;
                string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";
                var form = CurrentHttpContext.Request.Form;

                if (form["status"] == "success")
                {
                    merc_hash_vars_seq = hash_seq.Split('|');
                    Array.Reverse(merc_hash_vars_seq);
                    merc_hash_string = code + "|" + form["status"];

                    if (!string.IsNullOrEmpty(form["additionalCharges"]))
                        merc_hash_string = form["additionalCharges"] + "|" + code + "|" + form["status"];

                    foreach (string merc_hash_var in merc_hash_vars_seq)
                    {
                        merc_hash_string += "|";
                        merc_hash_string += form[merc_hash_var].ToString() ?? "";
                    }

                    merc_hash = Generatehash512(merc_hash_string).ToLower();

                    if (merc_hash != form["hash"])
                    {
                        // hash mismatch
                    }
                    else
                    {
                        order_id = form["txnid"];

                        if (VerifyPayment(order_id, form["mihpayid"].ToString(), key, env, code))
                        {
                            razorPay =await SetdataByOrderId(order_id);
                            PaymentStatus = true;
                            razorPay.PaymentStatus = PaymentStatus;
                        }
                        else
                        {
                            razorPay.PaymentStatus = PaymentStatus;
                            return razorPay;
                        }
                    }
                }
                else
                {
                    razorPay.PaymentStatus = PaymentStatus;
                    return razorPay;
                }
            }
            catch (Exception)
            {
                razorPay.PaymentStatus = PaymentStatus;
                return razorPay;
            }

            return razorPay;
        }

        public bool VerifyPayment(string txnid, string mihpayid, string keys, string env, string code)
        {
            string command = "verify_payment";
            string hashstr = keys + "|" + command + "|" + txnid + "|" + code;
            string hash = Generatehash512(hashstr);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            var request = (HttpWebRequest)WebRequest.Create(getURL_PayUVerify(env));

            var postData = "key=" + Uri.EscapeDataString(keys);
            postData += "&hash=" + Uri.EscapeDataString(hash);
            postData += "&var1=" + Uri.EscapeDataString(txnid);
            postData += "&command=" + Uri.EscapeDataString(command);
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            using var response = (HttpWebResponse)request.GetResponse();
            using var reader = new System.IO.StreamReader(response.GetResponseStream());
            var responseString = reader.ReadToEnd();

            return responseString.Contains("\"mihpayid\":\"" + mihpayid + "\"") && responseString.Contains("\"status\":\"success\"");
        }

        public string Generatehash512(string text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);
            string hex = "";
            using (var hashAlg = SHA512.Create())
            {
                byte[] hashValue = hashAlg.ComputeHash(message);
                foreach (byte x in hashValue)
                {
                    hex += string.Format("{0:x2}", x);
                }
            }
            return hex;
        }

        private string PreparePOSTForm(string url, Dictionary<string, string> data)
        {
            string formID = "PostForm";
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\">");

            foreach (var kv in data)
            {
                strForm.Append("<input type=\"hidden\" name=\"" + kv.Key + "\" value=\"" + kv.Value + "\">");
            }

            strForm.Append("</form>");
            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." + formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");

            return strForm.ToString() + strScript.ToString();
        }

        public string getURL_PayU(string env)
        {
            if (env == "test") return "https://test.payu.in";
            if (env == "prod") return "https://secure.payu.in";
            return "https://test.payu.in";
        }

        public string getURL_PayUVerify(string env)
        {
            if (env == "test") return "https://test.payu.in/merchant/postservice?form=2";
            if (env == "prod") return "https://secure.payu.in/merchant/postservice?form=2";
            return "https://test.payu.in/merchant/postservice?form=2";
        }
        #endregion

        #region Cashfree
        public string MethodCashfree(double totaamount, string orderId, string Key, string code, string merchantid, string env, string service, out OrderModel order)
        {
            string strForm;
            CustomerVM BD = Session.GetObject<CustomerVM>("customer");

            string mainva = "";
            var baseurl = GetBaseURL(service);
            formtorequest model = new formtorequest();

            model.appId = code;
            model.orderId = orderId;
            model.orderAmount = totaamount.ToString();
            model.orderCurrency = "INR";
            model.orderNote = "Booking";
            model.customerName = BD.FirstName;
            model.customerEmail = BD.UserName;
            model.customerPhone = Convert.ToString(BD.Mobile);
            model.returnUrl = GetSiteRoot() + baseurl.SuccessURl;
            model.notifyUrl = GetSiteRoot() + baseurl.RedirectURL;

            string secretKey = Key;
            string signatureData = "";
            PropertyInfo[] keys = model.GetType().GetProperties();
            keys = keys.OrderBy(key => key.Name).ToArray();

            foreach (PropertyInfo key in keys)
            {
                signatureData += key.Name + key.GetValue(model);
            }
            using var hmacsha256 = new HMACSHA256(StringEncode(secretKey));
            byte[] gensignature = hmacsha256.ComputeHash(StringEncode(signatureData));
            string signature = Convert.ToBase64String(gensignature);

            strForm = preparePostFormCashfree(getURL_Cashfree(env), model, signature);

            OrderModel orderModel = new OrderModel();
            orderModel.strformPayU = strForm;
            orderModel.orderId = orderId;
            order = orderModel;

            return mainva;
        }

        public async Task<RazorPayVM> CaptureCashFree(string key)
        {
            RazorPayVM razorPay = new RazorPayVM();
            bool PaymentStatus = false;

            try
            {
                var form = CurrentHttpContext.Request.Form;
                string secretKey = key;
                string orderId = form["orderId"];
                string orderAmount = form["orderAmount"];
                string referenceId = form["referenceId"];
                string txStatus = form["txStatus"];
                string paymentMode = form["paymentMode"];
                string txMsg = form["txMsg"];
                string txTime = form["txTime"];
                string signature = form["signature"];

                string signatureData = orderId + orderAmount + referenceId + txStatus + paymentMode + txMsg + txTime;

                using var hmacsha256 = new HMACSHA256(StringEncode(secretKey));
                byte[] gensignature = hmacsha256.ComputeHash(StringEncode(signatureData));
                string computedsignature = Convert.ToBase64String(gensignature);

                if (signature == computedsignature)
                {
                    if (txStatus.ToLower() == "success")
                    {
                        razorPay =await SetdataByOrderId(orderId);
                        PaymentStatus = true;
                        razorPay.PaymentStatus = PaymentStatus;
                    }
                }
            }
            catch (Exception)
            {
                razorPay.PaymentStatus = PaymentStatus;
                return razorPay;
            }

            return razorPay;
        }

        public static byte[] StringEncode(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        public string getURL_Cashfree(string env)
        {
            if (env == "test") return "https://test.cashfree.com/billpay/checkout/post/submit";
            if (env == "prod") return "https://www.cashfree.com/checkout/post/submit";
            return "https://test.cashfree.com/billpay/checkout/post/submit";
        }

        private string preparePostFormCashfree(string url, formtorequest formtorequest, string signature)
        {
            string formID = "PostForm";
            StringBuilder strForm = new StringBuilder();
            strForm.Append("<form id=\"" + formID + "\" name=\"" + formID + "\" action=\"" + url + "\" method=\"POST\">");
            strForm.Append("  <div class=\"header\">");
            strForm.Append("   <br/>");
            strForm.Append("   <h3 class=\"text -center\">Please Wait.......</h3>");
            strForm.Append("     <br/>");
            strForm.Append("    </div>");

            strForm.Append("<input type=\"hidden\" name=\"signature\" value=\"" + signature + "\">");
            strForm.Append("<input type=\"hidden\" name=\"orderNote\" value=\"" + formtorequest.orderNote + "\">");
            strForm.Append("<input type=\"hidden\" name=\"orderCurrency\" value=\"" + formtorequest.orderCurrency + "\">");
            strForm.Append("<input type=\"hidden\" name=\"customerName\" value=\"" + formtorequest.customerName + "\">");
            strForm.Append("<input type=\"hidden\" name=\"customerEmail\" value=\"" + formtorequest.customerEmail + "\">");
            strForm.Append("<input type=\"hidden\" name=\"customerPhone\" value=\"" + formtorequest.customerPhone + "\">");
            strForm.Append("<input type=\"hidden\" name=\"orderAmount\" value=\"" + formtorequest.orderAmount + "\">");
            strForm.Append("<input type=\"hidden\" name=\"notifyUrl\" value=\"" + formtorequest.notifyUrl + "\">");
            strForm.Append("<input type=\"hidden\" name=\"returnUrl\" value=\"" + formtorequest.returnUrl + "\">");
            strForm.Append("<input type=\"hidden\" name=\"appId\" value=\"" + formtorequest.appId + "\">");
            strForm.Append("<input type=\"hidden\" name=\"orderId\" value=\"" + formtorequest.orderId + "\">");
            strForm.Append("</form>");

            StringBuilder strScript = new StringBuilder();
            strScript.Append("<script language='javascript'>");
            strScript.Append("var v" + formID + " = document." + formID + ";");
            strScript.Append("v" + formID + ".submit();");
            strScript.Append("</script>");

            return strForm.ToString() + strScript.ToString();
        }
        #endregion

        #region Razorpay
        //public string MethodRazorpay(double totaamount, string orderId, string Key, string code, string merchantid, string env, out OrderModel order, TraviYo.Flight.Models.FlightViewModel.ProfileSettings profileSettings)
        //{
        //    string mainva = "";
        //    CustomerVM BD = Session.GetObject<CustomerVM>("customer") ?? new CustomerVM();

        //    #region RazorPay
        //    //Random randomObj = new Random();
        //    //string transactionId = randomObj.Next(10000000, 100000000).ToString();

        //    //MethodRazorpay.Api.RazorpayClient client = new Razorpay.Api.RazorpayClient(Key, code);
        //    //Dictionary<string, object> options = new Dictionary<string, object>();
        //    //options.Add("amount", (totaamount * 100)); // Amount in paise
        //    //options.Add("receipt", transactionId);
        //    //options.Add("currency", "INR");
        //    //options.Add("payment_capture", "0"); // 1 - automatic, 0 - manual
        //    //Razorpay.Api.Order orderResponse = client.Order.Create(options);

        //    //OrderModel orderModel = new OrderModel
        //    //{
        //    //    orderId = orderResponse.Attributes["id"],
        //    //    razorpayKey = Key,
        //    //    amount = Convert.ToDecimal(totaamount),
        //    //    currency = "INR",
        //    //    name = BD.FirstName,
        //    //    contactNumber = BD.Mobile,
        //    //    email = BD.UserName,
        //    //    logo = profileSettings.Logo,
        //    //    companyname = profileSettings.CompanyName,
        //    //    description = "Portal Transaction",
        //    //};
        //    #endregion

        //   order = null;
        //    Session.Remove("LocalOrderId");
        //    return mainva;
        //}

        //public RazorPayVM CaptureRazorPay(string key, string secret)
        //{
        //    RazorPayVM razorPay = new RazorPayVM();
        //    bool PaymentStatus = false;

        //    try
        //    {
        //        var query = CurrentHttpContext.Request.Query;
        //        string paymentId = query["rzp_paymentid"];
        //        string orderid = query["rzp_orderid"];

        //        //RazorPage.Api.RazorpayClient client = new RazorPage.Api.RazorpayClient.Api.RazorpayClient(key, secret);
        //        //Razorpay.Api.Payment payment = client.Payment.Fetch(paymentId);

        //        //Dictionary<string, object> options = new Dictionary<string, object>();
        //        //options.Add("amount", payment.Attributes["amount"]);
        //        //Razorpay.Api.Payment paymentCaptured = payment.Capture(options);
        //        //string amt = paymentCaptured.Attributes["amount"];
        //        //decimal Amount = Convert.ToDecimal(amt) / 100;

        //        //if (paymentCaptured.Attributes["status"] == "captured")
        //        //{
        //        //    razorPay = SetdataByOrderId(orderid);
        //        //    PaymentStatus = true;
        //        //    razorPay.PaymentStatus = PaymentStatus;
        //        //}
        //    }
        //    catch (Exception)
        //    {
        //        razorPay.PaymentStatus = PaymentStatus;
        //        return razorPay;
        //    }

        //    return razorPay;
        //}
        #endregion

        #region easebuzz
        public string MethodEaseBuzz(double totaamount, string orderId, string Key, string salt, string env, string service)
        {
            CustomerVM BD = Session.GetObject<CustomerVM>("customer") ?? new CustomerVM();
            string mainva = "";
            var baseurl = GetBaseURL(service);

            string amount = totaamount.ToString();
            string firstname = BD.FirstName.Trim();
            string email = BD.Email;
            string phone = BD.Mobile.Replace("+91", " ").Trim();
            string productinfo = "Booking";
            string surl = GetSiteRoot() + baseurl.SuccessURl;
            string furl = GetSiteRoot() + baseurl.FailURl;
            string Txnid = orderId.Trim();
            string UDF1 = ClientId.ToString();
            string UDF2 = "", UDF3 = "", UDF4 = "", UDF5 = "";
            string UDF6 = "", UDF7 = "", UDF8 = "", UDF9 = "", UDF10 = "";
            string Show_payment_mode = "";
            string split_payments = "";
            string sub_merchant_id = "";

            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("txnid", Txnid);
            dict.Add("key", Key);
            dict.Add("amount", amount);
            dict.Add("firstname", firstname.Trim());
            dict.Add("email", BD.Email.Trim());
            dict.Add("phone", BD.Mobile.Trim());
            dict.Add("productinfo", productinfo.Trim());
            dict.Add("surl", surl.Trim());
            dict.Add("furl", furl.Trim());
            dict.Add("udf1", UDF1.Trim());
            dict.Add("udf2", UDF2.Trim());
            dict.Add("udf3", UDF3.Trim());
            dict.Add("udf4", UDF4.Trim());
            dict.Add("udf5", UDF5.Trim());
            dict.Add("udf6", UDF6.Trim());
            dict.Add("udf7", UDF7.Trim());
            dict.Add("udf8", UDF8.Trim());
            dict.Add("udf9", UDF9.Trim());
            dict.Add("udf10", UDF10.Trim());
            dict.Add("show_payment_mode", Show_payment_mode.Trim());

            if (split_payments.Length > 0) dict.Add("split_payments", split_payments);
            if (sub_merchant_id.Length > 0) dict.Add("sub_merchant_id", sub_merchant_id);

            string result = initiatePaymentAPI(dict, salt, env);
            bool isUri = Uri.IsWellFormedUriString(result, UriKind.RelativeOrAbsolute);
            if (isUri)
            {
                mainva = result;
                return mainva;
            }

            return mainva;
        }

        public async Task<RazorPayVM> CaptureEaseBuzz(string Salt)
        {
            RazorPayVM razorPay = new RazorPayVM();
            bool PaymentStatus = false;

            try
            {
                var form = CurrentHttpContext.Request.Form;
                string[] merc_hash_vars_seq;
                string merc_hash_string;
                string merc_hash;
                string order_id;
                string hash_seq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";
                merc_hash_vars_seq = hash_seq.Split('|');
                Array.Reverse(merc_hash_vars_seq);
                merc_hash_string = Salt + "|" + form["status"];

                foreach (string merc_hash_var in merc_hash_vars_seq)
                {
                    merc_hash_string += "|";
                    merc_hash_string += form[merc_hash_var].ToString() ?? "";
                }
                merc_hash = Easebuzz_Generatehash512(merc_hash_string).ToLower();

                if (merc_hash != form["hash"])
                {
                    // mismatch
                }
                else
                {
                    order_id = form["txnid"];
                    if (form["status"] == "success")
                    {
                        razorPay =await SetdataByOrderId(order_id);
                        PaymentStatus = true;
                        razorPay.PaymentStatus = PaymentStatus;
                    }
                }
            }
            catch (Exception)
            {
                razorPay.PaymentStatus = PaymentStatus;
                return razorPay;
            }

            return razorPay;
        }

        public async Task<RazorPayVM> SetdataByOrderId(string order_id)
        {
            RazorPayVM razorPayVM = await ValidateOrderId(order_id);

            CustomerVM BD = JsonConvert.DeserializeObject<CustomerVM>(razorPayVM.CustomerSession);
            Session.SetObject("customer", BD);

            if (BD.FlightBookingDetals != null)
                Session.SetObject("Cardetails", JsonConvert.DeserializeObject<TravelSummaryViewModel>(BD.FlightBookingDetals));

            if (BD.InsAuthRes != null)
                Session.SetObject("customer", JsonConvert.DeserializeObject<CustomerVM>(BD.InsAuthRes));

          

            return razorPayVM;
        }

        internal string initiatePaymentAPI(Dictionary<string, string> dict, string salt, string env)
        {
            string result = "";

            if (emptyValidation(dict, salt))
            {
                var obj = new
                {
                    status = "0",
                    data = "Mandatory parameter " + empty_value + " can not empty"
                };
                return JsonConvert.SerializeObject(obj);
            }
            else
            {
                string hashVarsSeq = dict["key"] + "|" + dict["txnid"] + "|" + dict["amount"] + "|" + dict["productinfo"] + "|" + dict["firstname"] + "|"
                    + dict["email"] + "|" + dict["udf1"] + "|" + dict["udf2"] + "|" + dict["udf3"] + "|" + dict["udf4"] + "|" + dict["udf5"] + "|" + dict["udf6"] + "|" + dict["udf7"] + "|"
                    + dict["udf8"] + "|" + dict["udf9"] + "|" + dict["udf10"] + "|" + salt;

                gen_hash = Easebuzz_Generatehash512(hashVarsSeq).ToLower();
                dict.Add("hash", gen_hash);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var client = new RestClient(getURL(env));
                RestRequest request = new RestRequest("/payment/initiateLink");

                foreach (var data in dict)
                {
                    request.AddParameter(data.Key, data.Value);
                }

                var response = client.Post(request);
                var responseDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content.ToString());

                string is_enable_iframe = "false";
                string is_enable_seamless = "false";
                if (responseDict != null && responseDict["status"] == "1")
                {
                    if (!string.IsNullOrEmpty(responseDict["data"]))
                    {
                        if (is_enable_seamless == "true" || is_enable_iframe == "true")
                        {
                            result = responseDict["data"];
                        }
                        else
                        {
                            result = getURL(env) + "/pay/" + responseDict["data"];
                        }
                    }
                }
                else
                {
                    result = response.Content.ToString();
                }

                return result;
            }
        }

        public bool emptyValidation(Dictionary<string, string> dictionary, string salt)
        {
            bool isValid = false;

            if (dictionary != null)
            {
                if (string.IsNullOrEmpty(dictionary["key"])) { empty_value = "Merchant Key"; isValid = true; }
                else if (string.IsNullOrEmpty(dictionary["txnid"])) { empty_value = "Transaction Id"; isValid = true; }
                else if (string.IsNullOrEmpty(dictionary["amount"])) { empty_value = "Amount"; isValid = true; }
                else if (string.IsNullOrEmpty(dictionary["productinfo"])) { empty_value = "Product Infomation"; isValid = true; }
                else if (string.IsNullOrEmpty(dictionary["firstname"])) { empty_value = "First Name"; isValid = true; }
                else if (string.IsNullOrEmpty(dictionary["email"])) { empty_value = "Email"; isValid = true; }
                else if (string.IsNullOrEmpty(dictionary["phone"])) { empty_value = "Phone"; isValid = true; }
                else if (!string.IsNullOrEmpty(dictionary["phone"]))
                {
                    if (dictionary["phone"].Length != 10)
                    {
                        empty_value = "Phone number must be 10 digit";
                        isValid = true;
                    }
                }
                else if (string.IsNullOrEmpty(dictionary["surl"])) { empty_value = "Success URL"; isValid = true; }
                else if (string.IsNullOrEmpty(dictionary["furl"])) { empty_value = "Failure URL"; isValid = true; }
                else if (string.IsNullOrEmpty(salt)) { empty_value = "Merchant Salt Key"; isValid = true; }
            }

            return isValid;
        }

        public string Easebuzz_Generatehash512(string text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);
            string hex = "";
            using (var hashAlg = SHA512.Create())
            {
                byte[] hashValue = hashAlg.ComputeHash(message);
                foreach (byte x in hashValue)
                {
                    hex += string.Format("{0:x2}", x);
                }
            }
            return hex;
        }

        public string getURL(string env)
        {
            if (env == "test") return "https://testpay.easebuzz.in";
            if (env == "prod") return "https://pay.easebuzz.in";
            return "https://testpay.easebuzz.in";
        }
        #endregion

        #region Common Save Transactions
        public async Task SavetransactionDetails(string Id, string Amt, string CustomerSession)
        {
          await  _razor.SaveTransactionDetails(Id, Amt, CustomerSession);
        }

        public async Task<RazorPayVM> ValidateOrderId(string Id)
        {
            return await _razor.ValidateOrderId(Id);
        }
        #endregion

        #region BaseURL
        public BaseURL GetBaseURL(string MethodName)
        {
            BaseURL baseURL = new BaseURL();
            switch (MethodName)
            {
                case "Cabbooking":
                    baseURL.SuccessURl = "/Booking/Index";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/ThankYou/YourBooking";
                    break;
                case "hotel":
                    baseURL.SuccessURl = "/HotelDetail/HotelBookingComplete";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/ThankYou/YourHotelBooking";
                    break;
                case "hoteloffline":
                    baseURL.SuccessURl = "/Bookings/HotelBookingComplete";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/ThankYou/YourHotelBooking";
                    break;
                case "package":
                    baseURL.SuccessURl = "/Bookings/PackageComplete";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/ThankYou/YourPkgBooking";
                    break;
                case "grouppackage":
                    baseURL.SuccessURl = "/Bookings/GroupPackageComplete";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/ThankYou/YourPkgBooking";
                    break;
                case "activity":
                    baseURL.SuccessURl = "/Bookings/Complete";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/ThankYou/YourActBooking";
                    break;
                case "wallet":
                    baseURL.SuccessURl = "/Wallet/Complete";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/Wallet/Index";
                    break;
                case "visa":
                    baseURL.SuccessURl = "/Visa/Complete";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/Thankyou/YourVisaBooking";
                    break;
                case "bus":
                    baseURL.SuccessURl = "/Bus/ConfirmBusBooking";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/Thankyou/YourBusBooking";
                    break;
                case "travelInsurance":
                    baseURL.SuccessURl = "/TravelInsurance/Confirmation";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/Thankyou/yourtravelinsurancebooking";
                    break;
                case "transfer":
                    baseURL.SuccessURl = "/Transfer/TransferConfirmation";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/Thankyou/yourtransferbooking";
                    break;
                case "flightairiq":
                    baseURL.SuccessURl = "/FdFlights/CompleteBooking";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/FdFlights/BookingConfirmation";
                    break;
                case "flightfdking":
                    baseURL.SuccessURl = "/FdFlights/CompleteBookingFdKing";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/FdFlights/BookingConfirmationFdking";
                    break;
                case "flightselfseries":
                    baseURL.SuccessURl = "/SeriesFlights/ProcessingBooking";
                    baseURL.FailURl = "/PayNow/PaymentCancel";
                    baseURL.RedirectURL = "/ThankYou/YourBooking";
                    break;
            }
            return baseURL;
        }
        #endregion
    }

    

    //public interface IHomeRepository
    //{
    //    void SavetransactionDetails(string id, string amount, string customerSession);
    //    RazorPayVM ValidateOrderId(string id);
    //}
}