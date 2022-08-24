using System;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Golf.EntityFrameworkCore.Repositories;
using Golf.Domain.Shared.Relationship;
using Golf.Domain.SocialNetwork;
using Golf.Domain.Shared;
using Golf.Core.Dtos.Controllers.GolfersController.Requests;
using Golf.Core.Common.Golfer;
using Golf.Core.Exceptions;
using Golf.Domain.GolferData;
using Golf.Services.Notifications;
using Golf.Core.Dtos.Controllers.NotificationController.Requests;
using Golf.Domain.Shared.Notifications;
using Golf.Domain.Notifications;

namespace Golf.Services
{
    public class RelationshipService
    {
        private readonly GolferRepository _golferRepository;
        private readonly RelationshipRepository _relationshipRepository;
        private readonly GroupMemberRepository _groupMemberRepository;
        private readonly IMapper _mapper;
        private readonly NotificationService _notificationService;

        public RelationshipService(
            GroupMemberRepository groupMemberRepository,
            NotificationService notificationService,
            RelationshipRepository relationshipRepository,
            GolferRepository golferRepository,
            IMapper mapper
        )
        {
            _groupMemberRepository = groupMemberRepository;
            _relationshipRepository = relationshipRepository;
            _golferRepository = golferRepository;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public string GetRelationship(Guid currentGolferID, Guid GolferID)
        {
            if (currentGolferID == GolferID)
            {
                return "IsYou";
            }
            var relationship = _relationshipRepository.Find(r =>
             (r.GolferID == currentGolferID && r.FriendID == GolferID) ||
            (r.GolferID == GolferID && r.FriendID == currentGolferID)).FirstOrDefault();
            if (relationship == null)
            {
                return "IsNotFriend";
            }
            else
            {
                switch (relationship.Status)
                {
                    case RelationshipStatus.IsFriend:
                        return "IsFriend";
                    case RelationshipStatus.Block:
                        {
                            if (relationship.GolferID == currentGolferID)
                            {
                                return "Blocked";
                            }
                            else
                            {
                                return "IsBlocked";
                            }
                        }
                    case RelationshipStatus.RequestSent:
                        {
                            if (relationship.GolferID == currentGolferID)
                            {
                                return "SentRequest";
                            }
                            else
                            {
                                return "HasRequest";
                            }
                        }
                    default:
                        throw new Exception("Error Get Current Golfer Friend Relation");
                }
            }
        }

        public bool GetRelationshipWithGolfer(Guid currentGolferID, Guid GolferID)
        {
            if (currentGolferID.ToString()== GolferID.ToString())
                return true;
            var relationship = _relationshipRepository.Find(r => (r.GolferID == currentGolferID && r.FriendID == GolferID) || (r.GolferID == GolferID && r.FriendID == currentGolferID)).FirstOrDefault();
            if (relationship == null)
            {
                return false;
            }
            else
            {
                switch (relationship.Status)
                {
                    case RelationshipStatus.IsFriend:
                        {
                            return true;
                        }
                    case RelationshipStatus.Block:
                        {
                            if (relationship.GolferID == currentGolferID)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    case RelationshipStatus.RequestSent:
                        {
                            return false;
                        }
                    default:
                        {
                            return false;
                        }
                }
            }
        }

        public List<MinimizedGolfer> SearchFriend(Guid GolferID, string searchKey, int startIndex)
        {
            var friendRelationShips = _relationshipRepository.FindWithGolfer(r => ((r.FriendID == GolferID &&(r.Golfer.LastName.Trim().ToLower().Contains(searchKey) || r.Golfer.FirstName.Trim().ToLower().Contains(searchKey)||r.Golfer.Email.Trim().ToLower().Contains(searchKey)||r.Golfer.PhoneNumber.Trim().ToLower().Contains(searchKey)))|| (r.GolferID == GolferID && (r.Friend.LastName.Trim().ToLower().Contains(searchKey)||r.Friend.FirstName.Trim().ToLower().Contains(searchKey) || r.Friend.Email.Trim().ToLower().Contains(searchKey) || r.Friend.PhoneNumber.Trim().ToLower().ToLower().Contains(searchKey)))) && r.Status == RelationshipStatus.IsFriend).Skip(startIndex).Take(Const.PageSize);
            if (friendRelationShips.Count() == 0)
                return new List<MinimizedGolfer>();
            List<MinimizedGolfer> minimizedGolfers = new List<MinimizedGolfer>();

            foreach (var i in friendRelationShips)
            {
                if(GolferID==i.GolferID)
                {
                    minimizedGolfers.Add(_mapper.Map<MinimizedGolfer>(i.Friend));
                } 
                 else
                {
                    minimizedGolfers.Add(_mapper.Map<MinimizedGolfer>(i.Golfer));
                }
            }
            return minimizedGolfers;
        }
        public List<MinimizedGolfer> SearchFriendToAddIntoGroup(Guid groupID,Guid GolferID, string searchKey, int startIndex)
        {
            var groupMember = _groupMemberRepository.Find(gm => gm.GroupID == groupID&&gm.Status!=MemberStatus.Request).ToList();
            var groupMemberIDs = groupMember.Select(gm => gm.GolferID).ToList();
            if(groupMemberIDs.Contains(GolferID)==false)
            {
                throw new ForbiddenException("Access denied!");
            }    
            var friendRelationShips = _relationshipRepository.FindWithGolfer(r => 
                                    (
                                        (   
                                            r.FriendID == GolferID 
                                            &&(r.Golfer.LastName.Trim().ToLower().Contains(searchKey) 
                                            || r.Golfer.FirstName.Trim().ToLower().Contains(searchKey)
                                            ||r.Golfer.Email.Trim().ToLower().Contains(searchKey)
                                            ||r.Golfer.PhoneNumber.Trim().ToLower().Contains(searchKey))
                                             && groupMemberIDs.Contains(r.GolferID) == false
                                        )
                                        || (
                                            r.GolferID == GolferID 
                                            &&(r.Friend.LastName.Trim().ToLower().Contains(searchKey)
                                                ||r.Friend.FirstName.Trim().ToLower().Contains(searchKey) 
                                                || r.Friend.Email.Trim().ToLower().Contains(searchKey) 
                                                || r.Friend.PhoneNumber.Trim().ToLower().ToLower().Contains(searchKey))
                                            && groupMemberIDs.Contains(r.FriendID) == false
                                         )
                                    ) 
                                    && r.Status == RelationshipStatus.IsFriend
                                   
                                    
                                    ).Skip(startIndex).Take(Const.PageSize);
            if (friendRelationShips.Count() == 0)
                return new List<MinimizedGolfer>();
            List<MinimizedGolfer> minimizedGolfers = new List<MinimizedGolfer>();

            foreach (var i in friendRelationShips)
            {
                if(GolferID==i.GolferID)
                {
                    minimizedGolfers.Add(_mapper.Map<MinimizedGolfer>(i.Friend));
                } 
                 else
                {
                    minimizedGolfers.Add(_mapper.Map<MinimizedGolfer>(i.Golfer));
                }
            }
            return minimizedGolfers;
        }
        //public List<MinimizedGolfer> SearchFriend(Guid GolferID, string searchKey, int startIndex)
        //{
        //    var friendRelationShips = _relationshipRepository.Find(r => (r.FriendID == GolferID || r.GolferID == GolferID) && r.Status == RelationshipStatus.IsFriend);
        //    if (friendRelationShips.Count() == 0)
        //        return new List<MinimizedGolfer>();
        //    var friendIDs = new List<Guid>();
        //    foreach (var i in friendRelationShips)
        //    {
        //        var fID = GolferID == i.GolferID ? i.FriendID : GolferID;
        //        friendIDs.Add(fID);
        //    }
        //    var result = _golferRepository.Find(g => friendIDs.Contains(g.Id) && (g.FirstName.Trim().ToLower().Contains(searchKey.Trim().ToLower()) || g.LastName.Trim().ToLower().Contains(searchKey.Trim().ToLower()))).Skip(startIndex).Take(Const.PageAmount);
        //    if (result.Count() == 0)
        //        return new List<MinimizedGolfer>();
        //    List<MinimizedGolfer> response = new List<MinimizedGolfer>();
        //    foreach (var j in result)
        //    {
        //        response.Add(_mapper.Map<MinimizedGolfer>(j));
        //    }
        //    return response;
        //}

        public int CountFriends(Guid GolferID)
        {
            var relationship = _relationshipRepository.CountAll(
                r => (r.GolferID == GolferID || r.FriendID == GolferID)
                 && r.Status == RelationshipStatus.IsFriend);
            return relationship;
        }

        public List<Relationship> GetAllGolferRelations(Guid GolferID)
        {
            var relationships = _relationshipRepository.Find(
                            r => (r.GolferID == GolferID || r.FriendID == GolferID) )
                                .ToList();
            return relationships;
        }

        public List<Guid> GetGolferRelationshipIDs(Guid GolferID)
        {
            List<Guid> relationshipIDs = new List<Guid>();
            var relationships = _relationshipRepository.Find(
                            r => (r.GolferID == GolferID || r.FriendID == GolferID)
                             && r.Status == RelationshipStatus.IsFriend).ToList();
            foreach (Relationship relationship in relationships)
            {
                if (relationship.GolferID == GolferID)
                {
                    relationshipIDs.Add(relationship.FriendID);
                }
                relationshipIDs.Add(relationship.GolferID);
            }
            return relationshipIDs;
        }

        public List<Relationship> GetAllGolferBlockedRelations(Guid GolferID)
        {
            var relationship = _relationshipRepository.Find(
                            r => (r.GolferID == GolferID || r.FriendID == GolferID)
                             && r.Status == RelationshipStatus.Block).ToList();
            return relationship;
        }

        public bool ChangeRelationship(Guid currentGolferID, Guid GolferID, ChangeRelationshipRequest request)
        {
            if(currentGolferID==GolferID)
            {
                return false;
            }    
            var relationship = _relationshipRepository.Find(r =>
              (r.GolferID == currentGolferID && r.FriendID == GolferID) ||
             (r.GolferID == GolferID && r.FriendID == currentGolferID)).FirstOrDefault();
            if (relationship == null)
            {
                if (request == ChangeRelationshipRequest.SendRequest)
                {
                    _relationshipRepository.Add(new Relationship
                    {
                        GolferID = currentGolferID,
                        FriendID = GolferID,
                        Status = RelationshipStatus.RequestSent,
                    });
                    //Make notification
                    _notificationService.NotificationAddFriendRequest(currentGolferID, GolferID);
                    return true;
                }
                else
                {
                    throw new BadRequestException("Send Request Error");
                }
            }
            else
            {
                switch (request)
                {
                    case ChangeRelationshipRequest.UnfriendRequest:
                        {
                            _relationshipRepository.Remove(relationship);
                            return true;
                        }
                    case ChangeRelationshipRequest.CancelRequest:
                        {
                            if (relationship.Status == RelationshipStatus.RequestSent
                                && relationship.GolferID == currentGolferID)
                            {
                                _relationshipRepository.Remove(relationship);
                                return true;
                            }
                            else
                            {
                                throw new BadRequestException("Cancel Request Error");
                            }
                        }
                    case ChangeRelationshipRequest.AcceptRequest:
                        {
                            if (relationship.Status == RelationshipStatus.RequestSent
                                && relationship.FriendID == currentGolferID)
                            {
                                relationship.Status = RelationshipStatus.IsFriend;
                                _relationshipRepository.UpdateEntity(relationship);
                                //Make notification
                            _notificationService.NotificationConfirmAddFriendRequest(currentGolferID, GolferID);
                                return true;
                            }
                            else
                            {
                                throw new BadRequestException("Accept Request Error");
                            }
                        }
                    case ChangeRelationshipRequest.RejectRequest:
                        {
                            if (relationship.Status == RelationshipStatus.RequestSent
                                && relationship.FriendID == currentGolferID)
                            {
                                _relationshipRepository.Remove(relationship);
                                return true;
                            }
                            else
                            {
                                throw new BadRequestException("Reject Request Error");
                            }
                        }
                    default:
                        {
                            throw new BadRequestException("Unknown Action");
                        }
                }
            }
        }

        public List<MinimizedGolfer> GetRelationshipGolfers(Guid currentGolferID,
            RelationshipRequestStatus relationshipRequestStatus,
            int startIndex)
        {
            var golferIDs = new List<Guid>();
            var relationships = GetAllGolferRelations(currentGolferID);
            foreach (Relationship relationship in relationships)
            {
                if (relationshipRequestStatus == RelationshipRequestStatus.Sent)
                {
                    if (relationship.GolferID == currentGolferID&&relationship.Status==RelationshipStatus.RequestSent)
                    {
                        golferIDs.Add(relationship.FriendID);
                    }
                }
                else if (relationshipRequestStatus == RelationshipRequestStatus.Received)
                {
                    if (relationship.FriendID == currentGolferID&&relationship.Status == RelationshipStatus.RequestSent)
                    {
                        golferIDs.Add(relationship.GolferID);
                    }
                }
                else
                {
                    if (relationship.GolferID == currentGolferID&& relationship.Status== RelationshipStatus.IsFriend)
                    {
                        golferIDs.Add(relationship.FriendID);
                    }
                    else if (relationship.FriendID == currentGolferID && relationship.Status == RelationshipStatus.IsFriend)
                    {
                        golferIDs.Add(relationship.GolferID);
                    }
                }
            }
            var golfers = _golferRepository.GetGolfers(golferIDs.Skip(startIndex).Take(Const.PageSize).ToList());
            return _mapper.Map<List<Golfer>, List<MinimizedGolfer>>(golfers);
        }

        public bool IsFriend(Guid curentID,Guid friendID)
        {
            if (curentID == friendID)
                return true;
            var relationship = _relationshipRepository.Find(r =>
              (r.GolferID == curentID && r.FriendID == friendID) ||
             (r.GolferID == friendID && r.FriendID == curentID)).FirstOrDefault();
            if(relationship!=null)
            {
                if(relationship.Status==RelationshipStatus.IsFriend)
                {
                    return true;
                }    
            }
            return false;
        }
    }
}