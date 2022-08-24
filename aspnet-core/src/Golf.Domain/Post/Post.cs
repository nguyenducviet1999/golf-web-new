using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Golf.Domain.Shared.Post;
using Golf.Domain.Base;
using Golf.Domain.SocialNetwork;
using Golf.Domain.GolferData;

namespace Golf.Domain.Post
{
    public class Post : ISafeEntityBase
    {
        [ForeignKey("Owner")]
        public Guid OwnerID { get; set; }
        public virtual Golfer Owner { get; set; }
        public PostPrivacyLevel Privacy { get; set; } = PostPrivacyLevel.Public;
        public string Content { get; set; }
        [Column(TypeName = "jsonb")]
        public List<Guid> TagIDs { get; set; } = new List<Guid>();
        public string PhotoNames { get; set; } = "";
        [Column(TypeName = "jsonb")]
        public List<ReferenceObject> ReferenceObject { get; set; } = new List<ReferenceObject>();
#nullable enable
        [ForeignKey("Parent")]
        public Guid? ParentID { get; set; }
        public virtual Post? Parent { get; set; }
        [ForeignKey("Group")]
        public Guid? GroupID { get; set; }
        public bool? IsPin { get; set; } = false;
        public virtual Group? Group { get; set; }
        public PostAction? PostAction { get; set; }

        public List<Guid> ScorecardIDs()
        {
            List<Guid> result = new List<Guid>();
            foreach(var i in this.ReferenceObject)
            {
                if (i.Type == ReferenceObjectType.Scorecard)
                    result.Add(i.ID);
            }
            return result;
        }
        public List<Guid> EventIDs()
        {
            List<Guid> result = new List<Guid>();
            foreach(var i in this.ReferenceObject)
            {
                if (i.Type == ReferenceObjectType.Event)
                    result.Add(i.ID);
            }
            return result;
        } 
        public List<Guid> TournamentIDs()
        {
            List<Guid> result = new List<Guid>();
            foreach(var i in this.ReferenceObject)
            {
                if (i.Type == ReferenceObjectType.Tournament)
                    result.Add(i.ID);
            }
            return result;
        }
        public void SetScorecardIDs(List<Guid> scorecardIDs)
        {

            foreach (var i in scorecardIDs)
            {
                this.ReferenceObject.Add(new ReferenceObject() {ID=i,Type=ReferenceObjectType.Scorecard });
            }
        }
        public void SetEventIDs(List<Guid> eventIDs)
        {

            foreach (var i in eventIDs)
            {
                this.ReferenceObject.Add(new ReferenceObject() {ID=i,Type=ReferenceObjectType.Event });
            }
        }
        public void SetEventID(Guid eventID)
        {

                this.ReferenceObject.Add(new ReferenceObject() {ID= eventID, Type=ReferenceObjectType.Event });
          
        } public void SetTournamentID(Guid tournamentID)
        {

                this.ReferenceObject.Add(new ReferenceObject() {ID= tournamentID, Type=ReferenceObjectType.Tournament });
          
        } public void SetScorecardID(Guid scorecardID)
        {

                this.ReferenceObject.Add(new ReferenceObject() {ID= scorecardID, Type=ReferenceObjectType.Scorecard });
          
        }
        public List<string> GetPhotoNames()
        {
            List<string> result = new List<string>();
            if (this.PhotoNames != "" && this.PhotoNames != null)
            {
                var tmp = this.PhotoNames.Split(",");
                foreach (var i in tmp)
                {
                    if(i!="")
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
                this.PhotoNames = this.PhotoNames.Replace(photoName.ToString()+",", "");
            }
        }
        public void RemovePhotoNames(List<string> photoNames)
        {
            foreach(var i in photoNames)
            {
                this.RemovePhotoName(i);
            }
        }
        public void AddPhotoNames(List<string> photoNames)
        {
            foreach(var i in photoNames)
            {
                this.AddPhotoName(i);
            }
        }
        public void SetPhotoNames(List<string> photoNames)
        {
            var tmp = "";
            foreach(var i in photoNames)
            {
                if(tmp=="")
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