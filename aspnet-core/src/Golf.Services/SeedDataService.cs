using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

using Golf.EntityFrameworkCore.Repositories;
using Golf.Domain.Shared.Scorecard;
using Golf.EntityFrameworkCore;
using Golf.Domain.Scorecard;
using Golf.Domain.Shared.Golfer;
using Golf.Core.Exceptions;
using Golf.Domain.Courses;
using Golf.Domain.GolferData;
using Golf.Domain.Bookings;
using Golf.Domain.SocialNetwork;

namespace Golf.Services
{
    public class SeedDataService
    {
        private readonly CourseExtensionRepository _courseExtensionRepository;
        private readonly DatabaseTransaction _databaseTransaction;
        private readonly ScorecardRepository _scorecardRepository;
        private readonly LocationRepository _locationRepository;
        private readonly ProfileRepository _profileRepository;
        private readonly CourseRepository _courseRepository;
        private readonly GolferRepository _golferRepository;
        private readonly UserManager<Golfer> _golferManager;
        private readonly ProductRepository _productRepository;
        private readonly GroupRepository _groupRepository;
        private readonly GroupMemberRepository _groupMemberRepository;

        public SeedDataService(
            CourseExtensionRepository courseExtensionRepository,
            DatabaseTransaction databaseTransaction,
            ScorecardRepository scorecardRepository,
            LocationRepository locationRepository,
            ProfileRepository profileRepository,
            CourseRepository courseRepository,
            GolferRepository golferRepository,
            UserManager<Golfer> golferManager,
            ProductRepository productRepository,
            GroupRepository groupRepository,
            GroupMemberRepository groupMemberRepository

            )
        {
            _courseExtensionRepository = courseExtensionRepository;
            _scorecardRepository = scorecardRepository;
            _databaseTransaction = databaseTransaction;
            _locationRepository = locationRepository;
            _profileRepository = profileRepository;
            _courseRepository = courseRepository;
            _golferRepository = golferRepository;
            _golferManager = golferManager;
            _productRepository = productRepository;
            _groupMemberRepository = groupMemberRepository;
            _groupRepository = groupRepository;

        }

