using Golf.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Golf.Domain.Messages
{
    public class Conversation : ISafeEntityBase
    {
        public string? Name { get; set; }      
        public string? Image { get; set; }      
        public string GolferIDs { get; set; } ="";
        public List<Guid> ReaderIDs { get; set; } = new List<Guid>();
        public bool IsEmpty { get; set; } = true;

       public void AddGolferID(Guid id)
        {
            if(this.GolferIDs==""||this.GolferIDs==null)
            {
                this.GolferIDs = id.ToString();
            }    
            else
            {
                this.GolferIDs = this.GolferIDs + "," + id.ToString();
            }                
        } 
        public void RemoveGolferID(Guid id)
        {
            if(this.GolferIDs==""||this.GolferIDs==null)
            {
                return;
            }    
            else
            {
                this.GolferIDs=this.GolferIDs.Replace("," + id.ToString(), "");
                this.GolferIDs=this.GolferIDs.Replace(id.ToString() + ",", "");
            }
        }  
        public bool read(Guid golferID)
        {
           if(this.ReaderIDs.FindAll(rd=>rd==golferID).Count()>0)
            {
                return true;
            }
            return false;
        }
        public List<Guid> GetGolferIDs()
        {
            List<Guid> result = new List<Guid>();
            if (this.GolferIDs != "" && this.GolferIDs != null)
            {
                var tmp=this.GolferIDs.Split(",");
                foreach(var i in tmp)
                {
                    if(i!="")
                    {
                        result.Add(new Guid(i));
                    }    
                    
                }
            }
            return result;
        }
        public List<Guid> GetReceivers(Guid SenderID)
        {
            List<Guid> result = new List<Guid>();
            foreach(var i in this.GetGolferIDs())
            {
                if (i.ToString().Contains(SenderID.ToString()) == false)
                    result.Add(i);
              
            }
            return result;
        }
    }
}
