// using Golf.Core.Exceptions;
// using Golf.Domain.Courses;
// using Golf.Domain.Match;
// using Golf.EntityFrameworkCore.Repositories;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace Golf.Services.Matchs
// {
//     public class MatchService
//     {
//         private readonly MatchRepository _matchRepository;
//        public MatchService(MatchRepository matchRepository)
//         {
//             _matchRepository = matchRepository;
//         }
//         async public Task<Match> Add(Guid uId, Match  match)
//         {
//             match.CreatedBy = uId;
//             _matchRepository.Add(match);
//             return _matchRepository.Get(match.ID);


//         }

//           public IEnumerable<Match> GetByFilter(Guid uid,string fill)
//         {
//             switch (fill)
//             {
//                 case "5":
//                     {
//                         return _matchRepository.Entities.Where(m=>m.CreatedBy==uid).Take(5).OrderByDescending(m=>m.Date).ToList();

//                     }
//                 case "10":
//                     {
//                         return _matchRepository.Entities.Where(m => m.CreatedBy == uid).Take(10).OrderByDescending(m => m.Date).ToList();


//                     }
//                 case "20":
//                     {
//                         return _matchRepository.Entities.Where(m => m.CreatedBy == uid).Take(20).OrderByDescending(m => m.Date).ToList();

//                     }



//                 case "thisweek":
//                     {
//                         return _matchRepository.Entities.Where(m => m.CreatedBy == uid&&m.Date.DayOfWeek< DateTime.Now.DayOfWeek &&(DateTime.Now.DayOfYear - m.Date.DayOfYear)<7).OrderByDescending(m => m.Date).ToList();


//                     }
//                 case "thismonth":
//                     {
//                         return _matchRepository.Entities.Where(m => m.CreatedBy == uid && m.Date.Month == DateTime.Now.Month && m.Date.Year == DateTime.Now.Year).OrderByDescending(m => m.Date).ToList();

//                     }
//                 case "thisquarter":
//                     {
//                         switch(DateTime.Now.Month/4)
//                         {

//                             case 0:
//                                 {
//                                     return _matchRepository.Entities.Where(m => m.CreatedBy == uid && (m.Date.Month==1||m.Date.Month == 2||m.Date.Month == 3)).OrderByDescending(m => m.Date).ToList();


//                                 }
//                             case 1:
//                                 {
//                                     return _matchRepository.Entities.Where(m => m.CreatedBy == uid && (m.Date.Month == 4 || m.Date.Month == 5 || m.Date.Month == 6 )).OrderByDescending(m => m.Date).ToList();


//                                 }
//                             case 2:
//                                 {
//                                     return _matchRepository.Entities.Where(m => m.CreatedBy == uid && (m.Date.Month == 7 || m.Date.Month == 8 || m.Date.Month == 9 )).OrderByDescending(m => m.Date).ToList();


//                                 }
//                             case 3:
//                                 {
//                                     return _matchRepository.Entities.Where(m => m.CreatedBy == uid && (m.Date.Month == 10 || m.Date.Month == 11 || m.Date.Month == 12 )).OrderByDescending(m => m.Date).ToList();


//                                 }
//                             default:
//                                 {
//                                     throw new BadRequestException("This filter don't exit");
//                                     break;
//                                 }

//                         }    


//                         break;
//                     }
//                 case "6month":
//                     {
//                         return _matchRepository.Entities.Where(m => m.CreatedBy == uid &&   6>(DateTime.Now.Month- m.Date.Month)&& (DateTime.Now.Month - m.Date.Month) >= 0 && m.Date.Year == DateTime.Now.Year).OrderByDescending(m => m.Date).ToList();

//                     }
//                 case "thisyear":
//                     {
//                         return _matchRepository.Entities.Where(m => m.CreatedBy == uid && m.Date.Year == DateTime.Now.Year).OrderByDescending(m => m.Date).ToList();


//                     }
//                 case "all":
//                     {

//                         return _matchRepository.Entities.Where(m => m.CreatedBy == uid).OrderByDescending(m => m.Date).ToList();

//                     }
//                 default:
//                     {
//                         DateTime dateTime = DateTime.Parse(fill);
//                         if (dateTime!=null)
//                         {
//                             return _matchRepository.Entities.Where(m => m.CreatedBy == uid && 6 > (DateTime.Now.Month - m.Date.Month) && (DateTime.Now.Month - m.Date.Month) >= 0 && m.Date.Year == DateTime.Now.Year).OrderByDescending(m => m.Date).ToList();

//                         }
//                         else
//                         {
//                             throw new BadRequestException("This filter don't exit");
//                         }

//                     }


//             }



//         }
//         async public Task<Match> Get(Guid id)
//         {

//             var result = _matchRepository.Get(id);

//             if (result != null)
//             {
//                 return result;
//             }
//             else
//             {
//                 throw new BadRequestException("Not found Match !");
//             }


//         }

//         async public Task<Match> Edit(Guid uId, Match match)
//         {
//             if (uId == match.CreatedBy)
//             {

//                 _matchRepository.UpdateEntity(match);

//                 return match;

//             }
//             else
//             {
//                 throw new BadRequestException("Edit Post Error !");
//             }

//         }
//         async public Task<Match> Delete(Guid uId, Guid id)
//         {
//             var match = _matchRepository.Get(id);
//             if (match != null)
//             {

//                 if (uId == match.CreatedBy)
//                 {
//                     _matchRepository.RemoveEntity(match);
//                     return match;
//                 }
//                 else
//                 {
//                     throw new BadRequestException("Can not delete match !");
//                 }
//             }
//             else
//             {
//                 throw new BadRequestException("Can not delete match !");
//             }
//         }

//     }
// }
