using System.Collections.Generic;


namespace Golf.Domain.Shared.Setting
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int Expires { get; set; }
        public int RefreshExpires { get; set; }
        public int RefreshTokenTTL { get; set; }
        //Odoo 
        public string DbOdoo { get; set; }
        public string BaseOdooUrl { get; set; }
        public string OdooCookieKey { get; set; }
        public string OdooUserID { get; set; }
        public string OdooPartnerID { get; set; }
        //Auth
        public string AuthLogin { get; set; }
        public string AuthSignUp { get; set; }
        public string UpdateProfile { get; set; }
        public string ChangePassword { get; set; }
        public string ForgotPassword { get; set; }
        //Product
        public string ProductTemplate { get; set; }
        public string ProductDetail { get; set; }
        public string ProductCategory { get; set; }
        public string ProductReview { get; set; }
        public string AddProductReview { get; set; }
        public string AddProductReviewImage { get; set; }
        public string RemoveProductReviewImage { get; set; }
        //booking
        public string CourseBooking { get; set; }
        //Cart
        public string Cart { get; set; }
        public string AddToCart { get; set; }
        public string RemoveFromCart { get; set; }
        public string SubmitCart { get; set; }
        //Promotion
        public string PromotionCodeDetail { get; set; }
        public string ListPromotionCode { get; set; }
        public string ChoosePromotionCode { get; set; }
        public string UnapplyPromotionCode { get; set; }
        //wishlist
        public string WishList { get; set; }
        public string WishListAdd { get; set; }
        public string WishListRemove { get; set; }
        //adress
        public string Address { get; set; }
        public string ListCountry { get; set; }
        public string ListState { get; set; }
        public string MyAddress { get; set; }
        public string ChooseAddress { get; set; }
        //Membership
        public string Membership { get; set; }
        public string BuyMembership { get; set; }
        public string MyMembership { get; set; }

        //Odoo
        public List<string> SupportedClientIds { get; set; }
        public string getUrl(string url)
        {
            return BaseOdooUrl + url;
        }
        public string getUpdateProfileUrl()
        {
            return BaseOdooUrl + UpdateProfile;
        }
        public string getAuthLoginUrl()
        {
            return BaseOdooUrl + AuthLogin;
        }
        public string getAuthSignUpUrl()
        {
            return BaseOdooUrl + AuthSignUp;
        }
        public string getProductTemplateUrl()
        {
            return BaseOdooUrl + ProductTemplate;
        }
        public string getProductCategoryUrl()
        {
            return BaseOdooUrl + ProductCategory;
        }
        public string getForgotPasswordUrl()
        {
            return BaseOdooUrl + ForgotPassword;
        }
        public string getChangePasswordUrl()
        {
            return BaseOdooUrl + ChangePassword;
        }
        public string getProductDetail()
        {
            return BaseOdooUrl + ProductDetail;
        }
        public string getProductReview()
        {
            return BaseOdooUrl + ProductReview;
        }
        public string getPromotionCode()
        {
            return BaseOdooUrl + PromotionCodeDetail;
        }
    }
}
