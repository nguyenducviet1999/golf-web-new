using System;
using System.Collections.Generic;
using System.Text;

namespace Golf.Domain.Common
{
    public class GPSAddressDto
    {
        public string Longitude { get; set; } //kinh độ
        public string Latitude { get; set; } // vĩ độ
        public GPSAddress GPSAddress() 
        {
            return new GPSAddress(Longitude, Latitude);
        }
    }
    public class GPSAddress
    {
        public GPSAddress() { Latitude = 91.0;Longitude = 181; }
        public GPSAddress(string longitude,string latitude) 
        {
            if(latitude==null||latitude==""||longitude==null||longitude=="")
            {
                Latitude = 91.0;
                Longitude = 181;
            }    
            else
            {
                try
                {
                    Longitude = double.Parse(longitude);
                    Latitude= double.Parse(latitude);
                }
                catch(Exception e)
                {
                    Latitude = 91.0;
                    Longitude = 181;
                }
            }
        }

        public double Longitude { get; set; } = 181.0;//kinh độ
        public double Latitude { get; set; } = 91.0;// vĩ độ
        public bool Validate()
        {
            if (Latitude < 90 && Latitude > -90 && Longitude > 0 && Longitude < 180)
            {
                return true;
            }
            return false;
        }
        public string toString()
        {
            return Latitude.ToString() + "," + Longitude.ToString();
        }
    }
}