        public bool SeedCourse()
        {
            Guid CourseID = Guid.Parse("46c3c7e7-6062-4db3-aded-4c76cef96bcf");
            Course course = _courseRepository.Get(CourseID);
            if (course != null)
            {
                _courseRepository.RemoveEntity(course);
            }

            if (true)
            {
                var location = _locationRepository.Get(Guid.Parse("41cd0aa1-0b91-4fab-83fc-a2aa9d30b1ee"));
                if (location == null)
                {
                    throw new NotFoundException("Seed location first");
                }
                course = new Course
                {
                    Location = location,
                    OwnerID = new Guid("23bf4220-724a-43a4-ab15-4d009b6086ac"),
                    MainVersionID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
                    ID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
                    Cover = "",
                    Name = "Van Tri Golf Club",
                    Description = "20k MoMo",
                    Extensions = _courseExtensionRepository.GetAll().Select(extension => extension.ID).ToList(),

                    TotalHoles = 18,
                    PhotoNames = "",
                    Version = 0,
                    IsConfirmed = true,
                    Par = 80,
                    CourseHoles = new List<CourseHole>()
                            {
                                new CourseHole
                                {
                                    Par =4,
                                    Index = 1,
                                    StrokeIndex = 15,
                                    BlueTeeDistance = 361,
                                    BlackTeeDistance = 396,
                                    RedTeeDistance = 287,
                                    WhiteTeeDistance = 322
                                },
                                new CourseHole
                                {
                                    Par =4,
                                    Index = 2,
                                    StrokeIndex = 13,
                                    BlueTeeDistance = 349,
                                    BlackTeeDistance = 387,
                                    RedTeeDistance = 276,
                                    WhiteTeeDistance = 319
                                },
                                new CourseHole
                                {
                                    Par =4,
                                    Index = 3,
                                    StrokeIndex = 3,
                                    BlueTeeDistance = 393,
                                    BlackTeeDistance = 428,
                                    RedTeeDistance = 321,
                                    WhiteTeeDistance = 358
                                },
                                new CourseHole
                                {
                                    Par =3,
                                    Index = 4,
                                    StrokeIndex = 11,
                                    BlackTeeDistance = 428,
                                    BlueTeeDistance = 201,
                                    RedTeeDistance = 109,
                                    WhiteTeeDistance = 142
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 5,
                                    StrokeIndex = 7,
                                    BlueTeeDistance = 413,
                                    BlackTeeDistance = 443,
                                    RedTeeDistance = 327,
                                    WhiteTeeDistance = 378
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 6,
                                    StrokeIndex = 5,
                                    BlueTeeDistance = 546,
                                    BlackTeeDistance = 577,
                                    RedTeeDistance = 471,
                                    WhiteTeeDistance = 505
                                },
                                new CourseHole
                                {
                                    Par = 3,
                                    Index = 7,
                                    StrokeIndex = 17,
                                    BlueTeeDistance = 167,
                                    BlackTeeDistance = 187,
                                    RedTeeDistance = 135,
                                    WhiteTeeDistance = 147
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 8,
                                    StrokeIndex = 1,
                                    BlueTeeDistance = 359,
                                    BlackTeeDistance = 394,
                                    RedTeeDistance = 279,
                                    WhiteTeeDistance = 324
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 9,
                                    StrokeIndex = 9,
                                    BlueTeeDistance = 543,
                                    BlackTeeDistance = 573,
                                    RedTeeDistance = 470,
                                    WhiteTeeDistance = 509
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 10,
                                    StrokeIndex = 10,
                                    WhiteTeeDistance = 327,
                                    RedTeeDistance = 286,
                                    BlueTeeDistance = 369,
                                    BlackTeeDistance = 401
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 11,
                                    StrokeIndex = 8,
                                    WhiteTeeDistance = 527,
                                    RedTeeDistance = 474,
                                    BlueTeeDistance = 558,
                                    BlackTeeDistance = 578
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 12,
                                    StrokeIndex = 6,
                                    WhiteTeeDistance = 369,
                                    RedTeeDistance = 332,
                                    BlueTeeDistance = 405,
                                    BlackTeeDistance = 439
                                },
                                new CourseHole
                                {
                                    Par = 3,
                                    Index = 13,
                                    StrokeIndex = 14,
                                    WhiteTeeDistance = 131,
                                    RedTeeDistance = 120,
                                    BlueTeeDistance = 163,
                                    BlackTeeDistance = 189
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 14,
                                    StrokeIndex = 2,
                                    WhiteTeeDistance = 380,
                                    RedTeeDistance = 336,
                                    BlueTeeDistance = 420,
                                    BlackTeeDistance = 453
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 15,
                                    StrokeIndex = 18,
                                    WhiteTeeDistance = 318,
                                    RedTeeDistance = 230,
                                    BlueTeeDistance = 346,
                                    BlackTeeDistance = 372
                                },
                                new CourseHole
                                {
                                    Par = 3,
                                    Index = 16,
                                    StrokeIndex = 16,
                                    WhiteTeeDistance = 132,
                                    RedTeeDistance = 105,
                                    BlueTeeDistance = 188,
                                    BlackTeeDistance = 188
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 17,
                                    StrokeIndex = 4,
                                    WhiteTeeDistance = 524,
                                    RedTeeDistance = 482,
                                    BlueTeeDistance = 567,
                                    BlackTeeDistance = 603
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 18,
                                    StrokeIndex = 12,
                                    WhiteTeeDistance = 320,
                                    RedTeeDistance = 276,
                                    BlueTeeDistance = 354,
                                    BlackTeeDistance = 393
                                }
                            },
                    Tees = new List<Tee>()
                            {
                                new Tee
                                {  Name = "Blue", Color = Domain.Shared.Course.TeeColor.xFF2385F8, SlopeRating = 134, CourseRating = 71.7 },
                                new Tee
                                {  Name = "Black", Color = Domain.Shared.Course.TeeColor.xFF000000,SlopeRating = 145,CourseRating = 74.8 },
                                new Tee
                                {  Name = "White", Color = Domain.Shared.Course.TeeColor.xFFFFFFFF, SlopeRating = 126, CourseRating = 68.6 },
                                new Tee
                                {  Name = "Red",Color = Domain.Shared.Course.TeeColor.xFFFB2B2B, SlopeRating = 126, CourseRating = 71.5 },
                            },
                    CreatedDate = DateTime.Now,
                };
                course.MoreInformations = new List<CourseInformation>()
                {
                new CourseInformation() { Title = "Chính sách hủy", Info = { "Hủy đặt sân quý khách sẽ mất 100% phí chơi" } },
                new CourseInformation() { Title = "Ghép flight", Info = { "Quý khách có thể ghép đội với bạn chơi khác theo điều phối của sân" } },
                new CourseInformation() { Title = "Caddy và xe điện", Info = { "Phí caddy: 0đ/caddy", "Phí xe điện: 0đ/xe" } },
                new CourseInformation() { Title = "Chính sách", Info = { "Quý khách có thể đặt trước 7 ngày", "Người chời phải có mặt tại sân trước giờ chơi 20 phút" } }
                };
                _courseRepository.Add(course);
            }
            Course course1 = _courseRepository.Get(new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcd"));
            if (course1 != null)
            {
                _courseRepository.RemoveEntity(course1);
            }
            if (true)
            {
                var location1 = _locationRepository.Get(new Guid("41cd0aa1-0b91-4fab-83fc-a2aa9d30b1ee"));
                if (location1 == null)
                {
                    throw new NotFoundException("Seed location first");
                }
                course1 = new Course
                {
                    Location = location1,
                    MainVersionID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcd"),
                    ID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcd"),
                    OwnerID = new Guid("23bf4220-724a-43a4-ab15-4d009b6086ac"),
                    Cover = "",
                    Name = "Van Tri 1 Golf Club",
                    Description = "20k MoMo",
                    Extensions = _courseExtensionRepository.GetAll().Select(extension => extension.ID).ToList(),
                    TotalHoles = 18,
                    PhotoNames = "",
                    Version = 0,
                    IsConfirmed = true,
                    Par = 80,
                    CourseHoles = new List<CourseHole>()
                            {
                                new CourseHole
                                {
                                    Par =4,
                                    Index = 1,
                                    StrokeIndex = 15,
                                    BlueTeeDistance = 361,
                                    BlackTeeDistance = 396,
                                    RedTeeDistance = 287,
                                    WhiteTeeDistance = 322
                                },
                                new CourseHole
                                {
                                    Par =4,
                                    Index = 2,
                                    StrokeIndex = 13,
                                    BlueTeeDistance = 349,
                                    BlackTeeDistance = 387,
                                    RedTeeDistance = 276,
                                    WhiteTeeDistance = 319
                                },
                                new CourseHole
                                {
                                    Par =4,
                                    Index = 3,
                                    StrokeIndex = 3,
                                    BlueTeeDistance = 393,
                                    BlackTeeDistance = 428,
                                    RedTeeDistance = 321,
                                    WhiteTeeDistance = 358
                                },
                                new CourseHole
                                {
                                    Par =3,
                                    Index = 4,
                                    StrokeIndex = 11,
                                    BlackTeeDistance = 428,
                                    BlueTeeDistance = 201,
                                    RedTeeDistance = 109,
                                    WhiteTeeDistance = 142
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 5,
                                    StrokeIndex = 7,
                                    BlueTeeDistance = 413,
                                    BlackTeeDistance = 443,
                                    RedTeeDistance = 327,
                                    WhiteTeeDistance = 378
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 6,
                                    StrokeIndex = 5,
                                    BlueTeeDistance = 546,
                                    BlackTeeDistance = 577,
                                    RedTeeDistance = 471,
                                    WhiteTeeDistance = 505
                                },
                                new CourseHole
                                {
                                    Par = 3,
                                    Index = 7,
                                    StrokeIndex = 17,
                                    BlueTeeDistance = 167,
                                    BlackTeeDistance = 187,
                                    RedTeeDistance = 135,
                                    WhiteTeeDistance = 147
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 8,
                                    StrokeIndex = 1,
                                    BlueTeeDistance = 359,
                                    BlackTeeDistance = 394,
                                    RedTeeDistance = 279,
                                    WhiteTeeDistance = 324
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 9,
                                    StrokeIndex = 9,
                                    BlueTeeDistance = 543,
                                    BlackTeeDistance = 573,
                                    RedTeeDistance = 470,
                                    WhiteTeeDistance = 509
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 10,
                                    StrokeIndex = 10,
                                    WhiteTeeDistance = 327,
                                    RedTeeDistance = 286,
                                    BlueTeeDistance = 369,
                                    BlackTeeDistance = 401
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 11,
                                    StrokeIndex = 8,
                                    WhiteTeeDistance = 527,
                                    RedTeeDistance = 474,
                                    BlueTeeDistance = 558,
                                    BlackTeeDistance = 578
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 12,
                                    StrokeIndex = 6,
                                    WhiteTeeDistance = 369,
                                    RedTeeDistance = 332,
                                    BlueTeeDistance = 405,
                                    BlackTeeDistance = 439
                                },
                                new CourseHole
                                {
                                    Par = 3,
                                    Index = 13,
                                    StrokeIndex = 14,
                                    WhiteTeeDistance = 131,
                                    RedTeeDistance = 120,
                                    BlueTeeDistance = 163,
                                    BlackTeeDistance = 189
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 14,
                                    StrokeIndex = 2,
                                    WhiteTeeDistance = 380,
                                    RedTeeDistance = 336,
                                    BlueTeeDistance = 420,
                                    BlackTeeDistance = 453
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 15,
                                    StrokeIndex = 18,
                                    WhiteTeeDistance = 318,
                                    RedTeeDistance = 230,
                                    BlueTeeDistance = 346,
                                    BlackTeeDistance = 372
                                },
                                new CourseHole
                                {
                                    Par = 3,
                                    Index = 16,
                                    StrokeIndex = 16,
                                    WhiteTeeDistance = 132,
                                    RedTeeDistance = 105,
                                    BlueTeeDistance = 188,
                                    BlackTeeDistance = 188
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 17,
                                    StrokeIndex = 4,
                                    WhiteTeeDistance = 524,
                                    RedTeeDistance = 482,
                                    BlueTeeDistance = 567,
                                    BlackTeeDistance = 603
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 18,
                                    StrokeIndex = 12,
                                    WhiteTeeDistance = 320,
                                    RedTeeDistance = 276,
                                    BlueTeeDistance = 354,
                                    BlackTeeDistance = 393
                                }
                            },
                    Tees = new List<Tee>()
                            {
                                new Tee
                                {  Name = "Blue", Color = Domain.Shared.Course.TeeColor.xFF2385F8, SlopeRating = 134, CourseRating = 71.7 },
                                new Tee
                                {  Name = "Black", Color = Domain.Shared.Course.TeeColor.xFF000000,SlopeRating = 145,CourseRating = 74.8 },
                                new Tee
                                {  Name = "White", Color = Domain.Shared.Course.TeeColor.xFFFFFFFF, SlopeRating = 126, CourseRating = 68.6 },
                                new Tee
                                {  Name = "Red",Color = Domain.Shared.Course.TeeColor.xFFFB2B2B, SlopeRating = 126, CourseRating = 71.5 },
                            },
                    CreatedDate = DateTime.Now,
                };
                course1.MoreInformations = new List<CourseInformation>()
                {
                new CourseInformation() { Title = "Chính sách hủy", Info = { "Hủy đặt sân quý khách sẽ mất 100% phí chơi" } },
                new CourseInformation() { Title = "Ghép flight", Info = { "Quý khách có thể ghép đội với bạn chơi khác theo điều phối của sân" } },
                new CourseInformation() { Title = "Caddy và xe điện", Info = { "Phí caddy: 0đ/caddy", "Phí xe điện: 0đ/xe" } },
                new CourseInformation() { Title = "Chính sách", Info = { "Quý khách có thể đặt trước 7 ngày", "Người chời phải có mặt tại sân trước giờ chơi 20 phút" } }
                };
                _courseRepository.Add(course1);
            }
            Course course2 = _courseRepository.Get(new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"));
            if (course2 != null)
            {
                _courseRepository.RemoveEntity(course2);
            }
            if (true)
            {
                var location2 = _locationRepository.Get(new Guid("41cd0aa1-0b91-4fab-83fc-a2aa9d30b1aa"));
                if (location2 == null)
                {
                    throw new NotFoundException("Seed location first");
                }
                course2 = new Course
                {
                    Location = location2,
                    MainVersionID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
                    ID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
                    OwnerID = new Guid("23bf4220-724a-43a4-ab15-4d009b6086ac"),
                    Cover = "",
                    Name = "Sao Khue Golf Club",
                    Description = "20k MoMo",
                    Extensions = _courseExtensionRepository.GetAll().Select(extension => extension.ID).ToList(),

                    TotalHoles = 18,
                    PhotoNames = "",
                    Version = 0,
                    IsConfirmed = true,
                    CourseHoles =new List<CourseHole>()
                            {
                                new CourseHole
                                {
                                    Par =4,
                                    Index = 1,
                                    StrokeIndex = 15,
                                    BlueTeeDistance = 361,
                                    BlackTeeDistance = 396,
                                    RedTeeDistance = 287,
                                    WhiteTeeDistance = 322
                                },
                                new CourseHole
                                {
                                    Par =4,
                                    Index = 2,
                                    StrokeIndex = 13,
                                    BlueTeeDistance = 349,
                                    BlackTeeDistance = 387,
                                    RedTeeDistance = 276,
                                    WhiteTeeDistance = 319
                                },
                                new CourseHole
                                {
                                    Par =4,
                                    Index = 3,
                                    StrokeIndex = 3,
                                    BlueTeeDistance = 393,
                                    BlackTeeDistance = 428,
                                    RedTeeDistance = 321,
                                    WhiteTeeDistance = 358
                                },
                                new CourseHole
                                {
                                    Par =3,
                                    Index = 4,
                                    StrokeIndex = 11,
                                    BlackTeeDistance = 428,
                                    BlueTeeDistance = 201,
                                    RedTeeDistance = 109,
                                    WhiteTeeDistance = 142
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 5,
                                    StrokeIndex = 7,
                                    BlueTeeDistance = 413,
                                    BlackTeeDistance = 443,
                                    RedTeeDistance = 327,
                                    WhiteTeeDistance = 378
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 6,
                                    StrokeIndex = 5,
                                    BlueTeeDistance = 546,
                                    BlackTeeDistance = 577,
                                    RedTeeDistance = 471,
                                    WhiteTeeDistance = 505
                                },
                                new CourseHole
                                {
                                    Par = 3,
                                    Index = 7,
                                    StrokeIndex = 17,
                                    BlueTeeDistance = 167,
                                    BlackTeeDistance = 187,
                                    RedTeeDistance = 135,
                                    WhiteTeeDistance = 147
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 8,
                                    StrokeIndex = 1,
                                    BlueTeeDistance = 359,
                                    BlackTeeDistance = 394,
                                    RedTeeDistance = 279,
                                    WhiteTeeDistance = 324
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 9,
                                    StrokeIndex = 9,
                                    BlueTeeDistance = 543,
                                    BlackTeeDistance = 573,
                                    RedTeeDistance = 470,
                                    WhiteTeeDistance = 509
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 10,
                                    StrokeIndex = 10,
                                    WhiteTeeDistance = 327,
                                    RedTeeDistance = 286,
                                    BlueTeeDistance = 369,
                                    BlackTeeDistance = 401
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 11,
                                    StrokeIndex = 8,
                                    WhiteTeeDistance = 527,
                                    RedTeeDistance = 474,
                                    BlueTeeDistance = 558,
                                    BlackTeeDistance = 578
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 12,
                                    StrokeIndex = 6,
                                    WhiteTeeDistance = 369,
                                    RedTeeDistance = 332,
                                    BlueTeeDistance = 405,
                                    BlackTeeDistance = 439
                                },
                                new CourseHole
                                {
                                    Par = 3,
                                    Index = 13,
                                    StrokeIndex = 14,
                                    WhiteTeeDistance = 131,
                                    RedTeeDistance = 120,
                                    BlueTeeDistance = 163,
                                    BlackTeeDistance = 189
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 14,
                                    StrokeIndex = 2,
                                    WhiteTeeDistance = 380,
                                    RedTeeDistance = 336,
                                    BlueTeeDistance = 420,
                                    BlackTeeDistance = 453
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 15,
                                    StrokeIndex = 18,
                                    WhiteTeeDistance = 318,
                                    RedTeeDistance = 230,
                                    BlueTeeDistance = 346,
                                    BlackTeeDistance = 372
                                },
                                new CourseHole
                                {
                                    Par = 3,
                                    Index = 16,
                                    StrokeIndex = 16,
                                    WhiteTeeDistance = 132,
                                    RedTeeDistance = 105,
                                    BlueTeeDistance = 188,
                                    BlackTeeDistance = 188
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 17,
                                    StrokeIndex = 4,
                                    WhiteTeeDistance = 524,
                                    RedTeeDistance = 482,
                                    BlueTeeDistance = 567,
                                    BlackTeeDistance = 603
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 18,
                                    StrokeIndex = 12,
                                    WhiteTeeDistance = 320,
                                    RedTeeDistance = 276,
                                    BlueTeeDistance = 354,
                                    BlackTeeDistance = 393
                                }
                            },
                    Tees = new List<Tee>()
                            {
                                new Tee
                                {  Name = "Blue", Color = Domain.Shared.Course.TeeColor.xFF2385F8, SlopeRating = 134, CourseRating = 71.7 },
                                new Tee
                                {  Name = "Black", Color = Domain.Shared.Course.TeeColor.xFF000000,SlopeRating = 145,CourseRating = 74.8 },
                                new Tee
                                {  Name = "White", Color = Domain.Shared.Course.TeeColor.xFFFFFFFF, SlopeRating = 126, CourseRating = 68.6 },
                                new Tee
                                {  Name = "Red",Color = Domain.Shared.Course.TeeColor.xFFFB2B2B, SlopeRating = 126, CourseRating = 71.5 },
                            },
                    CreatedDate = DateTime.Now,
                };
                course2.MoreInformations = new List<CourseInformation>()
                {
                new CourseInformation() { Title = "Chính sách hủy", Info = { "Hủy đặt sân quý khách sẽ mất 100% phí chơi" } },
                new CourseInformation() { Title = "Ghép flight", Info = { "Quý khách có thể ghép đội với bạn chơi khác theo điều phối của sân" } },
                new CourseInformation() { Title = "Caddy và xe điện", Info = { "Phí caddy: 0đ/caddy", "Phí xe điện: 0đ/xe" } },
                new CourseInformation() { Title = "Chính sách", Info = { "Quý khách có thể đặt trước 7 ngày", "Người chời phải có mặt tại sân trước giờ chơi 20 phút" } }
                };
                _courseRepository.Add(course2);

            }
            Course course3 = _courseRepository.Get(new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcb"));
            if (course3 != null)
            {
                _courseRepository.RemoveEntity(course3);
            }
            if (true)
            {
                var location3 = _locationRepository.Get(new Guid("41cd0aa1-0b91-4fab-83fc-a2aa9d30b1aa"));
                if (location3 == null)
                {
                    throw new NotFoundException("Seed location first");
                }
                course3 = new Course
                {
                    Location = location3,
                    MainVersionID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcb"),
                    ID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcb"),
                    OwnerID = new Guid("23bf4220-724a-43a4-ab15-4d009b6086ac"),
                    Cover = "",
                    Name = "Sao Khue2 Golf Club",
                    Description = "20k MoMo",
                    Extensions = _courseExtensionRepository.GetAll().Select(extension => extension.ID).ToList(),

                    TotalHoles = 18,
                    PhotoNames = "",
                    Version = 0,
                    IsConfirmed = true,
                    CourseHoles = new List<CourseHole>()
                            {
                                new CourseHole
                                {
                                    Par =4,
                                    Index = 1,
                                    StrokeIndex = 15,
                                    BlueTeeDistance = 361,
                                    BlackTeeDistance = 396,
                                    RedTeeDistance = 287,
                                    WhiteTeeDistance = 322
                                },
                                new CourseHole
                                {
                                    Par =4,
                                    Index = 2,
                                    StrokeIndex = 13,
                                    BlueTeeDistance = 349,
                                    BlackTeeDistance = 387,
                                    RedTeeDistance = 276,
                                    WhiteTeeDistance = 319
                                },
                                new CourseHole
                                {
                                    Par =4,
                                    Index = 3,
                                    StrokeIndex = 3,
                                    BlueTeeDistance = 393,
                                    BlackTeeDistance = 428,
                                    RedTeeDistance = 321,
                                    WhiteTeeDistance = 358
                                },
                                new CourseHole
                                {
                                    Par =3,
                                    Index = 4,
                                    StrokeIndex = 11,
                                    BlackTeeDistance = 428,
                                    BlueTeeDistance = 201,
                                    RedTeeDistance = 109,
                                    WhiteTeeDistance = 142
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 5,
                                    StrokeIndex = 7,
                                    BlueTeeDistance = 413,
                                    BlackTeeDistance = 443,
                                    RedTeeDistance = 327,
                                    WhiteTeeDistance = 378
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 6,
                                    StrokeIndex = 5,
                                    BlueTeeDistance = 546,
                                    BlackTeeDistance = 577,
                                    RedTeeDistance = 471,
                                    WhiteTeeDistance = 505
                                },
                                new CourseHole
                                {
                                    Par = 3,
                                    Index = 7,
                                    StrokeIndex = 17,
                                    BlueTeeDistance = 167,
                                    BlackTeeDistance = 187,
                                    RedTeeDistance = 135,
                                    WhiteTeeDistance = 147
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 8,
                                    StrokeIndex = 1,
                                    BlueTeeDistance = 359,
                                    BlackTeeDistance = 394,
                                    RedTeeDistance = 279,
                                    WhiteTeeDistance = 324
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 9,
                                    StrokeIndex = 9,
                                    BlueTeeDistance = 543,
                                    BlackTeeDistance = 573,
                                    RedTeeDistance = 470,
                                    WhiteTeeDistance = 509
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 10,
                                    StrokeIndex = 10,
                                    WhiteTeeDistance = 327,
                                    RedTeeDistance = 286,
                                    BlueTeeDistance = 369,
                                    BlackTeeDistance = 401
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 11,
                                    StrokeIndex = 8,
                                    WhiteTeeDistance = 527,
                                    RedTeeDistance = 474,
                                    BlueTeeDistance = 558,
                                    BlackTeeDistance = 578
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 12,
                                    StrokeIndex = 6,
                                    WhiteTeeDistance = 369,
                                    RedTeeDistance = 332,
                                    BlueTeeDistance = 405,
                                    BlackTeeDistance = 439
                                },
                                new CourseHole
                                {
                                    Par = 3,
                                    Index = 13,
                                    StrokeIndex = 14,
                                    WhiteTeeDistance = 131,
                                    RedTeeDistance = 120,
                                    BlueTeeDistance = 163,
                                    BlackTeeDistance = 189
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 14,
                                    StrokeIndex = 2,
                                    WhiteTeeDistance = 380,
                                    RedTeeDistance = 336,
                                    BlueTeeDistance = 420,
                                    BlackTeeDistance = 453
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 15,
                                    StrokeIndex = 18,
                                    WhiteTeeDistance = 318,
                                    RedTeeDistance = 230,
                                    BlueTeeDistance = 346,
                                    BlackTeeDistance = 372
                                },
                                new CourseHole
                                {
                                    Par = 3,
                                    Index = 16,
                                    StrokeIndex = 16,
                                    WhiteTeeDistance = 132,
                                    RedTeeDistance = 105,
                                    BlueTeeDistance = 188,
                                    BlackTeeDistance = 188
                                },
                                new CourseHole
                                {
                                    Par = 5,
                                    Index = 17,
                                    StrokeIndex = 4,
                                    WhiteTeeDistance = 524,
                                    RedTeeDistance = 482,
                                    BlueTeeDistance = 567,
                                    BlackTeeDistance = 603
                                },
                                new CourseHole
                                {
                                    Par = 4,
                                    Index = 18,
                                    StrokeIndex = 12,
                                    WhiteTeeDistance = 320,
                                    RedTeeDistance = 276,
                                    BlueTeeDistance = 354,
                                    BlackTeeDistance = 393
                                }
                            },
                    Tees = new List<Tee>()
                            {
                                new Tee
                                {  Name = "Blue", Color = Domain.Shared.Course.TeeColor.xFF2385F8, SlopeRating = 134, CourseRating = 71.7 },
                                new Tee
                                {  Name = "Black", Color = Domain.Shared.Course.TeeColor.xFF000000,SlopeRating = 145,CourseRating = 74.8 },
                                new Tee
                                {  Name = "White", Color = Domain.Shared.Course.TeeColor.xFFFFFFFF, SlopeRating = 126, CourseRating = 68.6 },
                                new Tee
                                {  Name = "Red",Color = Domain.Shared.Course.TeeColor.xFFFB2B2B, SlopeRating = 126, CourseRating = 71.5 },
                            },
                    CreatedDate = DateTime.Now,
                };
                course3.MoreInformations = new List<CourseInformation>()
                {
                new CourseInformation() { Title = "Chính sách hủy", Info = { "Hủy đặt sân quý khách sẽ mất 100% phí chơi" } },
                new CourseInformation() { Title = "Ghép flight", Info = { "Quý khách có thể ghép đội với bạn chơi khác theo điều phối của sân" } },
                new CourseInformation() { Title = "Caddy và xe điện", Info = { "Phí caddy: 0đ/caddy", "Phí xe điện: 0đ/xe" } },
                new CourseInformation() { Title = "Chính sách", Info = { "Quý khách có thể đặt trước 7 ngày", "Người chời phải có mặt tại sân trước giờ chơi 20 phút" } }
                };
                _courseRepository.Add(course3);
            }


            return true;
        }

        public bool SeedLocation()
        {
            var location = _locationRepository.Get(Guid.Parse("41cd0aa1-0b91-4fab-83fc-a2aa9d30b1ee"));
            if (location == null)
            {
                location = new Location
                {
                    ID = new Guid("41cd0aa1-0b91-4fab-83fc-a2aa9d30b1ee"),
                    MainVersionID = new Guid("41cd0aa1-0b91-4fab-83fc-a2aa9d30b1ee"),
                    Name = "Van Tri Golf Club",
                    Address = "Kim No commune, Dong Anh District, Ha Noi city",
                    PhoneNumber = "84242176004",
                    FaxNumber = "12345678910",
                    HeadOffice = "Room 210-211 Daeha Business Center, 360 Kim Ma, Hà Nội",
                    Email = "sales@vantrigolf.com.vn",
                    Website = "www.vantrigolf.com.vn",
                    GPSAddress = new Domain.Common.GPSAddress
                    {
                        Latitude = 21.0291298,
                        Longitude = 105.8086038,
                    },
                    Country = "Vietnam",
                    Version = 0,
                    IsConfirmed = true,
                    Description = "20k MoMo",
                    OwnerID=new Guid("23bf4220-724a-43a4-ab15-4d009b6086ac")
                };
                _locationRepository.Add(location);
            }

            var location1 = _locationRepository.Get(new Guid("41cd0aa1-0b91-4fab-83fc-a2aa9d30b1aa"));
            if (location1 == null)
            {
                location1 = new Location
                {
                    ID = new Guid("41cd0aa1-0b91-4fab-83fc-a2aa9d30b1aa"),
                    MainVersionID = new Guid("41cd0aa1-0b91-4fab-83fc-a2aa9d30b1aa"),
                    Name = "Sao Khue Golf Club",
                    Address = "Kim No commune, Dong Anh District, Ha Noi city",
                    PhoneNumber = "84242176004",
                    FaxNumber = "12345678910",
                    HeadOffice = "Room 210-211 Daeha Business Center, 360 Kim Ma, Hà Nội",
                    Email = "sales@vantrigolf.com.vn",
                    Website = "www.vantrigolf.com.vn",
                    GPSAddress = new Domain.Common.GPSAddress
                    {
                        Latitude = 21.0291298,
                        Longitude = 105.8086038,
                    },
                    Country = "Vietnam",
                    Version = 0,
                    IsConfirmed = true,
                    Description = "20k MoMo",
                    OwnerID = new Guid("23bf4220-724a-43a4-ab15-4d009b6086ac")
                };
                _locationRepository.Add(location1);
            }
            return true;
        }


        public async System.Threading.Tasks.Task<bool> SeedAccountAsync()
        {

            var golfer = await _golferManager.FindByNameAsync("0987654321");
            if (golfer != null)
            {
                return true;
            }

            var newGolfer = new Golfer()
            {
                Id = new Guid("23bf4220-724a-43a4-ab15-4d009b6086ac"),
                PhoneNumber = "0987654321",
                UserName = "0987654321",
                NormalizedUserName = "0987654321",
                FirstName = "Khue",
                LastName = "Sao",
                Avatar = "",
                Cover = "",
                Handicap = 30.3,
                IDX = 0.0
            };
            var result = await _golferManager.CreateAsync(newGolfer, "12345678");
            if (result.Succeeded)
            {
                List<string> roles = new List<string>();
                roles.Add(RoleNormalizedName.SystemAdmin);
                roles.Add(RoleNormalizedName.CourseAdmin);
                roles.Add(RoleNormalizedName.GSA);
                roles.Add(RoleNormalizedName.Golfer);

                await _golferManager.AddToRolesAsync(newGolfer, roles);
                _profileRepository.Add(new Profile
                {
                    ID = newGolfer.Id,
                });

                return true;
            }
            throw new Exception(result.Errors.ToString());
        }

        public bool SeedScorecard()
        {
            Course course = _courseRepository.Get(Guid.Parse("46c3c7e7-6062-4db3-aded-4c76cef96bcf"));
            Golfer golfer = _golferRepository.Get(Guid.Parse("23bf4220-724a-43a4-ab15-4d009b6086ac"));
            var rand = new Random();

            double HandicapAfter = rand.NextDouble() * 50;
            double HandicapBefore = rand.NextDouble() * 50;
            double CourseHandicapAfter = rand.NextDouble() * 70;
            double CourseHandicapBefore = rand.NextDouble() * 70;
            double HandicapDifferential = rand.NextDouble() * 60;
            double System36Handicap = rand.NextDouble() * 60;

            var scorecard = new Scorecard
            {
                Owner = golfer,
                //OwnerID = Guid.Parse("23bf4220-724a-43a4-ab15-4d009b6086ac"),
                CreatedBy = Guid.Parse("23bf4220-724a-43a4-ab15-4d009b6086ac"),
                CreatedDate = DateTime.Now,
                ID = Guid.NewGuid(),
                CourseID = course.ID,
                Course = course,
                HandicapAfter = Convert.ToDouble(HandicapAfter.ToString().Substring(0, HandicapAfter.ToString().IndexOf(".") + 2)),
                HandicapBefore = Convert.ToDouble(HandicapBefore.ToString().Substring(0, HandicapBefore.ToString().IndexOf(".") + 2)),
                CourseHandicapAfter = Convert.ToDouble(CourseHandicapAfter.ToString().Substring(0, CourseHandicapAfter.ToString().IndexOf(".") + 2)),
                CourseHandicapBefore = Convert.ToDouble(CourseHandicapBefore.ToString().Substring(0, CourseHandicapBefore.ToString().IndexOf(".") + 2)),
                Achievements = new Achievements()
                {
                    Albatrosses = new List<int>(),
                    Pars = new List<int>() { 6, 9 },
                    Bogeys = new List<int>() { 1, 2, 3, 5, 8 },
                    Eagles = new List<int>(),
                    Birdies= new List<int>(),
                    Condors= new List<int>(),
                    HoleInOnes= new List<int>(),
                    LowerScores = new List<int>(),
                    DoubleBogeys = new List<int>() { 4,7},
                    TripleBogeys = new List<int>() ,

                },
                HandicapDifferential = Convert.ToDouble(HandicapDifferential.ToString().Substring(0, HandicapDifferential.ToString().IndexOf(".") + 2)),
                Date = DateTime.Now,
                Grosses = rand.Next(course.Par, course.Par + 20),
                InputType = ScorecardInputType.AfterPlay,
                //IsPending = false,
                //IsVerified = true,
                RealGrosses = rand.Next(course.Par, 100),
                Tee = course.Tees[0],
                Holes = new List<Hole>() {
                new Hole
                        {
                            Index = 1,
                            Fairway = Fairway.Hit,
                            Grosses = 5,
                            ClubOfTee = "",
                            Putts = 0,
                            SandShots = 0,
                            PenaltyStrokes = 0,
                            Note = "sdada"
                        },
                new Hole
                {
                    Index = 1,
                    Fairway = Fairway.Hit,
                    Grosses = 5,
                    ClubOfTee = "",
                    Putts = 0,
                    SandShots = 0,
                    PenaltyStrokes = 0,
                    Note = "sdada"
                },
                new Hole
                {
                    Index = 1,
                    Fairway = Fairway.Hit,
                    Grosses = 5,
                    ClubOfTee = "",
                    Putts = 0,
                    SandShots = 0,
                    PenaltyStrokes = 0,
                    Note = "sdada"
                },
                new Hole
                {
                    Index = 1,
                    Fairway = Fairway.Hit,
                    Grosses = 5,
                    ClubOfTee = "",
                    Putts = 0,
                    SandShots = 0,
                    PenaltyStrokes = 0,
                    Note = "sdada"
                },
                new Hole
                {
                    Index = 1,
                    Fairway = Fairway.Hit,
                    Grosses = 5,
                    ClubOfTee = "",
                    Putts = 0,
                    SandShots = 0,
                    PenaltyStrokes = 0,
                    Note = "sdada"
                },
                new Hole
                {
                    Index = 1,
                    Fairway = Fairway.Hit,
                    Grosses = 5,
                    ClubOfTee = "",
                    Putts = 0,
                    SandShots = 0,
                    PenaltyStrokes = 0,
                    Note = "sdada"
                },
                new Hole
                {
                    Index = 1,
                    Fairway = Fairway.Hit,
                    Grosses = 5,
                    ClubOfTee = "",
                    Putts = 0,
                    SandShots = 0,
                    PenaltyStrokes = 0,
                    Note = "sdada"
                },
                new Hole
                {
                    Index = 1,
                    Fairway = Fairway.Hit,
                    Grosses = 5,
                    ClubOfTee = "",
                    Putts = 0,
                    SandShots = 0,
                    PenaltyStrokes = 0,
                    Note = "sdada"
                },
                new Hole
                {
                    Index = 1,
                    Fairway = Fairway.Hit,
                    Grosses = 5,
                    ClubOfTee = "",
                    Putts = 0,
                    SandShots = 0,
                    PenaltyStrokes = 0,
                    Note = "sdada"
                },
                    new Hole
                {
                    Index = 1,
                    Fairway = Fairway.Hit,
                    Grosses = 5,
                    ClubOfTee = "",
                    Putts = 0,
                    SandShots = 0,
                    PenaltyStrokes = 0,
                    Note = "sdada"
                },
                    new Hole
                {
                    Index = 1,
                    Fairway = Fairway.Hit,
                    Grosses = 5,
                    ClubOfTee = "",
                    Putts = 0,
                    SandShots = 0,
                    PenaltyStrokes = 0,
                    Note = "sdada"

                }

                        },
                IsConfirmed = true,
                System36Handicap = Convert.ToDouble(System36Handicap.ToString().Substring(0, System36Handicap.ToString().IndexOf(".") + 2)),
                ParsAverage=new List<double>() { 5,5,5},
                Type = ScorecardType.Posted,
            };

    // for (int i = 0; i < 18; i++)
    // {
    //     int achievement = rand.Next(0, 10);
    //     switch (achievement)
    //     {
    //         case 0:
    //             {
    //                 scorecard.Achievements.HoleInOnes = 1;
    //                 break;
    //             }
    //         case 1:
    //             {
    //                 scorecard.Achievements.Condors = 1;
    //                 break;
    //             }
    //         case 2:
    //             {
    //                 scorecard.Achievements.Albatrosses = 1;
    //                 break;
    //             }
    //         case 3:
    //             {
    //                 scorecard.Achievements.Eagles = 1;
    //                 break;
    //             }
    //         case 4:
    //             {
    //                 scorecard.Achievements.Birdies = 1;
    //                 break;
    //             }
    //         case 5:
    //             {
    //                 scorecard.Achievements.Pars = 1;
    //                 break;
    //             }
    //         case 6:
    //             {
    //                 scorecard.Achievements.Bogeys = 1;
    //                 break;
    //             }
    //         case 7:
    //             {
    //                 scorecard.Achievements.DoubleBogeys = 1;
    //                 break;
    //             }
    //         case 8:
    //             {
    //                 scorecard.Achievements.TripleBogeys = 1;
    //                 break;
    //             }
    //         case 9:
    //             {
    //                 scorecard.Achievements.LowerScores = 1;
    //                 break;
    //             }
    //     }
    // }

    _scorecardRepository.Add(scorecard);
            return true;
        }


public bool SeedProduct()
{
    var product1 = _productRepository.Get(new Guid("4a53aad8-f7a4-4521-a87a-1760ba39056e"));

    if (product1 != null)
    {
        _productRepository.RemoveEntity(product1);
    }

    product1 = new Product
    {
        ID = new Guid("4a53aad8-f7a4-4521-a87a-1760ba39056e"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
        Price = 1000000,
        MaxPlayer = 4,
        Promotion = 0.15,
        Date = DateTime.Now.AddDays(1),
        TeeTime = TimeSpan.Parse("10:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product1);

    var product2 = _productRepository.Get(new Guid("4a53aad8-f7a4-4521-a87a-1760ba39056c"));

    if (product2 != null)
    {
        _productRepository.RemoveEntity(product2);
    }
    product2 = new Product
    {
        ID = new Guid("4a53aad8-f7a4-4521-a87a-1760ba39056c"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
        Price = 2000000,
        MaxPlayer = 4,
        Promotion = 0.25,
        Date = DateTime.Now.AddDays(1),
        TeeTime = TimeSpan.Parse("14:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product2);


    var product3 = _productRepository.Get(new Guid("4a53aad8-f7a4-4521-a87a-1760ba39056b"));
    if (product3 != null)
    {
        _productRepository.RemoveEntity(product3);
    }
    product3 = new Product
    {
        ID = new Guid("4a53aad8-f7a4-4521-a87a-1760ba39056b"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
        Price = 3000000,
        MaxPlayer = 4,
        Promotion = 0.35,
        Date = DateTime.Now.AddDays(1),
        TeeTime = TimeSpan.Parse("18:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product3);


    var product4 = _productRepository.Get(new Guid("4a53aad8-f7a4-4521-a87a-1760ba39056a"));
    if (product4 != null)
    {
        _productRepository.RemoveEntity(product4);
    }
    product4 = new Product
    {
        ID = new Guid("4a53aad8-f7a4-4521-a87a-1760ba39056a"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
        Price = 3000000,
        MaxPlayer = 4,
        Promotion = 0.45,
        Date = DateTime.Now.AddDays(1),
        TeeTime = TimeSpan.Parse("20:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product4);

    var product5 = _productRepository.Get(new Guid("4a53aad8-f7a4-4521-a87a-1760ba39056d"));
    if (product5 != null)
    {
        _productRepository.RemoveEntity(product5);
    }
    product5 = new Product
    {
        ID = new Guid("4a53aad8-f7a4-4521-a87a-1760ba39056d"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
        Price = 2000000,
        MaxPlayer = 4,
        Promotion = 0.35,
        Date = DateTime.Now.AddDays(1),
        TeeTime = TimeSpan.Parse("18:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product5);

    var product6 = _productRepository.Get(new Guid("4a53aad8-f7a4-4521-a87a-1760ba39046a"));
    if (product6 != null)
    {
        _productRepository.RemoveEntity(product6);
    }
    product6 = new Product
    {
        ID = new Guid("4a53aad8-f7a4-4521-a87a-1760ba39046a"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
        Price = 1000000,
        MaxPlayer = 4,
        Promotion = 0.15,
        Date = DateTime.Now.AddDays(1),
        TeeTime = TimeSpan.Parse("16:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product6);
    var product7 = _productRepository.Get(new Guid("5a53aad8-f7a4-4521-a87a-1760ba39056e"));

    if (product7 != null)
    {
        _productRepository.RemoveEntity(product7);
    }

    product7 = new Product
    {
        ID = new Guid("5a53aad8-f7a4-4521-a87a-1760ba39056e"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
        Price = 1000000,
        MaxPlayer = 4,
        Promotion = 0.15,
        Date = DateTime.Now.AddDays(2),
        TeeTime = TimeSpan.Parse("10:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product7);

    var product8 = _productRepository.Get(new Guid("6a53aad8-f7a4-4521-a87a-1760ba39056c"));

    if (product8 != null)
    {
        _productRepository.RemoveEntity(product8);
    }
    product8 = new Product
    {
        ID = new Guid("6a53aad8-f7a4-4521-a87a-1760ba39056c"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
        Price = 2000000,
        MaxPlayer = 4,
        Promotion = 0.25,
        Date = DateTime.Now.AddDays(2),
        TeeTime = TimeSpan.Parse("14:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product8);


    var product9 = _productRepository.Get(new Guid("7a53aad8-f7a4-4521-a87a-1760ba39056b"));
    if (product9 != null)
    {
        _productRepository.RemoveEntity(product9);
    }
    product9 = new Product
    {
        ID = new Guid("7a53aad8-f7a4-4521-a87a-1760ba39056b"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
        Price = 3000000,
        MaxPlayer = 4,
        Promotion = 0.35,
        Date = DateTime.Now.AddDays(2),
        TeeTime = TimeSpan.Parse("18:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product9);


    var product10 = _productRepository.Get(new Guid("8a53aad8-f7a4-4521-a87a-1760ba39056a"));
    if (product10 != null)
    {
        _productRepository.RemoveEntity(product10);
    }
    product10 = new Product
    {
        ID = new Guid("8a53aad8-f7a4-4521-a87a-1760ba39056a"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
        Price = 3000000,
        MaxPlayer = 4,
        Promotion = 0.45,
        Date = DateTime.Now.AddDays(2),
        TeeTime = TimeSpan.Parse("20:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product10);

    var product11 = _productRepository.Get(new Guid("9a53aad8-f7a4-4521-a87a-1760ba39056d"));
    if (product11 != null)
    {
        _productRepository.RemoveEntity(product11);
    }
    product11 = new Product
    {
        ID = new Guid("9a53aad8-f7a4-4521-a87a-1760ba39056d"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
        Price = 2000000,
        MaxPlayer = 4,
        Promotion = 0.35,
        Date = DateTime.Now.AddDays(2),
        TeeTime = TimeSpan.Parse("18:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product11);

    var product12 = _productRepository.Get(new Guid("4153aad8-f7a4-4521-a87a-1760ba39046a"));
    if (product12 != null)
    {
        _productRepository.RemoveEntity(product12);
    }
    product12 = new Product
    {
        ID = new Guid("4153aad8-f7a4-4521-a87a-1760ba39046a"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
        Price = 1000000,
        MaxPlayer = 4,
        Promotion = 0.15,
        Date = DateTime.Now.AddDays(2),
        TeeTime = TimeSpan.Parse("16:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product12);
    var product13 = _productRepository.Get(new Guid("4253aad8-f7a4-4521-a87a-1760ba39056e"));

    if (product13 != null)
    {
        _productRepository.RemoveEntity(product13);
    }

    product13 = new Product
    {
        ID = new Guid("4253aad8-f7a4-4521-a87a-1760ba39056e"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
        Price = 1000000,
        MaxPlayer = 4,
        Promotion = 0.15,
        Date = DateTime.Now.AddDays(3),
        TeeTime = TimeSpan.Parse("10:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product13);

    var product14 = _productRepository.Get(new Guid("4353aad8-f7a4-4521-a87a-1760ba39056c"));

    if (product14 != null)
    {
        _productRepository.RemoveEntity(product14);
    }
    product14 = new Product
    {
        ID = new Guid("4353aad8-f7a4-4521-a87a-1760ba39056c"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
        Price = 2000000,
        MaxPlayer = 4,
        Promotion = 0.25,
        Date = DateTime.Now.AddDays(3),
        TeeTime = TimeSpan.Parse("14:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product14);


    var product15 = _productRepository.Get(new Guid("4453aad8-f7a4-4521-a87a-1760ba39056b"));
    if (product15 != null)
    {
        _productRepository.RemoveEntity(product15);
    }
    product15 = new Product
    {
        ID = new Guid("4453aad8-f7a4-4521-a87a-1760ba39056b"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
        Price = 3000000,
        MaxPlayer = 4,
        Promotion = 0.35,
        Date = DateTime.Now.AddDays(3),
        TeeTime = TimeSpan.Parse("18:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product15);


    var product16 = _productRepository.Get(new Guid("4553aad8-f7a4-4521-a87a-1760ba39056a"));
    if (product16 != null)
    {
        _productRepository.RemoveEntity(product16);
    }
    product16 = new Product
    {
        ID = new Guid("4553aad8-f7a4-4521-a87a-1760ba39056a"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
        Price = 3000000,
        MaxPlayer = 4,
        Promotion = 0.45,
        Date = DateTime.Now.AddDays(3),
        TeeTime = TimeSpan.Parse("20:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product16);

    var product17 = _productRepository.Get(new Guid("4653aad8-f7a4-4521-a87a-1760ba39056d"));
    if (product17 != null)
    {
        _productRepository.RemoveEntity(product17);
    }
    product17 = new Product
    {
        ID = new Guid("4653aad8-f7a4-4521-a87a-1760ba39056d"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
        Price = 2000000,
        MaxPlayer = 4,
        Promotion = 0.35,
        Date = DateTime.Now.AddDays(3),
        TeeTime = TimeSpan.Parse("18:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product17);

    var product18 = _productRepository.Get(new Guid("4753aad8-f7a4-4521-a87a-1760ba39046a"));
    if (product18 != null)
    {
        _productRepository.RemoveEntity(product18);
    }
    product18 = new Product
    {
        ID = new Guid("4753aad8-f7a4-4521-a87a-1760ba39046a"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
        Price = 1000000,
        MaxPlayer = 4,
        Promotion = 0.15,
        Date = DateTime.Now.AddDays(3),
        TeeTime = TimeSpan.Parse("16:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product18);
    var product19 = _productRepository.Get(new Guid("4853aad8-f7a4-4521-a87a-1760ba39056e"));

    if (product19 != null)
    {
        _productRepository.RemoveEntity(product19);
    }

    product19 = new Product
    {
        ID = new Guid("4853aad8-f7a4-4521-a87a-1760ba39056e"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
        Price = 1000000,
        MaxPlayer = 4,
        Promotion = 0.15,
        Date = DateTime.Now.AddDays(4),
        TeeTime = TimeSpan.Parse("10:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product19);

    var product20 = _productRepository.Get(new Guid("4953aad8-f7a4-4521-a87a-1760ba39056c"));

    if (product20 != null)
    {
        _productRepository.RemoveEntity(product20);
    }
    product20 = new Product
    {
        ID = new Guid("4953aad8-f7a4-4521-a87a-1760ba39056c"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
        Price = 2000000,
        MaxPlayer = 4,
        Promotion = 0.25,
        Date = DateTime.Now.AddDays(4),
        TeeTime = TimeSpan.Parse("14:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product20);


    var product21 = _productRepository.Get(new Guid("4a54aad8-f7a4-4521-a87a-1760ba39056b"));
    if (product21 != null)
    {
        _productRepository.RemoveEntity(product21);
    }
    product21 = new Product
    {
        ID = new Guid("4a54aad8-f7a4-4521-a87a-1760ba39056b"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bca"),
        Price = 3000000,
        MaxPlayer = 4,
        Promotion = 0.35,
        Date = DateTime.Now.AddDays(4),
        TeeTime = TimeSpan.Parse("18:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product21);


    var product22 = _productRepository.Get(new Guid("4a55aad8-f7a4-4521-a87a-1760ba39056a"));
    if (product22 != null)
    {
        _productRepository.RemoveEntity(product22);
    }
    product22 = new Product
    {
        ID = new Guid("4a55aad8-f7a4-4521-a87a-1760ba39056a"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
        Price = 3000000,
        MaxPlayer = 4,
        Promotion = 0.45,
        Date = DateTime.Now.AddDays(4),
        TeeTime = TimeSpan.Parse("20:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product22);

    var product23 = _productRepository.Get(new Guid("4a56aad8-f7a4-4521-a87a-1760ba39056d"));
    if (product23 != null)
    {
        _productRepository.RemoveEntity(product23);
    }
    product23 = new Product
    {
        ID = new Guid("4a56aad8-f7a4-4521-a87a-1760ba39056d"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
        Price = 2000000,
        MaxPlayer = 4,
        Promotion = 0.35,
        Date = DateTime.Now.AddDays(4),
        TeeTime = TimeSpan.Parse("18:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product23);

    var product24 = _productRepository.Get(new Guid("4a57aad8-f7a4-4521-a87a-1760ba39046a"));
    if (product24 != null)
    {
        _productRepository.RemoveEntity(product24);
    }
    product24 = new Product
    {
        ID = new Guid("4a57aad8-f7a4-4521-a87a-1760ba39046a"),
        CourseID = new Guid("46c3c7e7-6062-4db3-aded-4c76cef96bcf"),
        Price = 1000000,
        MaxPlayer = 4,
        Promotion = 0.15,
        Date = DateTime.Now.AddDays(4),
        TeeTime = TimeSpan.Parse("16:40:33"),
        Description = "Sao Khue",
        LisExtensionID = new List<int>() { 1, 2, 3 }
    };
    _productRepository.Add(product24);

    return true;
}

        //public bool SeedGroup()
        //{
        //    var group = _groupRepository.Get(new Guid("53cfd580-3b97-4244-ada0-13944ebb7f6e"));
        //    if(group!=null)
        //    Group group = new Group();
        //    group.Description = "sao khue test";
        //    group.GroupStatus = 1;
        //    group.ID = Guid.NewGuid();
        //    group.Name = "Sao Khue Fan Club";
        //    group.Type = 1;
        //    _groupRepository.Add(group);

        //    Group group1 = new Group();
        //    group1.Description = "sao khue 1 test";
        //    group1.GroupStatus = 1;
        //    group1.ID = Guid.NewGuid();
        //    group1.Name = "Sao Khue1 Fan Club";
        //    group1.Type = 1;
        //    _groupRepository.Add(group);
        //}
    }
}