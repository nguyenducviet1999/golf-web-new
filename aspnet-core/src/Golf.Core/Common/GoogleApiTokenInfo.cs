namespace Golf.Core.Common
{
    public class GoogleApiTokenInfo
    {
        public string iss { get; set; }
        public string at_hash { get; set; }
        /// <summary>
        /// Identifies the audience that this ID token is intended for.
        /// It must be one of the OAuth 2.0 client IDs of your application.
        /// </summary>
        public string aud { get; set; }
        public string sub { get; set; }
        public string email_verified { get; set; }
        public string azp { get; set; }
        public string email { get; set; }
        public string iat { get; set; }
        public string exp { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string locale { get; set; }
        public string alg { get; set; }
        public string kid { get; set; }
    }
}