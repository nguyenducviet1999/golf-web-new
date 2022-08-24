using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Golf.Core.Dtos.Post;
using Golf.Core.Dtos.Controllers.ProfileController.Requests;
using Golf.Domain.Courses;
using Golf.Domain.SocialNetwork;
using Golf.Core.Common.Golfer;
using Golf.Core.Dtos.Groups;
using Golf.Core.Dtos.Controllers.GroupController.Requests;
using Golf.Core.Common.Course;
using Golf.Core.Dtos.Controllers.BookingController.Responses;
using Golf.Domain.Post;
using Golf.Core.Dtos.Controllers.PostController.Requests;
using Golf.Domain.Bookings;
using Golf.Core.Dtos.Controllers.CoursesController.Responses;
using Golf.Domain.Notifications;
using Golf.Core.Dtos.Controllers.NotificationController.Respone;
using Golf.Domain.Scorecard;
using Golf.Core.Common.Scorecard;
using Golf.Core.Common.Post;
using Golf.Core.Dtos.Controllers.EventController.Response;
using Golf.Domain.Events;
using Golf.Core.Dtos.Controllers.EventController.Request;
using Golf.Domain.Tournaments;
using Golf.Core.Dtos.Controllers.TournamentController.Response;
using Golf.Core.Dtos.Controllers.TournamentController.Request;
using Golf.Core.Dtos.Controllers.AdminController.Tournament;
using Golf.Core.Dtos.Controllers.ShopControler.Response;
using Golf.Core.Dtos.Controllers.ShopControler.Request;
using Golf.Core.Dtos.Controllers.MembershipController.Responses;
using Golf.Core.Dtos.Controllers.OdooResourcesController.Response;
using Golf.Core.Common.Odoo.OdooResponse;

namespace Golf.DataMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Golf.Domain.GolferData.Golfer, MinimizedGolfer>()
                .ForMember(minimizedGolfer => minimizedGolfer.FullName, opt => opt.MapFrom(golfer => golfer.FirstName + " " + golfer.LastName)); 
            CreateMap<List<string>, OdooObject>()
                .ForMember(odooObject => odooObject.ID, opt => opt.MapFrom(list=>list.Count()==0?-1:int.Parse(list[0])))
                .ForMember(odooObject => odooObject.Name, opt => opt.MapFrom(list => list.Count()==0?"":list[1]));
            CreateMap<Location, BookingResponse>();
            CreateMap<Course, BookingResponse>();
            CreateMap<Course, CourseResponse>();
            CreateMap<Group, GroupResponse>();
            CreateMap<Location, CourseResponse>();
            CreateMap<Course, CourseDetailResponse>();
            CreateMap<Location, CourseDetailResponse>();
            CreateMap<Product, ProductResponse>();
            CreateMap<Product, ProductResponse>();
            CreateMap<Notification, NotificationResponse>();
            CreateMap<GroupRequest, Group>();
            CreateMap<Scorecard, MinimizedScorecard>();
            CreateMap<CommentResponse, CommentDetailResponse>();
            CreateMap<Event, EventResponse>();
            CreateMap<AddEventRequest, Event>();
            CreateMap<Tournament, TournamentResponse>();
            CreateMap<Golf.Domain.GolferData.Golfer, TournamentMemberResponse>();
            CreateMap<Tournament, TournamentAdminResponse>();
            CreateMap<TournamentRequest, Tournament>();
            CreateMap<EditGroupRequest, Group>();
            //shop odoo
            CreateMap<OdooAddressRequest, OdooAddressRequestDto>(); 
            //////////
            CreateMap<OdooProductResponseDto, OdooProductResponse>();
            CreateMap<OdooCategoryResponseDto, OdooCategoryResponse>();
            CreateMap<OdooPrductDetailResponseDto, OdooPrductDetailResponse>();
            CreateMap<ProductVariantResponseDto, ProductVariantResponse>();
            CreateMap<ProductTemplateImageResponseDto, ProductTemplateImageResponse>();
            CreateMap<AlternativeProductResponseDto, AlternativeProductResponse>();
            CreateMap<OdooProductReviewResponsesDto, OdooProductReviewResponses>();
            CreateMap<OdooProductReviewResponseDto, OdooProductReviewResponse>();
            CreateMap<OdooProductAttributeDto, OdooProductAttribute>();
            CreateMap<ProductTemplateAttributeValueDto, ProductTemplateAttributeValue>();
            //product
            CreateMap<AttachmentResponseDto, AttachmentResponse>();
            CreateMap<RatingResponseDto, RatingResponse>();
            CreateMap<OptionsResponseDto, OptionsResponse>();
            CreateMap<RatingStatsResponseDto, RatingStatsResponse>();
            CreateMap<PercentResponseDto, PercentResponse>();
            CreateMap<OdooPromotionCodeDetailResponseDto, OdooPromotionCodeDetailResponse>();
            CreateMap<OdooPromotionCodeResponseDto, OdooPromotionCodeDetailResponse>();
            CreateMap<OdooMembershipResponseDto, OdooMembershipResponse>();
            CreateMap<OdooMyMembershipResponseDto, OdooMyMembershipResponse>();
            CreateMap<OdooAdressResponseDto, OdooAdressResponse>();
            CreateMap<OdooAddressResponseDto, OdooAddressResponse>();
            CreateMap<OdooCartResponseDto, OdooCartResponse>();
            CreateMap<OdooOrderResponseDto, OdooOrderResponse>();
            CreateMap<OdooCartProductResponseDto, OdooCartProductResponse>();
            CreateMap<VariantDto, Variant>();
            CreateMap<OdooWishListProductResponseDto, OdooWishListProductResponse>();
            //odooImage
            CreateMap<OdooImageResponseDto, OdooImageResponse>();

            //CreateMap<Category, CategoryDto>()
            //    .ForMember(d => d.Skills, opt => opt.MapFrom(src => src.Skills.OrderBy(cs => cs.NumericalOrder)))
            //    .ReverseMap();
            //CreateMap<Skill, SkillDto>()
            //    .ForMember(d => d.Assignees, opt => opt.MapFrom(src => src.SkillAssignments.Select(cs => cs.Golfer)));
        }
    }
}
