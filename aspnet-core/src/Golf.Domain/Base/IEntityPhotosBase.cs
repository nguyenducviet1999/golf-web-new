using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Base
{
    public interface IEntityPhotosBase
    {
        public string PhotoNames { get; set; }
        public List<string> GetPhotoNames()
        {
            List<string> result = new List<string>();
            if (this.PhotoNames != "" && this.PhotoNames != null)
            {
                var tmp = this.PhotoNames.Split(",");
                foreach (var i in tmp)
                {
                    if (i != "")
                    {
                        result.Add(i);
                    }

                }
            }
            return result;
        }
        public void AddPhotoName(string photoName)
        {
            if (this.PhotoNames == "" || this.PhotoNames == null)
            {
                this.PhotoNames = photoName.ToString();
            }
            else
            {
                this.PhotoNames = this.PhotoNames + "," + photoName.ToString();
            }
        }
        public void RemovePhotoName(string photoName)
        {
            if (this.PhotoNames == "" || this.PhotoNames == null)
            {
                return;
            }
            else
            {
                this.PhotoNames = this.PhotoNames.Replace("," + photoName.ToString(), "");
                this.PhotoNames = this.PhotoNames.Replace(photoName.ToString() + ",", "");
            }
        }
        public void RemovePhotoNames(List<string> photoNames)
        {
            foreach (var i in photoNames)
            {
                this.RemovePhotoName(i);
            }
        }
        public void AddPhotoNames(List<string> photoNames)
        {
            foreach (var i in photoNames)
            {
                this.AddPhotoName(i);
            }
        }
        public void SetPhotoNames(List<string> photoNames)
        {
            var tmp = "";
            foreach (var i in photoNames)
            {
                if (tmp == "")
                {
                    tmp = tmp + i;
                }
                else
                {
                    tmp = tmp + "," + i;
                }
            }
            this.PhotoNames = tmp;
        }
    }
}
